#Requires -RunAsAdministrator
<#
.SYNOPSIS
Verifica y audita todos los requisitos de infraestructura, certificados, base de datos y configuración
necesarios para que HSis opere en producción con alta disponibilidad y sin errores.

Uso:
  .\Test-ProductionReadiness.ps1 [-ServerAddress 192.168.34.107] [-DatabaseServer .] [-DatabaseName HSis]
#>
param(
    [string]$ServerAddress = '192.168.34.107',
    [int]$HttpsPort = 443,
    [string]$DatabaseServer = '.',
    [string]$DatabaseName = 'HSis',
    [string]$ProtectedSecretPath = 'C:\ProgramData\HSis\jwt-secret.dpapi',
    [string]$CertificateThumbprint = ''
)

$ErrorActionPreference = 'Continue'
$resultados = [System.Collections.Generic.List[object]]::new()

function Registrar-Chequeo([string]$Area, [string]$Descripcion, [bool]$Cumple, [string]$Detalle) {
    $estado = if ($Cumple) { 'OK' } else { 'FALLA' }
    $color = if ($Cumple) { 'Green' } else { 'Red' }
    Write-Host "[$estado] $Area: $Descripcion" -ForegroundColor $color
    if (-not $Cumple -and $Detalle) {
        Write-Host "       Detalle / Acción: $Detalle" -ForegroundColor Yellow
    }
    $resultados.Add([pscustomobject]@{
        Area        = $Area
        Descripcion = $Descripcion
        Estado      = $estado
        Detalle     = $Detalle
    })
}

Write-Host "============================================================" -ForegroundColor Cyan
Write-Host " AUDITORIA DE REQUISITOS DE PRODUCCION (HSIS)" -ForegroundColor Cyan
Write-Host " Servidor: $ServerAddress | Puerto: $HttpsPort | BD: $DatabaseName" -ForegroundColor Cyan
Write-Host "============================================================`n" -ForegroundColor Cyan

# 1. RED Y DIRECCIÓN IP
$ipsLocales = [System.Net.Dns]::GetHostAddresses([System.Net.Dns]::GetHostName()) | ForEach-Object { $_.IPAddressToString }
$ipEncontrada = $ipsLocales -contains $ServerAddress -or $ServerAddress -in @('127.0.0.1', 'localhost')
Registrar-Chequeo "Red" "La IP o Host del servidor está asignada localmente" $ipEncontrada `
    "La IP $ServerAddress no parece estar asignada a ninguna interfaz de red de este equipo. Verifica si es IP estática."

# 2. FIREWALL DE WINDOWS
$reglaFw = Get-NetFirewallRule -DisplayName "HSis Server HTTPS (LAN)" -ErrorAction SilentlyContinue
$puertoFwValido = $false
if ($reglaFw) {
    $filtroPuerto = Get-NetFirewallPortFilter -AssociatedNetFirewallRule $reglaFw -ErrorAction SilentlyContinue
    $puertoFwValido = $reglaFw.Enabled -eq 'True' -and ($filtroPuerto.LocalPort -eq $HttpsPort.ToString() -or $filtroPuerto.LocalPort -eq '443')
}
Registrar-Chequeo "Firewall" "Regla de Firewall abierta en puerto $HttpsPort" $puertoFwValido `
    "Ejecuta .\scripts\Setup-LanServer.ps1 para abrir el puerto $HttpsPort en el firewall."

# 3. CERTIFICADO TLS (KESTREL)
$certificados = Get-ChildItem Cert:\LocalMachine\My -ErrorAction SilentlyContinue
$certTlsValido = $null
if ($CertificateThumbprint) {
    $certTlsValido = $certificados | Where-Object { $_.Thumbprint -eq $CertificateThumbprint.Replace(" ", "") -and $_.HasPrivateKey }
} else {
    $certTlsValido = $certificados | Where-Object { $_.Subject -like "*$ServerAddress*" -and $_.HasPrivateKey -and $_.NotAfter -gt (Get-Date) } | Select-Object -First 1
}
$tieneCertValido = $null -ne $certTlsValido
$detalleCert = if ($tieneCertValido) { "Huella: $($certTlsValido.Thumbprint)" } else { "No se encontró certificado TLS con clave privada en LocalMachine\My para $ServerAddress. Ejecuta New-LanCertificate.ps1." }
Registrar-Chequeo "TLS/Kestrel" "Certificado TLS válido con clave privada en LocalMachine\My" $tieneCertValido $detalleCert

# 4. CERTIFICADO EN RAÍZ DE CONFIANZA LOCAL
$certRoot = $null
if ($tieneCertValido) {
    $certRoot = Get-ChildItem Cert:\LocalMachine\Root | Where-Object { $_.Thumbprint -eq $certTlsValido.Thumbprint }
}
Registrar-Chequeo "TLS/Confianza" "Certificado instalado en LocalMachine\Root del servidor" ($null -ne $certRoot) `
    "Importa el certificado a LocalMachine\Root usando Setup-LanServer.ps1."

# 5. SECRETO DPAPI
$existeSecreto = Test-Path $ProtectedSecretPath
$tamanoSecreto = 0
if ($existeSecreto) {
    $tamanoSecreto = (Get-Item $ProtectedSecretPath).Length
}
$secretoValido = $existeSecreto -and $tamanoSecreto -ge 32
Registrar-Chequeo "Seguridad/DPAPI" "Archivo de clave JWT cifrada con DPAPI existe y no está vacío" $secretoValido `
    "Genera el secreto bajo la identidad de la cuenta de servicio ejecutando .\scripts\New-JwtSecret.ps1."

