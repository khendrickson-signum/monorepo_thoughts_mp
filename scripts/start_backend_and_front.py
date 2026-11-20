#!/usr/bin/env python3
"""Start the local API and Angular dev servers.

Anything already listening on those ports is stopped first, so a previous
run cannot keep the new servers from binding.
"""

import os
import signal
import subprocess
import sys
import time
from pathlib import Path


REPO_ROOT = Path(__file__).resolve().parent.parent
FRONTEND_ROOT = REPO_ROOT / "frontend"
API_PROJECT = (
    REPO_ROOT
    / "backend"
    / "SampleMonoRepo.DotNetApi"
    / "SampleMonoRepo.DotNetApi.csproj"
)
API_PORT = 21000
FRONTEND_PORT = 20000


def _listen_inodes(port: int) -> set[str]:
    inodes: set[str] = set()
    port_hex = f"{port:04X}"
    for proc_net in (Path("/proc/net/tcp"), Path("/proc/net/tcp6")):
        try:
            lines = proc_net.read_text().splitlines()
        except OSError:
            continue
        for line in lines[1:]:
            fields = line.split()
            if len(fields) < 10:
                continue
            local_port = fields[1].rsplit(":", 1)[-1]
            if local_port.upper() == port_hex and fields[3] == "0A":
                inodes.add(fields[9])
    return inodes


def pids_listening_on(port: int) -> set[int]:
    inodes = _listen_inodes(port)
    if not inodes:
        return set()

    sockets = {f"socket:[{inode}]" for inode in inodes}
    pids: set[int] = set()
    try:
        entries = list(Path("/proc").iterdir())
    except OSError:
        return pids

    for entry in entries:
        if not entry.name.isdigit():
            continue
        try:
            fds = list((entry / "fd").iterdir())
        except OSError:
            continue
        for fd in fds:
            try:
                target = os.readlink(fd)
            except OSError:
                continue
            if target in sockets:
                pids.add(int(entry.name))
                break
    return pids


def _stat_fields(pid: int) -> list[str] | None:
    try:
        stat = Path(f"/proc/{pid}/stat").read_text()
    except OSError:
        return None
    marker = stat.rfind(")")
    if marker < 0:
        return None
    return stat[marker + 2 :].split()


def _descendants(pid: int) -> set[int]:
    children: dict[int, list[int]] = {}
    try:
        entries = list(Path("/proc").iterdir())
    except OSError:
        return set()

    for entry in entries:
        if not entry.name.isdigit():
            continue
        fields = _stat_fields(int(entry.name))
        if fields is None or len(fields) < 2:
            continue
        try:
            children.setdefault(int(fields[1]), []).append(int(entry.name))
        except ValueError:
            continue

    found: set[int] = set()
    stack = [pid]
    while stack:
        current = stack.pop()
        for child in children.get(current, []):
            if child not in found:
                found.add(child)
                stack.append(child)
    return found


def _signal_targets(groups: set[int], pids: set[int], sig: int) -> None:
    for pgid in groups:
        try:
            os.killpg(pgid, sig)
        except ProcessLookupError:
            continue
        except PermissionError:
            print(f"No permission to signal process group {pgid}.", file=sys.stderr, flush=True)
            raise SystemExit(1) from None

    for pid in pids:
        if pid == os.getpid():
            continue
        try:
            os.kill(pid, sig)
        except ProcessLookupError:
            continue
        except PermissionError:
            print(f"No permission to signal process {pid}.", file=sys.stderr, flush=True)
            raise SystemExit(1) from None


def _wait_until_clear(port: int, timeout: float) -> bool:
    deadline = time.monotonic() + timeout
    while True:
        if not pids_listening_on(port):
            return True
        if time.monotonic() >= deadline:
            return False
        time.sleep(0.2)


def stop_listeners(port: int) -> None:
    listeners = pids_listening_on(port) - {os.getpid()}
    if not listeners:
        return

    names = ", ".join(str(pid) for pid in sorted(listeners))
    print(f"Stopping existing process on port {port} (pid {names}).", flush=True)

    own_pgid = os.getpgrp()
    groups: set[int] = set()
    direct: set[int] = set()
    for pid in listeners:
        try:
            pgid = os.getpgid(pid)
        except ProcessLookupError:
            continue
        # A listener started in this process group is signaled directly so a
        # group signal cannot include this script. Other groups are the
        # previous server session, including npm or dotnet and their children.
        if pgid in (0, 1, own_pgid):
            direct.add(pid)
            direct.update(_descendants(pid) - {os.getpid()})
        else:
            groups.add(pgid)

    _signal_targets(groups, direct, signal.SIGTERM)
    if _wait_until_clear(port, 10):
        return

    _signal_targets(groups, direct, signal.SIGKILL)
    if _wait_until_clear(port, 5):
        return

    print(f"Port {port} is still in use.", file=sys.stderr, flush=True)
    raise SystemExit(1)


def stop_process(process: subprocess.Popen[bytes]) -> None:
    if process.poll() is not None:
        return

    try:
        os.killpg(process.pid, signal.SIGTERM)
    except ProcessLookupError:
        return

    try:
        process.wait(timeout=10)
    except subprocess.TimeoutExpired:
        try:
            os.killpg(process.pid, signal.SIGKILL)
        except ProcessLookupError:
            return
        process.wait(timeout=5)


def main() -> None:
    stop_listeners(API_PORT)
    stop_listeners(FRONTEND_PORT)

    api = subprocess.Popen(
        [
            "dotnet",
            "run",
            "--project",
            str(API_PROJECT),
            "--launch-profile",
            "http",
        ],
        cwd=REPO_ROOT,
        start_new_session=True,
    )
    frontend = subprocess.Popen(
        ["npm", "start"],
        cwd=FRONTEND_ROOT,
        start_new_session=True,
    )
    processes = (api, frontend)
    interrupted = False

    def request_stop(signum: int, frame: object) -> None:
        del signum, frame
        nonlocal interrupted
        interrupted = True

    signal.signal(signal.SIGINT, request_stop)
    signal.signal(signal.SIGTERM, request_stop)

    print(f"API: http://localhost:{API_PORT}", flush=True)
    print(f"Frontend: http://localhost:{FRONTEND_PORT}", flush=True)
    print("Press Ctrl+C to stop both.", flush=True)

    exit_code = 0
    try:
        while not interrupted:
            finished = next(
                (process for process in processes if process.poll() is not None),
                None,
            )
            if finished is not None:
                exit_code = finished.returncode if finished.returncode is not None else 1
                break
            time.sleep(0.5)
    finally:
        for process in processes:
            stop_process(process)

    sys.exit(exit_code)


if __name__ == "__main__":
    main()
