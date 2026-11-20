#!/usr/bin/env python3
"""Start one SQL Server container per database and apply the EF migrations.

Host ports follow the client blocks used by simulate_production_deploy.py.
Each client owns a block of 10 ports: the frontend ends in 0, the backend ends
in 1, and that client's project database ends in 3. The shared Boss master
database uses 30003, the same ending in the base block ahead of the clients.

Migrations run as sa and insert the code-first seed data. Customer logins are
created afterwards with idempotent T-SQL. An EF migration is a poor place for
those logins: CREATE LOGIN is server-scoped and has to run as sa, outside the
application user's database. Run this script with "down" to stop the containers
without deleting their data volumes.
"""

import os
import re
import shutil
import subprocess
import sys
import time
from dataclasses import dataclass
from pathlib import Path


REPO_ROOT = Path(__file__).resolve().parent.parent
DATABASES_ROOT = REPO_ROOT / "databases"
COMPOSE_FILE = DATABASES_ROOT / "docker-compose.yml"
CLIENT_CODE_FILE = (
    REPO_ROOT
    / "backend"
    / "SampleMonoRepo.DotNetApi"
    / "ClientConfigurations"
    / "ClientCode.cs"
)
CLIENT_CONFIGURATION_DIR = CLIENT_CODE_FILE.parent

PROJECT_NAME = "sample-monorepo-databases"
SQL_IMAGE = "mcr.microsoft.com/mssql/server:2022-latest"
SA_PASSWORD = "Dev-SqlServer-Sa-1!"
PORT_BASE = 30000
PORT_BLOCK = 10
DATABASE_PORT_OFFSET = 3
BOSS_MASTER_PORT = PORT_BASE + DATABASE_PORT_OFFSET
BOSS_DATABASE_NAME = "BossMaster"
SQL_READY_TIMEOUT_SECONDS = 300
CONNECTION_STRING_PATTERN = re.compile(r'"Server=[^"]*"')
SQLCMD_CANDIDATES = (
    "/opt/mssql-tools18/bin/sqlcmd",
    "/opt/mssql-tools/bin/sqlcmd",
)


@dataclass(frozen=True)
class SqlDatabase:
    service: str
    container_name: str
    host_port: int
    database_name: str
    project_name: str
    context_name: str
    volume_name: str

    @property
    def project_directory(self) -> Path:
        return DATABASES_ROOT / self.project_name

    @property
    def project_file(self) -> Path:
        return self.project_directory / f"{self.project_name}.csproj"


@dataclass(frozen=True)
class CustomerDatabaseLogin:
    client_code: str
    project_database: SqlDatabase

    @property
    def slug(self) -> str:
        return self.client_code.lower()

    @property
    def project_user(self) -> str:
        return f"{self.slug}-project-database-user"

    @property
    def project_password(self) -> str:
        return f"{self.client_code}-Project-Rw-1!"

    @property
    def boss_user(self) -> str:
        return f"boss-master-database-{self.slug}-user"

    @property
    def boss_password(self) -> str:
        return f"{self.client_code}-Boss-Ro-1!"


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
        str(DATABASES_ROOT),
        "-f",
        str(COMPOSE_FILE),
        "-p",
        PROJECT_NAME,
    ]


def run(command: list[str], env: dict[str, str] | None = None) -> int:
    completed = subprocess.run(command, cwd=REPO_ROOT, env=env)
    return completed.returncode


def require_docker() -> None:
    if shutil.which("docker") is None:
        print("Docker is required to start the databases.", file=sys.stderr)
        raise SystemExit(1)

    compose = subprocess.run(
        ["docker", "compose", "version"],
        cwd=REPO_ROOT,
        capture_output=True,
        text=True,
        check=False,
    )
    if compose.returncode != 0:
        message = compose.stderr.strip() or "Docker Compose is required to start the databases."
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


def boss_database() -> SqlDatabase:
    return SqlDatabase(
        service="boss-master",
        container_name="sample-monorepo-boss-master-db",
        host_port=BOSS_MASTER_PORT,
        database_name=BOSS_DATABASE_NAME,
        project_name="BossMasterDatabase",
        context_name="BossMasterDbContext",
        volume_name="boss_master_data",
    )


def plan_customers(client_codes: list[str]) -> list[CustomerDatabaseLogin]:
    customers: list[CustomerDatabaseLogin] = []
    for index, client_code in enumerate(client_codes, start=1):
        host_port = PORT_BASE + index * PORT_BLOCK + DATABASE_PORT_OFFSET
        slug = client_code.lower()
        database = SqlDatabase(
            service=f"{slug}-project",
            container_name=f"sample-monorepo-{slug}-project-db",
            host_port=host_port,
            database_name=f"{client_code}Project",
            project_name=f"{client_code}ProjectDatabase",
            context_name=f"{client_code}ProjectDbContext",
            volume_name=f"{slug}_project_data",
        )
        customers.append(CustomerDatabaseLogin(client_code=client_code, project_database=database))
    return customers