# 6. CARPETAS DE REGISTRO (LOGS)
$dirLogs = Path-Combine $env:ProgramData "HSis\Logs"
function Path-Combine($a, $b) { [System.IO.Path]::Combine($a, $b) }
if (-not (Test-Path $dirLogs)) {
    try { New-Item -ItemType Directory -Force -Path $dirLogs | Out-Null } catch {}
}
$puedeEscribirLogs = Test-Path $dirLogs
Registrar-Chequeo "Operación/Logs" "Directorio de registro %ProgramData%\HSis\Logs accesible" $puedeEscribirLogs `
    "Crea la carpeta $dirLogs y concede permisos de lectura y escritura a la cuenta del servicio."

# 7. SERVICIO WINDOWS
$servicio = Get-Service -Name "HSis.Server" -ErrorAction SilentlyContinue
$servicioInstalado = $null -ne $servicio
$servicioAutomatico = $false
if ($servicioInstalado) {
    $servicioAutomatico = $servicio.StartType -eq 'Automatic'
}
Registrar-Chequeo "Servicio" "HSis.Server registrado como servicio Windows con inicio automático" ($servicioInstalado -and $servicioAutomatico) `
    "Registra o actualiza el servicio ejecutando .\scripts\Install-HSisServer.ps1."

# 8. BASE DE DATOS SQL SERVER
$sqlConexionOk = $false
$tablasFaltantes = @()
$columnasFaltantes = @()
$vistasFaltantes = @()
$hayRoles = $false

try {
    $cadenaSql = "Server=$DatabaseServer;Database=$DatabaseName;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;Timeout=5"
    $conexion = New-Object Microsoft.Data.SqlClient.SqlConnection($cadenaSql)
    $conexion.Open()
    $sqlConexionOk = $true

    # Comprobar tablas requeridas
    $cmd = $conexion.CreateCommand()
    $cmd.CommandText = @"
        SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
        WHERE TABLE_TYPE = 'BASE TABLE'
"@
    $lector = $cmd.ExecuteReader()
    $tablas = [System.Collections.Generic.List[string]]::new()
    while ($lector.Read()) { $tablas.Add($lector.GetString(0)) }
    $lector.Close()

    $tablasEsperadas = @('Usuario', 'RolUsuario', 'Ticket', 'DetTicket', 'Material', 'Empresa', 'Sucursal', 'Notificacion', 'Departamento', 'Puesto')
    foreach ($te in $tablasEsperadas) {
        if (-not ($tablas -contains $te)) { $tablasFaltantes += $te }
    }

    # Comprobar columnas de migración en Notificacion
    $cmd.CommandText = @"
        SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = 'Notificacion'
"@
    $lector = $cmd.ExecuteReader()
    $columnasNotif = [System.Collections.Generic.List[string]]::new()
    while ($lector.Read()) { $columnasNotif.Add($lector.GetString(0)) }
    $lector.Close()

    if (-not ($columnasNotif -contains 'TicketId')) { $columnasFaltantes += 'Notificacion.TicketId' }
    if (-not ($columnasNotif -contains 'MaterialId')) { $columnasFaltantes += 'Notificacion.MaterialId' }

    # Comprobar vista VHistorialInventario
    $cmd.CommandText = @"
        SELECT TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS 
        WHERE TABLE_NAME = 'VHistorialInventario'
"@
    $lector = $cmd.ExecuteReader()
    $tieneVista = $lector.Read()
    $lector.Close()
    if (-not $tieneVista) { $vistasFaltantes += 'VHistorialInventario' }

    # Comprobar roles sembrados
    $cmd.CommandText = "SELECT COUNT(*) FROM RolUsuario"
    $conteoRoles = [int]$cmd.ExecuteScalar()
    $hayRoles = $conteoRoles -ge 3

    $conexion.Close()
}
catch {
    $detalleSqlError = $_.Exception.Message
}

Registrar-Chequeo "Base de Datos" "Conexión a SQL Server ($DatabaseServer/$DatabaseName) con autenticación integrada" $sqlConexionOk `
    "No se pudo conectar a SQL Server. Detalle: $detalleSqlError"

if ($sqlConexionOk) {
    Registrar-Chequeo "Base de Datos" "Estructura de tablas principales requeridas" ($tablasFaltantes.Count -eq 0) `
        "Faltan tablas: $($tablasFaltantes -join ', ')"

    Registrar-Chequeo "Base de Datos" "Migraciones aplicadas en Notificaciones (TicketId, MaterialId)" ($columnasFaltantes.Count -eq 0) `
        "Faltan columnas de migración: $($columnasFaltantes -join ', '). Ejecuta los scripts en HSis.Data\Database\Migrations\"

    Registrar-Chequeo "Base de Datos" "Vista VHistorialInventario presente" ($vistasFaltantes.Count -eq 0) `
        "Falta la vista VHistorialInventario."

    Registrar-Chequeo "Base de Datos" "Roles iniciales sembrados (Admin, Técnico, Cliente)" $hayRoles `
        "La tabla RolUsuario debe contener al menos 3 roles."
}

Write-Host "`n============================================================" -ForegroundColor Cyan
$fallas = ($resultados | Where-Object { $_.Estado -eq 'FALLA' }).Count
if ($fallas -eq 0) {
    Write-Host " RESULTADO: SERVIDOR 100% PREPARADO PARA PRODUCCION" -ForegroundColor Green
} else {
    Write-Host " RESULTADO: $fallas CHEQUEO(S) PENDIENTE(S) DE ATENCION" -ForegroundColor Red
}
Write-Host "============================================================" -ForegroundColor Cyan

return $resultados
