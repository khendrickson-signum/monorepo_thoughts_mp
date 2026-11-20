#!/usr/bin/env python3
"""Write and open an HTML page of the running Docker containers.

Containers are grouped by client configuration. Angular frontend ports are
links. Database containers include a JDBC connection string for that client.
"""

import html
import json
import re
import shutil
import subprocess
import sys
import webbrowser
from dataclasses import dataclass
from datetime import datetime, timezone
from pathlib import Path


REPO_ROOT = Path(__file__).resolve().parent.parent
CLIENT_CONFIGURATION_DIR = (
    REPO_ROOT
    / "backend"
    / "SampleMonoRepo.DotNetApi"
    / "ClientConfigurations"
)
CLIENT_CODE_FILE = CLIENT_CONFIGURATION_DIR / "ClientCode.cs"
APPSETTINGS_FILE = REPO_ROOT / "backend" / "SampleMonoRepo.DotNetApi" / "appsettings.json"
STATUS_PAGE = REPO_ROOT / "running-services.html"
CONNECTION_STRING_PATTERN = re.compile(r'"Server=[^"]*"')
OTHER_GROUP = "Other"


@dataclass(frozen=True)
class ClientServices:
    name: str
    color: str
    project_connection: str
    master_connection: str

    @property
    def slug(self) -> str:
        return self.name.lower()


@dataclass(frozen=True)
class RunningContainer:
    name: str
    ports: str
    project: str
    service: str

    @property
    def host_port(self) -> int | None:
        match = re.search(r"(?:0\.0\.0\.0|\[::\]|127\.0\.0\.1):(\d+)->", self.ports)
        if match is None:
            return None
        return int(match.group(1))

    @property
    def is_frontend(self) -> bool:
        return "frontend" in self.service or "frontend" in self.name.lower()

    @property
    def is_boss_database(self) -> bool:
        text = f"{self.service} {self.name}".lower()
        return "boss-master" in text or "boss_master" in text

    @property
    def is_project_database(self) -> bool:
        text = f"{self.service} {self.name}".lower()
        return "project" in text and ("db" in text or "database" in text)


def client_code_order() -> list[str]:
    if not CLIENT_CODE_FILE.exists():
        return []
    text = CLIENT_CODE_FILE.read_text(encoding="utf-8")
    match = re.search(r"enum\s+ClientCode\s*\{([^}]*)\}", text, re.S)
    if match is None:
        return []
    body = re.sub(r"/\*.*?\*/", "", match.group(1), flags=re.S)
    body = re.sub(r"//.*?$", "", body, flags=re.M)
    return re.findall(r"\b([A-Za-z_][A-Za-z0-9_]*)\b", body)


def load_clients() -> list[ClientServices]:
    clients: list[ClientServices] = []
    paths = [
        path
        for path in CLIENT_CONFIGURATION_DIR.glob("*ClientConfiguration.cs")
        if path.name != "ClientConfiguration.cs"
    ]
    order = {name: index for index, name in enumerate(client_code_order())}
    paths.sort(key=lambda path: order.get(path.name.removesuffix("ClientConfiguration.cs"), len(order)))
    for path in paths:
        text = path.read_text(encoding="utf-8")
        connections = CONNECTION_STRING_PATTERN.findall(text)
        if len(connections) != 2:
            print(f"Expected 2 connection strings in {path.name}.", file=sys.stderr)
            raise SystemExit(1)
        color_match = re.search(r'"(#[0-9A-Fa-f]{6})"', text)
        clients.append(
            ClientServices(
                name=path.name.removesuffix("ClientConfiguration.cs"),
                color=color_match.group(1) if color_match else "#335C81",
                project_connection=connections[0].strip('"'),
                master_connection=connections[1].strip('"'),
            )
        )
    if not clients:
        print(f"No client configurations found in {CLIENT_CONFIGURATION_DIR}.", file=sys.stderr)
        raise SystemExit(1)
    return clients


