#!/usr/bin/env python3
"""Run the .NET and Angular unit tests."""

import subprocess
import sys
from pathlib import Path


REPO_ROOT = Path(__file__).resolve().parent.parent
FRONTEND_ROOT = REPO_ROOT / "frontend"
DOTNET_TEST_PROJECT = (
    REPO_ROOT
    / "backend"
    / "SampleMonoRepo.DotNetApi.Tests"
    / "SampleMonoRepo.DotNetApi.Tests.csproj"
)


def run(command: list[str], cwd: Path) -> None:
    print(f"\n> {' '.join(command)}", flush=True)
    completed = subprocess.run(command, cwd=cwd)
    if completed.returncode != 0:
        sys.exit(completed.returncode)


def main() -> None:
    run(["dotnet", "test", str(DOTNET_TEST_PROJECT)], REPO_ROOT)
    run(["npm", "test", "--", "--watch=false"], FRONTEND_ROOT)


if __name__ == "__main__":
    main()
