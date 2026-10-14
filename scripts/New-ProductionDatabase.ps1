#Requires -RunAsAdministrator
<#
.SYNOPSIS
Crea e inicializa la base de datos de producción (HSIS_PROD) a partir de la estructura de HSIS.
Permite elegir entre clonar solo los catálogos base (limpia) o copiar también usuarios existentes.

Uso:
  .\New-ProductionDatabase.ps1 [-ServerInstance .\SQLEXPRESS] [-SourceDatabase HSIS] [-TargetDatabase HSIS_PROD] [-CopiarCatalogos] [-CopiarUsuarios]
#>
param(
    [string]$ServerInstance = '.\SQLEXPRESS',
    [string]$SourceDatabase = 'HSIS',
    [string]$TargetDatabase = 'HSIS_PROD',
    [switch]$CopiarCatalogos,
    [switch]$CopiarUsuarios,
    [switch]$Force
)

$ErrorActionPreference = 'Stop'

Write-Host "============================================================" -ForegroundColor Cyan
Write-Host " CREACION DE BASE DE DATOS DE PRODUCCION (HSIS)" -ForegroundColor Cyan
Write-Host " Instancia : $ServerInstance" -ForegroundColor Cyan
Write-Host " Origen    : $SourceDatabase (Pruebas / Desarrollo)" -ForegroundColor Cyan
Write-Host " Destino   : $TargetDatabase (Producción)" -ForegroundColor Cyan
Write-Host "============================================================`n" -ForegroundColor Cyan

# 1. Comprobar si ya existe la base de datos destino
$checkQuery = "SET NOCOUNT ON; SELECT COUNT(*) FROM sys.databases WHERE name = '$TargetDatabase';"
$conteoDb = (sqlcmd -S $ServerInstance -E -C -b -Q $checkQuery -h -1).Trim()
$dbExiste = $conteoDb -match '^[1-9]\d*$'

if ($dbExiste -and (-not $Force)) {
    Write-Warning "La base de datos '$TargetDatabase' ya existe en $ServerInstance."
    Write-Host "Usa el parámetro -Force si deseas recrearla (ADVERTENCIA: Se perderán los datos de $TargetDatabase)." -ForegroundColor Yellow
    return
}

# 2. Generar backup temporal del origen en la carpeta de base de datos del proyecto
$backupFolder = "C:\HSis\HSis.BD"
if (-not (Test-Path $backupFolder)) {
    $backupFolder = [System.IO.Path]::GetTempPath()
}
$bakFile = Join-Path $backupFolder ("{0}_temp_clone_{1:yyyyMMdd_HHmmss}.bak" -f $SourceDatabase, (Get-Date))
$bakFileEscaped = $bakFile.Replace("'", "''")

Write-Host "[1/5] Generando respaldo temporal de $SourceDatabase..." -ForegroundColor Yellow
$backupCmd = "BACKUP DATABASE [$SourceDatabase] TO DISK = N'$bakFileEscaped' WITH COPY_ONLY, INIT, CHECKSUM"
sqlcmd -S $ServerInstance -E -C -b -Q $backupCmd
if ($LASTEXITCODE -ne 0) { throw "Error al respaldar $SourceDatabase." }

# 3. Obtener nombres lógicos de archivos
$fileListCmd = "RESTORE FILELISTONLY FROM DISK = N'$bakFileEscaped'"
$fileList = sqlcmd -S $ServerInstance -E -C -b -Q $fileListCmd -s "|" -W

$logicalData = $null
$logicalLog = $null
foreach ($line in ($fileList -split "`r?`n")) {
    $cols = $line -split '\|'
    if ($cols.Count -gt 2) {
        if ($cols[2].Trim() -eq 'D') { $logicalData = $cols[0].Trim() }
        if ($cols[2].Trim() -eq 'L') { $logicalLog = $cols[0].Trim() }
    }
}

if (-not $logicalData -or -not $logicalLog) {
    throw "No se pudieron obtener los nombres lógicos de los archivos del respaldo."
}

# 4. Determinar ruta para los nuevos archivos de base de datos
$dirQuery = "SELECT CAST(SERVERPROPERTY('InstanceDefaultDataPath') AS varchar(300)) AS DataPath, CAST(SERVERPROPERTY('InstanceDefaultLogPath') AS varchar(300)) AS LogPath"
$pathsOut = sqlcmd -S $ServerInstance -E -C -b -Q $dirQuery -s "|" -h -1 -W
$dataPath = ($pathsOut -split '\|')[0].Trim()
if (-not $dataPath -or -not (Test-Path $dataPath)) {
    $dataPath = "C:\HSis\HSis.BD\"
}

$targetMdf = Join-Path $dataPath "$TargetDatabase.mdf"
$targetLdf = Join-Path $dataPath "$TargetDatabase`_log.ldf"

# 5. Restaurar con nuevo nombre
Write-Host "[2/5] Restaurando $TargetDatabase con nuevos archivos..." -ForegroundColor Yellow
if ($dbExiste -and $Force) {
    $killCmd = "ALTER DATABASE [$TargetDatabase] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$TargetDatabase];"
    sqlcmd -S $ServerInstance -E -C -b -Q $killCmd | Out-Null
}

$restoreCmd = @"
RESTORE DATABASE [$TargetDatabase]
FROM DISK = N'$bakFileEscaped'
WITH 
    MOVE N'$logicalData' TO N'$targetMdf',
    MOVE N'$logicalLog' TO N'$targetLdf',
    REPLACE;