def connection_string(port: int, database: str, user: str, password: str) -> str:
    return (
        f"Server=localhost,{port};"
        f"Database={database};"
        f"User Id={user};"
        f"Password={password};"
        "TrustServerCertificate=True;"
    )


def render_service(database: SqlDatabase) -> list[str]:
    healthcheck = (
        "if [ -x /opt/mssql-tools18/bin/sqlcmd ]; then "
        "SQLCMD=/opt/mssql-tools18/bin/sqlcmd; "
        "else SQLCMD=/opt/mssql-tools/bin/sqlcmd; fi; "
        "$$SQLCMD -S localhost -U sa -P $$MSSQL_SA_PASSWORD -C -b -Q 'SELECT 1' || exit 1"
    )
    return [
        f"  {database.service}:",
        f"    image: {SQL_IMAGE}",
        f"    container_name: {database.container_name}",
        "    environment:",
        '      ACCEPT_EULA: "Y"',
        "      MSSQL_PID: Developer",
        f'      MSSQL_SA_PASSWORD: "{SA_PASSWORD}"',
        "    ports:",
        f'      - "{database.host_port}:1433"',
        "    volumes:",
        f"      - {database.volume_name}:/var/opt/mssql",
        "    mem_limit: 1g",
        "    shm_size: 512mb",
        "    healthcheck:",
        f'      test: ["CMD-SHELL", "{healthcheck}"]',
        "      interval: 10s",
        "      timeout: 10s",
        "      retries: 30",
        "      start_period: 60s",
        "    init: true",
    ]


def render_compose(databases: list[SqlDatabase]) -> str:
    lines = [
        f"name: {PROJECT_NAME}",
        "",
        "services:",
    ]
    for database in databases:
        lines.extend(render_service(database))
        lines.append("")
    lines.append("volumes:")
    for database in databases:
        lines.append(f"  {database.volume_name}:")
    lines.append("")
    return "\n".join(lines)


def write_compose(databases: list[SqlDatabase]) -> None:
    DATABASES_ROOT.mkdir(parents=True, exist_ok=True)
    COMPOSE_FILE.write_text(render_compose(databases), encoding="utf-8")


def published_ports(databases: list[SqlDatabase]) -> tuple[int, ...]:
    return tuple(database.host_port for database in databases)


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

    print("Stopping containers already using the database ports.", flush=True)
    if run(["docker", "rm", "-f", *unique]) != 0:
        raise SystemExit(1)


def bring_up() -> None:
    if run([*compose_command(), "up", "-d", "--remove-orphans"]) != 0:
        raise SystemExit(1)


def bring_down() -> None:
    if not COMPOSE_FILE.exists():
        print("No database compose file exists, so there is nothing to stop.", flush=True)
        return
    if run([*compose_command(), "down", "--remove-orphans"]) != 0:
        raise SystemExit(1)


def sql_command(
    container_name: str,
    sqlcmd: str,
    database_name: str,
    statement: str,
    user: str,
    password: str,
    trust_certificate: bool,
) -> subprocess.CompletedProcess[str]:
    command = [
        "docker",
        "exec",
        container_name,
        sqlcmd,
        "-S",
        "localhost",
        "-U",
        user,
        "-P",
        password,
        "-b",
        "-h-1",
        "-W",
        "-d",
        database_name,
        "-Q",
        statement,
    ]
    if trust_certificate:
        command.insert(command.index("-b"), "-C")
    return subprocess.run(
        command,
        cwd=REPO_ROOT,
        capture_output=True,
        text=True,
        check=False,
    )


def execute_sql(
    container_name: str,
    sqlcmd: str,
    database_name: str,
    statement: str,
    user: str = "sa",
    password: str = SA_PASSWORD,
) -> str:
    completed = sql_command(
        container_name,
        sqlcmd,
        database_name,
        statement,
        user,
        password,
        trust_certificate=True,
    )
    if completed.returncode != 0 and "Unknown option" in f"{completed.stderr} {completed.stdout}":
        completed = sql_command(
            container_name,
            sqlcmd,
            database_name,
            statement,
            user,
            password,
            trust_certificate=False,
        )
    if completed.returncode != 0:
        detail = (completed.stderr or completed.stdout).strip()
        print(
            f"SQL failed on {container_name}.{database_name}: {detail}",
            file=sys.stderr,
        )
        raise SystemExit(completed.returncode or 1)
    return completed.stdout


