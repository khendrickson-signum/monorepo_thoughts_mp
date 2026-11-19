#!/usr/bin/env python3
"""Start the local API and Angular dev servers."""

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

    print("API: http://localhost:5082", flush=True)
    print("Frontend: http://localhost:4200", flush=True)
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
