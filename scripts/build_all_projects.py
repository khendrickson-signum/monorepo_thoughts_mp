#!/usr/bin/env python3
"""Build the .NET solution and the Angular app."""

import subprocess
import sys
from pathlib import Path


REPO_ROOT = Path(__file__).resolve().parent.parent
FRONTEND_ROOT = REPO_ROOT / "frontend"
SOLUTION = REPO_ROOT / "backend" / "SampleMonoRepo.sln"


def run(command: list[str], cwd: Path) -> None:
    print(f"\n> {' '.join(command)}", flush=True)
    completed = subprocess.run(command, cwd=cwd)
    if completed.returncode != 0:
        sys.exit(completed.returncode)


def main() -> None:
    run(["dotnet", "build", str(SOLUTION)], REPO_ROOT)
    run(["npm", "run", "build"], FRONTEND_ROOT)


if __name__ == "__main__":
    main()