def wait_for_sql(database: SqlDatabase) -> str:
    deadline = time.time() + SQL_READY_TIMEOUT_SECONDS
    last_detail = "SQL Server did not accept connections."
    print(f"Waiting for {database.container_name} on localhost:{database.host_port}.", flush=True)
    while time.time() < deadline:
        for candidate in SQLCMD_CANDIDATES:
            probe = subprocess.run(
                ["docker", "exec", database.container_name, "test", "-x", candidate],
                cwd=REPO_ROOT,
                stdout=subprocess.DEVNULL,
                stderr=subprocess.DEVNULL,
                check=False,
            )
            if probe.returncode != 0:
                continue
            for trust_certificate in (True, False):
                completed = sql_command(
                    database.container_name,
                    candidate,
                    "master",
                    "SET NOCOUNT ON; SELECT 1;",
                    "sa",
                    SA_PASSWORD,
                    trust_certificate,
                )
                if completed.returncode == 0:
                    return candidate
                last_detail = (completed.stderr or completed.stdout).strip() or last_detail
        time.sleep(3)

    print(f"{database.container_name} was not ready: {last_detail}", file=sys.stderr)
    raise SystemExit(1)


def apply_migrations(database: SqlDatabase) -> None:
    connection = connection_string(database.host_port, database.database_name, "sa", SA_PASSWORD)
    print(f"Applying migrations for {database.database_name}.", flush=True)
    env = os.environ.copy()
    env["EF_CONNECTION"] = connection
    exit_code = run(
        [
            "dotnet",
            "ef",
            "database",
            "update",
            "--project",
            str(database.project_file),
            "--context",
            database.context_name,
            "--connection",
            connection,
        ],
        env=env,
    )
    if exit_code != 0:
        raise SystemExit(exit_code)


def sql_identifier(name: str) -> str:
    return name.replace("]", "]]")


def sql_literal(value: str) -> str:
    return value.replace("'", "''")


def ensure_login(database: SqlDatabase, sqlcmd: str, login: str, password: str) -> None:
    identifier = sql_identifier(login)
    literal = sql_literal(login)
    password_literal = sql_literal(password)
    execute_sql(
        database.container_name,
        sqlcmd,
        "master",
        (
            f"IF NOT EXISTS (SELECT 1 FROM sys.sql_logins WHERE name = N'{literal}') "
            f"EXEC(N'CREATE LOGIN [{identifier}] WITH PASSWORD = N''{password_literal}'', "
            "CHECK_POLICY = ON, CHECK_EXPIRATION = OFF');"
        ),
    )
    execute_sql(
        database.container_name,
        sqlcmd,
        "master",
        (
            f"ALTER LOGIN [{identifier}] WITH PASSWORD = N'{password_literal}', "
            f"CHECK_POLICY = ON, CHECK_EXPIRATION = OFF, "
            f"DEFAULT_DATABASE = [{sql_identifier(database.database_name)}];"
        ),
    )
    execute_sql(
        database.container_name,
        sqlcmd,
        database.database_name,
        (
            f"IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'{literal}') "
            f"EXEC(N'CREATE USER [{identifier}] FOR LOGIN [{identifier}]');"
        ),
    )


def ensure_role(database: SqlDatabase, sqlcmd: str, login: str, role: str) -> None:
    identifier = sql_identifier(login)
    literal = sql_literal(login)
    execute_sql(
        database.container_name,
        sqlcmd,
        database.database_name,
        (
            f"IF IS_ROLEMEMBER(N'{role}', N'{literal}') <> 1 "
            f"ALTER ROLE {role} ADD MEMBER [{identifier}];"
        ),
    )


def scalar(
    database: SqlDatabase,
    sqlcmd: str,
    query: str,
    user: str = "sa",
    password: str = SA_PASSWORD,
) -> str:
    output = execute_sql(
        database.container_name,
        sqlcmd,
        database.database_name,
        f"SET NOCOUNT ON; {query}",
        user=user,
        password=password,
    )
    numbers = re.findall(r"-?\d+", output)
    if not numbers:
        print(
            f"Expected a number from {database.database_name} and got: {output.strip()}",
            file=sys.stderr,
        )
        raise SystemExit(1)
    return numbers[-1]


def require_count(database: SqlDatabase, sqlcmd: str, table: str, expected: int) -> None:
    actual = int(scalar(database, sqlcmd, f"SELECT COUNT(*) FROM [{sql_identifier(table)}];"))
    if actual != expected:
        print(
            f"{database.database_name}.{table} has {actual} rows; expected {expected}.",
            file=sys.stderr,
        )
        raise SystemExit(1)
    print(f"{database.database_name}.{table}: {actual} rows.", flush=True)


