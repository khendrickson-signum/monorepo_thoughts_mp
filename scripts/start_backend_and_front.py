#!/usr/bin/env python3
"""Start the API and Angular dev servers in Docker.

Published ports match the previous local servers: API 21000, frontend 20000.
Containers left from a previous run, including anything else publishing those
ports, are removed first. Ctrl+C stops both.
"""

import shutil
import subprocess
import sys
from pathlib import Path


REPO_ROOT = Path(__file__).resolve().parent.parent
COMPOSE_FILE = REPO_ROOT / "docker-compose.yml"
API_PORT = 21000
FRONTEND_PORT = 20000


def compose_command() -> list[str]:
    return [
        "docker",
        "compose",
        "--project-directory",
        str(REPO_ROOT),
        "-f",
        str(COMPOSE_FILE),
    ]


def run(command: list[str]) -> int:
    completed = subprocess.run(command, cwd=REPO_ROOT)
    return completed.returncode


def require_docker() -> None:
    if shutil.which("docker") is None:
        print("Docker is required to start the servers.", file=sys.stderr)
        raise SystemExit(1)

    compose = subprocess.run(
        ["docker", "compose", "version"],
        cwd=REPO_ROOT,
        capture_output=True,
        text=True,
        check=False,
    )
    if compose.returncode != 0:
        message = compose.stderr.strip() or "Docker Compose is required to start the servers."
        print(message, file=sys.stderr)
        raise SystemExit(compose.returncode)

    info = subprocess.run(
        ["docker", "info"],
        cwd=REPO_ROOT,
        stdout=subprocess.DEVNULL,
        stderr=subprocess.PIPE,
        text=True,
        check=False,
    )
    if info.returncode != 0:
        message = info.stderr.strip() or "Docker is not running."
        print(message, file=sys.stderr)
        raise SystemExit(info.returncode)


def stop_containers_on_ports(ports: tuple[int, ...]) -> None:
    ids: list[str] = []
    for port in ports:
        completed = subprocess.run(
            ["docker", "ps", "-aq", "--filter", f"publish={port}"],
            cwd=REPO_ROOT,
            capture_output=True,
            text=True,
            check=False,
        )
        if completed.returncode != 0:
            print(completed.stderr, file=sys.stderr, end="")
            raise SystemExit(completed.returncode)
        ids.extend(line.strip() for line in completed.stdout.splitlines() if line.strip())

    unique = list(dict.fromkeys(ids))
    if not unique:
        return

    print("Stopping containers already using the server ports.", flush=True)
    if run(["docker", "rm", "-f", *unique]) != 0:
        raise SystemExit(1)


def main() -> None:
    require_docker()

    if run([*compose_command(), "down", "--remove-orphans"]) != 0:
        raise SystemExit(1)
    stop_containers_on_ports((API_PORT, FRONTEND_PORT))

    print(f"API: http://localhost:{API_PORT}", flush=True)
    print(f"Frontend: http://localhost:{FRONTEND_PORT}", flush=True)
    print("Press Ctrl+C to stop both.", flush=True)

    exit_code = 0
    try:
        exit_code = run(
            [
                *compose_command(),
                "up",
                "--build",
                "--abort-on-container-exit",
                "--remove-orphans",
            ]
        )
    except KeyboardInterrupt:
        exit_code = 0
    finally:
        down_code = run([*compose_command(), "down", "--remove-orphans"])
        if exit_code == 0 and down_code != 0:
            exit_code = down_code

    raise SystemExit(exit_code)


if __name__ == "__main__":
    main()
