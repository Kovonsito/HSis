#Requires -Version 5.1
<#
Genera un respaldo completo verificado de la base de datos de HSis con autenticación integrada de Windows.
Ejecutarlo antes de cambios de esquema, de RehashearContraseñasAsync o de cualquier operación irreversible.
No borra respaldos anteriores. El directorio debe ser escribible por la cuenta del servicio de SQL Server.
#>
param(
	[Parameter(Mandatory)][string]$ServerInstance,
	[Parameter(Mandatory)][string]$Database,
	[Parameter(Mandatory)][string]$BackupDirectory
)

$ErrorActionPreference = 'Stop'
if ($Database -notmatch '^[A-Za-z0-9_.\-]+$') { throw 'Nombre de base de datos no válido.' }
if ($ServerInstance -notmatch '^[A-Za-z0-9_.\-\\,]+$') { throw 'Instancia de SQL Server no válida.' }
if (-not (Get-Command sqlcmd -ErrorAction SilentlyContinue)) { throw 'No se encontró sqlcmd en el PATH.' }
if (-not (Test-Path $BackupDirectory -PathType Container)) { throw "No existe el directorio $BackupDirectory." }

$archivo = Join-Path $BackupDirectory ("{0}_{1:yyyyMMdd_HHmmss}.bak" -f $Database, (Get-Date))
$escapado = $archivo.Replace("'", "''")
$consulta = "BACKUP DATABASE [$Database] TO DISK = N'$escapado' WITH INIT, CHECKSUM, COMPRESSION; " +
	"RESTORE VERIFYONLY FROM DISK = N'$escapado' WITH CHECKSUM;"

sqlcmd -S $ServerInstance -E -b -Q $consulta
if ($LASTEXITCODE -ne 0) { throw "Falló el respaldo de $Database (sqlcmd devolvió $LASTEXITCODE)." }
Write-Host "Respaldo verificado: $archivo"