"@
sqlcmd -S $ServerInstance -E -C -b -Q $restoreCmd | Out-Null
if ($LASTEXITCODE -ne 0) { throw "Error al restaurar $TargetDatabase." }

# Eliminar archivo de respaldo temporal
Remove-Item -Path $bakFile -Force -ErrorAction SilentlyContinue

# 6. Limpieza de datos de prueba
Write-Host "[3/5] Limpiando datos transaccionales de prueba..." -ForegroundColor Yellow

$cleanSql = @"
USE [$TargetDatabase];

-- Desactivar temporalmente restricciones para limpieza limpia
EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT all';

-- Limpiar tablas transaccionales de prueba (Tickets, Historial, Notificaciones, Movimientos de prueba)
DELETE FROM dbo.Det_Ticket;
DELETE FROM dbo.Historial_Cambios_Ticket;
DELETE FROM dbo.Notificacion;
DELETE FROM dbo.Ticket;
DELETE FROM dbo.Movimientos_Material;

-- Reiniciar autoincrementables (IDENTITY)
DBCC CHECKIDENT ('dbo.Ticket', RESEED, 0);
DBCC CHECKIDENT ('dbo.Notificacion', RESEED, 0);
DBCC CHECKIDENT ('dbo.Movimientos_Material', RESEED, 0);
DBCC CHECKIDENT ('dbo.Historial_Cambios_Ticket', RESEED, 0);

"@

if (-not $CopiarCatalogos) {
    Write-Host "      (Limpiando también catálogos opcionales de prueba: Materiales, Puestos, Departamentos, Sucursales, Empresas)" -ForegroundColor DarkGray
    $cleanSql += @"
DELETE FROM dbo.Material;
DELETE FROM dbo.Puesto;
DELETE FROM dbo.Departamento;
DELETE FROM dbo.Sucursal;
DELETE FROM dbo.Empresa;
DBCC CHECKIDENT ('dbo.Material', RESEED, 0);
DBCC CHECKIDENT ('dbo.Puesto', RESEED, 0);
DBCC CHECKIDENT ('dbo.Departamento', RESEED, 0);
DBCC CHECKIDENT ('dbo.Sucursal', RESEED, 0);
DBCC CHECKIDENT ('dbo.Empresa', RESEED, 0);
"@
}

if (-not $CopiarUsuarios) {
    Write-Host "      (Conservando únicamente el usuario Administrador y limpiando usuarios de prueba)" -ForegroundColor DarkGray
    $cleanSql += @"
-- Conservar sólo el Administrador (id_Rol = 1) o dejar admin inicial
DELETE FROM dbo.Usuario WHERE id_Rol <> 1;
"@
}

$cleanSql += @"
-- Reactivar todas las restricciones y triggers
EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all';
"@

$sqlFile = Join-Path $backupFolder "temp_clean_$TargetDatabase.sql"
Set-Content -Path $sqlFile -Value $cleanSql -Encoding UTF8
sqlcmd -S $ServerInstance -d $TargetDatabase -E -C -b -i $sqlFile
$cleanExitCode = $LASTEXITCODE
Remove-Item -Path $sqlFile -Force -ErrorAction SilentlyContinue
if ($cleanExitCode -ne 0) { throw "Error al limpiar datos en $TargetDatabase." }

# 7. Asegurar que los roles base existan
Write-Host "[4/5] Verificando roles del sistema en $TargetDatabase..." -ForegroundColor Yellow
$seedRolesSql = @"
USE [$TargetDatabase];
IF NOT EXISTS (SELECT 1 FROM Rol_Usuario WHERE id_Rol = 1) INSERT INTO Rol_Usuario (id_Rol, Descripcion) VALUES (1, 'Administrador');
IF NOT EXISTS (SELECT 1 FROM Rol_Usuario WHERE id_Rol = 2) INSERT INTO Rol_Usuario (id_Rol, Descripcion) VALUES (2, 'Técnico');
IF NOT EXISTS (SELECT 1 FROM Rol_Usuario WHERE id_Rol = 3) INSERT INTO Rol_Usuario (id_Rol, Descripcion) VALUES (3, 'Usuario');
"@
sqlcmd -S $ServerInstance -d $TargetDatabase -E -C -b -Q $seedRolesSql | Out-Null

# 8. Comprobar integridad final
Write-Host "[5/5] Auditando base de datos generada..." -ForegroundColor Yellow
$verifySql = @"
USE [$TargetDatabase];
SELECT 
    (SELECT COUNT(*) FROM Rol_Usuario) AS Roles,
    (SELECT COUNT(*) FROM Usuario) AS Usuarios,
    (SELECT COUNT(*) FROM Ticket) AS Tickets,
    (SELECT COUNT(*) FROM Notificacion) AS Notificaciones;
"@
sqlcmd -S $ServerInstance -d $TargetDatabase -E -C -b -Q $verifySql

Write-Host "`n============================================================" -ForegroundColor Green
Write-Host " [EXITO] BASE DE DATOS '$TargetDatabase' CREADA Y LISTA" -ForegroundColor Green
Write-Host "============================================================" -ForegroundColor Green
Write-Host "Para vincular el servicio de producción a esta BD, ejecuta:" -ForegroundColor Cyan
Write-Host ".\scripts\Install-HSisServer.ps1 -SqlConnectionString ""Server=$ServerInstance;Database=$TargetDatabase;Integrated Security=True;Encrypt=True;TrustServerCertificate=True"" ..." -ForegroundColor White
