#!/usr/bin/env bash
set -euo pipefail

CONTAINER="gestorvendas_sqlserver"
USER="sa"
PASSWORD="${SA_PASSWORD:-SenhaForte123@@}"
DATABASE="${DATABASE:-MeuBanco}"
SQLCMD_IN_CONTAINER="/opt/mssql-tools18/bin/sqlcmd"
MSSQL_TOOLS_IMAGE="mcr.microsoft.com/mssql-tools:latest"

print_usage() {
  cat <<EOF
Usage: db.sh -a <sql-file> | -q <sql-query>

Options:
  -a <file>       Execute SQL file (local path). File content is streamed into the container.
  -q <query>      Execute single SQL query string.
  -h              Show this help.

Examples:
  ./scripts/db.sh -a src/sql/V1__CREATE_TABLES.sql
  ./scripts/db.sh -q "SELECT COUNT(*) FROM clientes"
EOF
}

ensure_container_running() {
  if ! docker ps --format '{{.Names}}' | grep -q "^${CONTAINER}$"; then
    echo "Container ${CONTAINER} is not running. Start it with: docker-compose up -d sqlserver" >&2
    exit 1
  fi
}

main() {
  if [ $# -eq 0 ]; then
    print_usage
    exit 0
  fi

  local file=""
  local query=""

  while getopts ":a:q:h" opt; do
    case $opt in
      a) file="$OPTARG" ;;
      q) query="$OPTARG" ;;
      h) print_usage; exit 0 ;;
      \?) echo "Invalid option: -$OPTARG" >&2; print_usage; exit 1 ;;
      :) echo "Option -$OPTARG requires an argument." >&2; exit 1 ;;
    esac
  done

  ensure_container_running

  # prefer sqlcmd inside container
  if docker exec "$CONTAINER" test -x "$SQLCMD_IN_CONTAINER" >/dev/null 2>&1; then
    SQLCMD_METHOD="exec"
  else
    SQLCMD_METHOD="tools_image"
  fi

  if [ -n "$file" ]; then
    if [ ! -f "$file" ]; then
      echo "SQL file not found: $file" >&2
      exit 1
    fi
    echo "Executing file: $file"
    if [ "$SQLCMD_METHOD" = "exec" ]; then
      docker exec -i "$CONTAINER" "$SQLCMD_IN_CONTAINER" -S localhost -U "$USER" -P "$PASSWORD" -d "$DATABASE" -i < "$file"
    else
      docker run --rm -i --network container:"$CONTAINER" "$MSSQL_TOOLS_IMAGE" sqlcmd -S localhost -U "$USER" -P "$PASSWORD" -d "$DATABASE" -i < "$file"
    fi
  elif [ -n "$query" ]; then
    echo "Executing query..."
    if [ "$SQLCMD_METHOD" = "exec" ]; then
      docker exec -i "$CONTAINER" "$SQLCMD_IN_CONTAINER" -S localhost -U "$USER" -P "$PASSWORD" -d "$DATABASE" -Q "$query"
    else
      docker run --rm --network container:"$CONTAINER" "$MSSQL_TOOLS_IMAGE" sqlcmd -S localhost -U "$USER" -P "$PASSWORD" -d "$DATABASE" -Q "$query"
    fi
  else
    print_usage
    exit 0
  fi
}

main "$@"
