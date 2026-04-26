#!/usr/bin/env bash
set -e

HOST=${MSSQL_HOST:-mssql}
USER_NAME=${MSSQL_USER:-sa}
PASSWORD=${MSSQL_SA_PASSWORD}

if [ -z "$PASSWORD" ]; then
  echo "MSSQL_SA_PASSWORD is not set" >&2
  exit 1
fi

echo "Waiting for SQL Server at $HOST:1433 ..."
for _ in $(seq 1 90); do
  if sqlcmd -S "$HOST" -U "$USER_NAME" -P "$PASSWORD" -C -Q "SET NOCOUNT ON; SELECT 1" > /dev/null 2>&1; then
    break
  fi
  sleep 2
done

if ! sqlcmd -S "$HOST" -U "$USER_NAME" -P "$PASSWORD" -C -Q "SET NOCOUNT ON; SELECT 1" > /dev/null 2>&1; then
  echo "Could not connect to $HOST after wait." >&2
  exit 1
fi

if [ -f /init/01-create-database.sql ]; then
  echo "Applying /init/01-create-database.sql ..."
  sqlcmd -S "$HOST" -U "$USER_NAME" -P "$PASSWORD" -C -b -d master -i /init/01-create-database.sql
fi

echo "Database init finished."
exit 0
