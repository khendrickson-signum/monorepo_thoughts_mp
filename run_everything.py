#!/usr/bin/env python3
"""Build everything, run unit tests, start the databases, and start the local servers.

`dotnet test` builds the API and the test project. `ng test` compiles the
Angular app for the test runner. Neither runs the production `ng build`, so
this script builds first, then tests, then starts the database containers and
the servers. When the servers are up, it opens an HTML page of the running
containers grouped by client.
"""

import subprocess
import sys
from pathlib import Path


REPO_ROOT = Path(__file__).resolve().parent
SCRIPTS = (
    REPO_ROOT / "scripts" / "build_all_projects.py",
    REPO_ROOT / "scripts" / "run_unit_tests.py",
    REPO_ROOT / "scripts" / "launch_databases.py",
    REPO_ROOT / "scripts" / "start_backend_and_front.py",
)


def main() -> None:
    for script in SCRIPTS:
        print(f"\n> {sys.executable} {script.name}", flush=True)
        completed = subprocess.run([sys.executable, str(script)], cwd=REPO_ROOT)
        if completed.returncode != 0:
            sys.exit(completed.returncode)


if __name__ == "__main__":
    main()