def ensure_customer_access(
    boss: SqlDatabase,
    boss_sqlcmd: str,
    customer: CustomerDatabaseLogin,
    project_sqlcmd: str,
) -> None:
    project = customer.project_database
    print(f"Ensuring logins for {customer.client_code}.", flush=True)
    ensure_login(project, project_sqlcmd, customer.project_user, customer.project_password)
    ensure_role(project, project_sqlcmd, customer.project_user, "db_datareader")
    ensure_role(project, project_sqlcmd, customer.project_user, "db_datawriter")
    ensure_login(boss, boss_sqlcmd, customer.boss_user, customer.boss_password)
    ensure_role(boss, boss_sqlcmd, customer.boss_user, "db_datareader")

    project_count = scalar(
        project,
        project_sqlcmd,
        "SELECT COUNT(*) FROM [DevicesForThisCustomer];",
        user=customer.project_user,
        password=customer.project_password,
    )
    boss_count = scalar(
        boss,
        boss_sqlcmd,
        "SELECT COUNT(*) FROM [AllSignumDevicesAcrossAllCustomers];",
        user=customer.boss_user,
        password=customer.boss_password,
    )
    if project_count != "2" or boss_count != "6":
        print(
            f"{customer.client_code} login check failed. "
            f"Project rows={project_count}, boss rows={boss_count}.",
            file=sys.stderr,
        )
        raise SystemExit(1)


def update_client_configuration(customer: CustomerDatabaseLogin, boss: SqlDatabase) -> None:
    path = CLIENT_CONFIGURATION_DIR / f"{customer.client_code}ClientConfiguration.cs"
    text = path.read_text(encoding="utf-8")
    matches = list(CONNECTION_STRING_PATTERN.finditer(text))
    if len(matches) != 2:
        print(f"Expected 2 connection strings in {path}.", file=sys.stderr)
        raise SystemExit(1)

    project = customer.project_database
    project_connection = connection_string(
        project.host_port,
        project.database_name,
        customer.project_user,
        customer.project_password,
    )
    master_connection = connection_string(
        boss.host_port,
        boss.database_name,
        customer.boss_user,
        customer.boss_password,
    )
    updated = text
    for match, value in zip(reversed(matches), (master_connection, project_connection), strict=True):
        updated = updated[: match.start()] + f'"{value}"' + updated[match.end() :]

    if updated == text:
        print(f"Connection strings already current in {path.name}.", flush=True)
        return

    path.write_text(updated, encoding="utf-8")
    print(f"Updated {path.relative_to(REPO_ROOT)}.", flush=True)


def print_plan(boss: SqlDatabase, customers: list[CustomerDatabaseLogin]) -> None:
    print(f"Boss master: localhost:{boss.host_port} database {boss.database_name}", flush=True)
    for customer in customers:
        project = customer.project_database
        print(
            f"{customer.client_code}: localhost:{project.host_port} database {project.database_name}",
            flush=True,
        )
        print(f"  {customer.project_user} (read/write)", flush=True)
        print(f"  {customer.boss_user} (read)", flush=True)


def main() -> None:
    action = parse_action()
    require_docker()
    boss = boss_database()
    customers = plan_customers(load_client_codes())
    databases = [boss, *[customer.project_database for customer in customers]]
    for database in databases:
        if not database.project_file.exists():
            print(f"Missing EF project {database.project_file}.", file=sys.stderr)
            raise SystemExit(1)

    write_compose(databases)
    if action == "down":
        bring_down()
        print("Stopped the database containers.", flush=True)
        return

    print_plan(boss, customers)
    stop_containers_on_ports(published_ports(databases))
    bring_up()

    sqlcmds = {database.container_name: wait_for_sql(database) for database in databases}
    for database in databases:
        apply_migrations(database)

    boss_sqlcmd = sqlcmds[boss.container_name]
    require_count(boss, boss_sqlcmd, "AllSignumDevicesAcrossAllCustomers", 6)
    for customer in customers:
        project = customer.project_database
        project_sqlcmd = sqlcmds[project.container_name]
        require_count(project, project_sqlcmd, "DevicesForThisCustomer", 2)
        require_count(project, project_sqlcmd, "MachineTypesForThisCustomer", 2)
        require_count(project, project_sqlcmd, "MachineConfigsThisCustomerSupports", 2)
        ensure_customer_access(boss, boss_sqlcmd, customer, project_sqlcmd)
        update_client_configuration(customer, boss)

    print("Database containers are up, migrated, and seeded.", flush=True)
    print(f"Stop them with: {sys.executable} {Path(__file__)} down", flush=True)


if __name__ == "__main__":
    main()