def active_client_name(clients: list[ClientServices]) -> str:
    if not APPSETTINGS_FILE.exists():
        return clients[0].name
    settings = json.loads(APPSETTINGS_FILE.read_text(encoding="utf-8"))
    configured = str(settings.get("ClientConfig", "")).strip()
    for client in clients:
        if client.name.lower() == configured.lower():
            return client.name
    return clients[0].name


def load_running_containers() -> list[RunningContainer]:
    completed = subprocess.run(
        ["docker", "ps", "--format", "{{json .}}"],
        cwd=REPO_ROOT,
        capture_output=True,
        text=True,
        check=False,
    )
    if completed.returncode != 0:
        detail = completed.stderr.strip() or "docker ps failed."
        print(detail, file=sys.stderr)
        raise SystemExit(completed.returncode or 1)

    containers: list[RunningContainer] = []
    for line in completed.stdout.splitlines():
        if not line.strip():
            continue
        item = json.loads(line)
        labels = parse_labels(str(item.get("Labels", "")))
        containers.append(
            RunningContainer(
                name=str(item.get("Names", "")),
                ports=str(item.get("Ports", "")),
                project=labels.get("com.docker.compose.project", ""),
                service=labels.get("com.docker.compose.service", ""),
            )
        )
    return containers


def parse_labels(labels: str) -> dict[str, str]:
    parsed: dict[str, str] = {}
    for piece in labels.split(","):
        if "=" not in piece:
            continue
        key, value = piece.split("=", 1)
        parsed[key] = value
    return parsed


def to_jdbc(ado_connection: str) -> str:
    parts: dict[str, str] = {}
    for piece in ado_connection.split(";"):
        if "=" not in piece:
            continue
        key, value = piece.split("=", 1)
        parts[key.strip().lower()] = value.strip()

    server = parts.get("server", "localhost")
    if "," in server:
        host, port = server.split(",", 1)
        server = f"{host}:{port}"
    database = parts.get("database", "")
    user = parts.get("user id", parts.get("uid", ""))
    password = parts.get("password", parts.get("pwd", ""))
    return (
        f"jdbc:sqlserver://{server};databaseName={database};"
        f"user={user};password={password};encrypt=true;trustServerCertificate=true;"
    )


def client_for_container(container: RunningContainer, clients: list[ClientServices]) -> str | None:
    if container.is_boss_database:
        return None
    text = f"{container.name} {container.service}".lower()
    for client in clients:
        if client.slug in text:
            return client.name
    return None


def is_dev_stack(container: RunningContainer) -> bool:
    return container.project == "sample-monorepo" and container.service in {"api", "frontend"}


def containers_for_client(
    client: ClientServices,
    containers: list[RunningContainer],
    active_client: str,
) -> list[RunningContainer]:
    selected: list[RunningContainer] = []
    for container in sorted(containers, key=container_sort_key):
        owner = client_for_container(container, [client])
        if owner == client.name or container.is_boss_database:
            selected.append(container)
            continue
        if client.name == active_client and is_dev_stack(container):
            selected.append(container)
    return selected


def unassigned_containers(
    containers: list[RunningContainer],
    clients: list[ClientServices],
    active_client: str,
) -> list[RunningContainer]:
    assigned: set[str] = set()
    for client in clients:
        for container in containers_for_client(client, containers, active_client):
            assigned.add(container.name)
    return [container for container in containers if container.name not in assigned]


def container_sort_key(container: RunningContainer) -> tuple[int, str]:
    if container.is_frontend:
        rank = 0
    elif container.is_project_database:
        rank = 2
    elif container.is_boss_database:
        rank = 3
    else:
        rank = 1
    return (rank, container.name)


def jdbc_for_container(container: RunningContainer, client: ClientServices) -> str | None:
    if container.is_boss_database:
        return to_jdbc(client.master_connection)
    if container.is_project_database:
        return to_jdbc(client.project_connection)
    return None


