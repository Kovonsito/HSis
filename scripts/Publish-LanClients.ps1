<#
.SYNOPSIS
Compila y publica las aplicaciones cliente (UI y Agente) listas para desplegar en equipos de la red local.
Configura las URLs con la IP LAN del servidor y empaqueta el certificado y script de confianza para clientes.
#>
param(
    [string]$ServerAddress = '192.168.34.107',
    [string]$OutputFolder = 'C:\HSis\publish\ClientPackage',
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'
$repoRoot = (Get-Item $PSScriptRoot).Parent.FullName

$uiProject = Join-Path $repoRoot 'HSis.UI\HSis.UI.csproj'
$agentProject = Join-Path $repoRoot 'HSis.NotificationAgent\HSis.NotificationAgent.csproj'
$certSource = Join-Path $PSScriptRoot 'lan-cert\hsis-lan.cer'

$uiOut = Join-Path $OutputFolder 'UI'
$agentOut = Join-Path $OutputFolder 'Agent'

Write-Host "=== 1. Publicando HSis.UI ===" -ForegroundColor Cyan
dotnet publish $uiProject -c $Configuration -r win-x64 --self-contained false -o $uiOut
if ($LASTEXITCODE -ne 0) { throw "Fallo al publicar HSis.UI." }

Write-Host "`n=== 2. Publicando HSis.NotificationAgent ===" -ForegroundColor Cyan
dotnet publish $agentProject -c $Configuration -r win-x64 --self-contained false -o $agentOut
if ($LASTEXITCODE -ne 0) { throw "Fallo al publicar HSis.NotificationAgent." }

Write-Host "`n=== 3. Configurando appsettings.json para LAN ($ServerAddress) ===" -ForegroundColor Cyan

# Configurar UI appsettings.json
$uiSettingsPath = Join-Path $uiOut 'appsettings.json'
if (Test-Path $uiSettingsPath) {
    $uiJson = Get-Content $uiSettingsPath -Raw | ConvertFrom-Json
    $uiJson.ApiSettings.BaseUrl = "https://$ServerAddress"
    $uiJson.SignalR.ServerUrl = "https://$ServerAddress/notificationHub"
    $uiJson | ConvertTo-Json -Depth 10 | Set-Content $uiSettingsPath -Encoding UTF8
    Write-Host "[OK] $uiSettingsPath actualizado con https://$ServerAddress." -ForegroundColor Green
}

# Configurar Agent appsettings.json
$agentSettingsPath = Join-Path $agentOut 'appsettings.json'
if (Test-Path $agentSettingsPath) {
    $agentJson = Get-Content $agentSettingsPath -Raw | ConvertFrom-Json
    $agentJson.ApiSettings.BaseUrl = "https://$ServerAddress"
    $agentJson.SignalR.ServerUrl = "https://$ServerAddress/notificationHub"
    $agentJson | ConvertTo-Json -Depth 10 | Set-Content $agentSettingsPath -Encoding UTF8
    Write-Host "[OK] $agentSettingsPath actualizado con https://$ServerAddress." -ForegroundColor Green
}

# Eliminar appsettings.Development.json de la carpeta de publicación si existe
$devFiles = @(
    (Join-Path $uiOut 'appsettings.Development.json'),
    (Join-Path $agentOut 'appsettings.Development.json')
)
foreach ($devFile in $devFiles) {
    if (Test-Path $devFile) {
        Remove-Item $devFile -Force
        Write-Host "[OK] Removido archivo de desarrollo $devFile de la publicación." -ForegroundColor Yellow
    }
}

Write-Host "`n=== 4. Empaquetando Certificado y Script para Equipos Cliente ===" -ForegroundColor Cyan
if (Test-Path $certSource) {
    Copy-Item -Path $certSource -Destination $OutputFolder -Force
    Write-Host "[OK] Certificado copiado a $OutputFolder\hsis-lan.cer" -ForegroundColor Green
} else {
    Write-Warning "No se encontró el certificado en $certSource."
}

# Crear script de configuración cliente
$clientSetupContent = @"
#Requires -RunAsAdministrator
`$certPath = Join-Path `$PSScriptRoot 'hsis-lan.cer'
if (-not (Test-Path `$certPath)) {
    throw 'No se encontro el certificado hsis-lan.cer en la misma carpeta.'
}

Write-Host 'Instalando certificado de confianza para HSis LAN...' -ForegroundColor Cyan
Import-Certificate -FilePath `$certPath -CertStoreLocation Cert:\LocalMachine\Root | Out-Null
Write-Host '[OK] Certificado instalado con exito en Cert:\LocalMachine\Root.' -ForegroundColor Green

Write-Host 'Probando conexion con el servidor en https://${ServerAddress} ...' -ForegroundColor Cyan
try {
    `$response = Invoke-WebRequest -Uri 'https://${ServerAddress}' -Method Get -TimeoutSec 5 -SkipCertificateCheck:`$false
    Write-Host '[OK] Conexion HTTPS establecida exitosamente.' -ForegroundColor Green
} catch {
    if (`$_.Exception.Response) {
        Write-Host '[OK] Conexion HTTPS establecida exitosamente (Respuesta del servidor recibida).' -ForegroundColor Green
    } else {
        Write-Warning ('No se pudo conectar a https://${ServerAddress}: ' + `$_.Exception.Message)
    }
}
Write-Host 'Configuracion completada. Ya puedes ejecutar UI\HSis.UI.exe.' -ForegroundColor Green
pause
"@

$clientSetupPath = Join-Path $OutputFolder 'Instalar-Certificado-Cliente.ps1'
Set-Content -Path $clientSetupPath -Value $clientSetupContent -Encoding UTF8
Write-Host "[OK] Script de instalacion para clientes creado en $clientSetupPath." -ForegroundColor Green

Write-Host "`nEmpaquetado completado en $OutputFolder." -ForegroundColor Green
