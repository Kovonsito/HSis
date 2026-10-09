#Requires -Version 5.1
<#
Genera un respaldo completo verificado de la base de datos de HSis con autenticación integrada de Windows.
Ejecutarlo antes de cambios de esquema, de RehashearContraseñasAsync o de cualquier operación irreversible.
Usa COPY_ONLY para no alterar la cadena de respaldos programados del servidor.
-Compress no es compatible con SQL Server Express.
-TrustServerCertificate solo debe usarse en instancias con certificado autofirmado de una red de confianza.
No borra respaldos anteriores. El directorio debe ser escribible por la cuenta del servicio de SQL Server.
#>
param(
	[Parameter(Mandatory)][string]$ServerInstance,
	[Parameter(Mandatory)][string]$Database,
	[Parameter(Mandatory)][string]$BackupDirectory,
	[switch]$TrustServerCertificate,
	[switch]$Compress
)

$ErrorActionPreference = 'Stop'
if ($Database -notmatch '^[A-Za-z0-9_.\-]+$') { throw 'Nombre de base de datos no válido.' }
if ($ServerInstance -notmatch '^[A-Za-z0-9_.\-\\,]+$') { throw 'Instancia de SQL Server no válida.' }
if (-not (Get-Command sqlcmd -ErrorAction SilentlyContinue)) { throw 'No se encontró sqlcmd en el PATH.' }
if (-not (Test-Path $BackupDirectory -PathType Container)) { throw "No existe el directorio $BackupDirectory." }

$archivo = Join-Path $BackupDirectory ("{0}_{1:yyyyMMdd_HHmmss}.bak" -f $Database, (Get-Date))
$escapado = $archivo.Replace("'", "''")
$opcionCompresion = if ($Compress) { ', COMPRESSION' } else { '' }
$consulta = "BACKUP DATABASE [$Database] TO DISK = N'$escapado' WITH COPY_ONLY, INIT, CHECKSUM$opcionCompresion; " +
	"RESTORE VERIFYONLY FROM DISK = N'$escapado' WITH CHECKSUM;"

$argumentos = @('-S', $ServerInstance, '-E', '-b', '-Q', $consulta)
if ($TrustServerCertificate) { $argumentos += '-C' }
sqlcmd @argumentos
if ($LASTEXITCODE -ne 0) { throw "Falló el respaldo de $Database (sqlcmd devolvió $LASTEXITCODE)." }
Write-Host "Respaldo verificado: $archivo"