def render_container(container: RunningContainer, client: ClientServices | None) -> str:
    ports = html.escape(container.ports or "no published ports")
    name = html.escape(container.name)
    if container.is_frontend and container.host_port is not None:
        url = f"http://localhost:{container.host_port}"
        title = f'<a href="{html.escape(url)}">{name}</a>'
    else:
        title = name

    jdbc = jdbc_for_container(container, client) if client is not None else None
    jdbc_html = ""
    if jdbc is not None:
        jdbc_html = f'<code class="jdbc">{html.escape(jdbc)}</code>'
    return (
        "<li>"
        f'<div class="container-name">{title}</div>'
        f'<div class="ports">{ports}</div>'
        f"{jdbc_html}"
        "</li>"
    )


def render_page(
    clients: list[ClientServices],
    containers: list[RunningContainer],
    active_client: str,
) -> str:
    generated = datetime.now(timezone.utc).strftime("%Y-%m-%d %H:%M:%S UTC")
    sections: list[str] = []
    for client in clients:
        grouped = containers_for_client(client, containers, active_client)
        items = "\n".join(render_container(container, client) for container in grouped)
        if not items:
            items = "<li>No running containers.</li>"
        sections.append(
            f"""
            <section style="border-color: {html.escape(client.color)}">
              <h2>{html.escape(client.name)}</h2>
              <ul>
                {items}
              </ul>
            </section>
            """
        )

    leftovers = unassigned_containers(containers, clients, active_client)
    if leftovers:
        items = "\n".join(render_container(container, None) for container in leftovers)
        sections.append(
            f"""
            <section>
              <h2>{OTHER_GROUP}</h2>
              <ul>
                {items}
              </ul>
            </section>
            """
        )

    return f"""<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8">
  <title>Running services</title>
  <style>
    body {{
      margin: 0;
      font-family: "Segoe UI", sans-serif;
      background: #f4f6f8;
      color: #1c2830;
    }}
    main {{
      max-width: 960px;
      margin: 0 auto;
      padding: 32px 20px 64px;
    }}
    h1 {{
      margin-bottom: 4px;
    }}
    .generated {{
      color: #5c6b76;
      margin-bottom: 28px;
    }}
    section {{
      background: white;
      border: 1px solid #d5dde3;
      border-left: 8px solid #335c81;
      border-radius: 8px;
      padding: 8px 20px 16px;
      margin-bottom: 20px;
    }}
    ul {{
      list-style: none;
      padding: 0;
      margin: 0;
    }}
    li {{
      padding: 12px 0;
      border-top: 1px solid #e6edf2;
    }}
    .container-name {{
      font-weight: 650;
    }}
    a {{
      color: #0b57d0;
    }}
    .ports {{
      color: #5c6b76;
      margin-top: 4px;
    }}
    code.jdbc {{
      display: block;
      margin-top: 8px;
      padding: 8px 10px;
      background: #f4f6f8;
      border-radius: 4px;
      overflow-wrap: anywhere;
    }}
  </style>
</head>
<body>
  <main>
    <h1>Running services</h1>
    <p class="generated">Generated {html.escape(generated)}. The API ClientConfig is {html.escape(active_client)}.</p>
    {"".join(sections)}
  </main>
</body>
</html>
"""


def open_in_browser(path: Path) -> None:
    print(f"Status page: {path}", flush=True)
    if shutil.which("wslpath") and shutil.which("explorer.exe"):
        converted = subprocess.run(
            ["wslpath", "-w", str(path)],
            capture_output=True,
            text=True,
            check=False,
        )
        if converted.returncode == 0:
            subprocess.Popen(
                ["explorer.exe", converted.stdout.strip()],
                stdin=subprocess.DEVNULL,
                stdout=subprocess.DEVNULL,
                stderr=subprocess.DEVNULL,
                start_new_session=True,
            )
            return

    if shutil.which("wslview"):
        subprocess.Popen(
            ["wslview", path.resolve().as_uri()],
            stdin=subprocess.DEVNULL,
            stdout=subprocess.DEVNULL,
            stderr=subprocess.DEVNULL,
            start_new_session=True,
        )
        return

    webbrowser.open(path.resolve().as_uri())


def main() -> None:
    clients = load_clients()
    containers = load_running_containers()
    active = active_client_name(clients)
    STATUS_PAGE.write_text(render_page(clients, containers, active), encoding="utf-8")
    open_in_browser(STATUS_PAGE)


if __name__ == "__main__":
    main()
