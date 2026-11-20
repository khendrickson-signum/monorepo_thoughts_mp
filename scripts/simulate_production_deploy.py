#!/usr/bin/env python3
"""Deploy one frontend and one backend container per client configuration.

Reads the ClientCode enum and publishes host ports in blocks of 10 starting
at 30000. The first client uses 30010 and 30011, the second uses 30020 and
30021, and so on. Inside each container the frontend still listens on 20000
and the backend on 21000.

Each backend gets its own appsettings.json with ClientConfig set to that
client. The repo copy of appsettings.json is left unchanged.

The stack stays up in the background. Run this script with "down" to stop it.
"""

import json
import re
import shutil
import subprocess
import sys
from dataclasses import dataclass
from pathlib import Path


REPO_ROOT = Path(__file__).resolve().parent.parent
DEV_COMPOSE_FILE = REPO_ROOT / "docker-compose.yml"
DEPLOY_DIR = REPO_ROOT / ".simulate-production"
COMPOSE_FILE = DEPLOY_DIR / "docker-compose.yml"
CLIENT_CODE_FILE = (
    REPO_ROOT
    / "backend"
    / "SampleMonoRepo.DotNetApi"
    / "ClientConfigurations"
    / "ClientCode.cs"
)
APPSETTINGS_FILE = (
    REPO_ROOT / "backend" / "SampleMonoRepo.DotNetApi" / "appsettings.json"
)

PROJECT_NAME = "sample-monorepo-production"
API_IMAGE = "sample-monorepo-api"
FRONTEND_IMAGE = "sample-monorepo-frontend"
PORT_BASE = 30000
PORT_BLOCK = 10
FRONTEND_CONTAINER_PORT = 20000
BACKEND_CONTAINER_PORT = 21000
CLIENT_CONFIG_KEY = "ClientConfig"


@dataclass(frozen=True)
class ClientDeployment:
    client_code: str
    frontend_port: int
    backend_port: int

    @property
    def slug(self) -> str:
        return self.client_code.lower()


def parse_action() -> str:
    if len(sys.argv) == 1:
        return "up"
    if len(sys.argv) == 2 and sys.argv[1] in {"up", "down"}:
        return sys.argv[1]
    print(f"Usage: {Path(__file__).name} [up|down]", file=sys.stderr)
    raise SystemExit(2)


def compose_command() -> list[str]:
    return [
        "docker",
        "compose",
        "--project-directory",
        str(REPO_ROOT),
        "-f",
        str(COMPOSE_FILE),
        "-p",
        PROJECT_NAME,
    ]


def dev_compose_command() -> list[str]:
    return [
        "docker",
        "compose",
        "--project-directory",
        str(REPO_ROOT),
        "-f",
        str(DEV_COMPOSE_FILE),
    ]


def run(command: list[str]) -> int:
    completed = subprocess.run(command, cwd=REPO_ROOT)
    return completed.returncode


def require_docker() -> None:
    if shutil.which("docker") is None:
        print("Docker is required to deploy the containers.", file=sys.stderr)
        raise SystemExit(1)

    compose = subprocess.run(
        ["docker", "compose", "version"],
        cwd=REPO_ROOT,
        capture_output=True,
        text=True,
        check=False,
    )
    if compose.returncode != 0:
        message = compose.stderr.strip() or "Docker Compose is required to deploy the containers."
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


def load_client_codes() -> list[str]:
    text = CLIENT_CODE_FILE.read_text(encoding="utf-8")
    match = re.search(r"enum\s+ClientCode\s*\{([^}]*)\}", text, re.S)
    if match is None:
        print(f"Could not find the ClientCode enum in {CLIENT_CODE_FILE}.", file=sys.stderr)
        raise SystemExit(1)

    body = re.sub(r"/\*.*?\*/", "", match.group(1), flags=re.S)
    body = re.sub(r"//.*?$", "", body, flags=re.M)
    client_codes = re.findall(r"\b([A-Za-z_][A-Za-z0-9_]*)\b", body)
    if not client_codes:
        print(f"The ClientCode enum in {CLIENT_CODE_FILE} has no members.", file=sys.stderr)
        raise SystemExit(1)
    return client_codes


def plan_deployments(client_codes: list[str]) -> list[ClientDeployment]:
    deployments: list[ClientDeployment] = []
    for index, client_code in enumerate(client_codes, start=1):
        frontend_port = PORT_BASE + index * PORT_BLOCK
        deployments.append(
            ClientDeployment(
                client_code=client_code,
                frontend_port=frontend_port,
                backend_port=frontend_port + 1,
            )
        )
    return deployments


def write_appsettings(deployment: ClientDeployment) -> None:
    settings = json.loads(APPSETTINGS_FILE.read_text(encoding="utf-8"))
    settings[CLIENT_CONFIG_KEY] = deployment.client_code
    destination = DEPLOY_DIR / deployment.client_code / "appsettings.json"
    destination.parent.mkdir(parents=True, exist_ok=True)
    destination.write_text(json.dumps(settings, indent=2) + "\n", encoding="utf-8")


