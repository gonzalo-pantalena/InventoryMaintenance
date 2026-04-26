# Creates the InventoryMaintenance database on the default local SQL Server Express
# instance (name SQLEXPRESS) if it does not exist, using Windows authentication.
# Requires: SQL Server Express (or a named instance) installed and "sqlcmd" in PATH
# (usually under "C:\Program Files\Microsoft SQL Server\Client SDK\...") or run from
# a Developer / SSMS "Developer PowerShell" where sqlcmd is on PATH.
#
# If your instance has another name, change the -S argument (e.g. "localhost\MyInstance").

$ErrorActionPreference = "Stop"
$server = "localhost\SQLEXPRESS"
$database = "InventoryMaintenance"

$exe = "sqlcmd"
if (-not (Get-Command $exe -ErrorAction SilentlyContinue)) {
  Write-Error "sqlcmd was not found. Install the SQL Server Command Line Utilities or add sqlcmd to PATH, or create the database manually in SSMS."
  exit 1
}

$check = "SET NOCOUNT ON; IF DB_ID(N'$database') IS NULL CREATE DATABASE [$database] COLLATE SQL_Latin1_General_CP1_CI_AS; ELSE PRINT 'Database already exists.';"

& sqlcmd -S $server -E -b -C -Q $check
if ($LASTEXITCODE -ne 0) {
  exit $LASTEXITCODE
}
Write-Host "OK: $database is available on $server" -ForegroundColor Green