def render_api_service(deployment: ClientDeployment) -> list[str]:
    slug = deployment.slug
    appsettings = f"./.simulate-production/{deployment.client_code}/appsettings.json"
    return [
        f"  {slug}-api:",
        f"    image: {API_IMAGE}",
        f"    container_name: {PROJECT_NAME}-{slug}-api",
        "    ports:",
        f'      - "{deployment.backend_port}:{BACKEND_CONTAINER_PORT}"',
        "    environment:",
        "      ASPNETCORE_ENVIRONMENT: Production",
        f"      ASPNETCORE_URLS: http://0.0.0.0:{BACKEND_CONTAINER_PORT}",
        "    volumes:",
        "      - ./backend:/src",
        f"      - {appsettings}:/src/SampleMonoRepo.DotNetApi/appsettings.json:ro",
        f"      - {slug}_api_bin:/src/SampleMonoRepo.DotNetApi/bin",
        f"      - {slug}_api_obj:/src/SampleMonoRepo.DotNetApi/obj",
        "    healthcheck:",
        f'      test: ["CMD", "bash", "-c", "echo > /dev/tcp/127.0.0.1/{BACKEND_CONTAINER_PORT}"]',
        "      interval: 5s",
        "      timeout: 5s",
        "      retries: 60",
        "      start_period: 300s",
        "    init: true",
    ]


def render_frontend_service(deployment: ClientDeployment) -> list[str]:
    slug = deployment.slug
    return [
        f"  {slug}-frontend:",
        f"    image: {FRONTEND_IMAGE}",
        f"    container_name: {PROJECT_NAME}-{slug}-frontend",
        "    ports:",
        f'      - "{deployment.frontend_port}:{FRONTEND_CONTAINER_PORT}"',
        "    environment:",
        f"      API_PROXY_TARGET: http://{slug}-api:{BACKEND_CONTAINER_PORT}",
        "    volumes:",
        "      - ./frontend:/app",
        f"      - {slug}_frontend_node_modules:/app/node_modules",
        f"      - {slug}_frontend_angular_cache:/app/.angular",
        "    depends_on:",
        f"      {slug}-api:",
        "        condition: service_healthy",
        "    healthcheck:",
        f'      test: ["CMD", "bash", "-c", "echo > /dev/tcp/127.0.0.1/{FRONTEND_CONTAINER_PORT}"]',
        "      interval: 10s",
        "      timeout: 5s",
        "      retries: 30",
        "      start_period: 480s",
        "    init: true",
    ]


def render_compose(deployments: list[ClientDeployment]) -> str:
    lines = [
        f"name: {PROJECT_NAME}",
        "",
        "services:",
    ]
    for deployment in deployments:
        lines.extend(render_api_service(deployment))
        lines.append("")
        lines.extend(render_frontend_service(deployment))
        lines.append("")

    lines.append("volumes:")
    for deployment in deployments:
        slug = deployment.slug
        lines.extend(
            [
                f"  {slug}_api_bin:",
                f"  {slug}_api_obj:",
                f"  {slug}_frontend_node_modules:",
                f"  {slug}_frontend_angular_cache:",
            ]
        )
    lines.append("")
    return "\n".join(lines)


def write_deployment_files(deployments: list[ClientDeployment]) -> None:
    DEPLOY_DIR.mkdir(parents=True, exist_ok=True)
    for deployment in deployments:
        write_appsettings(deployment)
    COMPOSE_FILE.write_text(render_compose(deployments), encoding="utf-8")


def print_plan(deployments: list[ClientDeployment]) -> None:
    print(
        f"Deploying {len(deployments)} clients ({len(deployments) * 2} containers).",
        flush=True,
    )
    for deployment in deployments:
        print(f"{deployment.client_code}", flush=True)
        print(f"  Frontend: http://localhost:{deployment.frontend_port}", flush=True)
        print(f"  Backend:  http://localhost:{deployment.backend_port}", flush=True)
        print(f"  {CLIENT_CONFIG_KEY}: {deployment.client_code}", flush=True)


def published_ports(deployments: list[ClientDeployment]) -> tuple[int, ...]:
    ports: list[int] = []
    for deployment in deployments:
        ports.append(deployment.frontend_port)
        ports.append(deployment.backend_port)
    return tuple(ports)


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

    print("Stopping containers already using the deployment ports.", flush=True)
    if run(["docker", "rm", "-f", *unique]) != 0:
        raise SystemExit(1)


def bring_down() -> None:
    if run([*compose_command(), "down", "--remove-orphans"]) != 0:
        raise SystemExit(1)


def main() -> None:
    action = parse_action()
    require_docker()
    deployments = plan_deployments(load_client_codes())
    write_deployment_files(deployments)

    if action == "down":
        bring_down()
        print("Stopped the simulated production containers.", flush=True)
        return

    print_plan(deployments)
    bring_down()
    stop_containers_on_ports(published_ports(deployments))

    if run([*dev_compose_command(), "build"]) != 0:
        raise SystemExit(1)

    try:
        exit_code = run(
            [
                *compose_command(),
                "up",
                "-d",
                "--wait",
                "--wait-timeout",
                "900",
                "--no-build",
                "--remove-orphans",
            ]
        )
    except KeyboardInterrupt:
        print("\nStopping the simulated production containers.", flush=True)
        bring_down()
        raise SystemExit(130)

    if exit_code != 0:
        print(
            f"Deployment failed. Stop any leftover containers with: {sys.executable} {Path(__file__)} down",
            file=sys.stderr,
        )
        raise SystemExit(exit_code)

    print("Simulated production containers are up.", flush=True)
    print_plan(deployments)
    print(f"Stop them with: {sys.executable} {Path(__file__)} down", flush=True)


if __name__ == "__main__":
    main()
