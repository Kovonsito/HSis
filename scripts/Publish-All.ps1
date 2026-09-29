<#
.SYNOPSIS
Publica de forma unificada y clasifica los 3 proyectos en C:\HSis\publish\:
  - publish\Server\        -> Servidor ASP.NET Core / Kestrel
  - publish\UI\            -> Aplicación de Escritorio WinForms
  - publish\Agent\         -> Agente de Notificaciones de Windows
  - publish\ClientPackage\ -> Paquete de despliegue para clientes LAN (UI + Agent + Certificado + Setup)
#>
param(
    [string]$ServerAddress = '192.168.34.107',
    [string]$PublishFolder = 'C:\HSis\publish',
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'
$repoRoot = (Get-Item $PSScriptRoot).Parent.FullName

$serverProject = Join-Path $repoRoot 'HSis.Server\HSis.Server.csproj'
$uiProject = Join-Path $repoRoot 'HSis.UI\HSis.UI.csproj'
$agentProject = Join-Path $repoRoot 'HSis.NotificationAgent\HSis.NotificationAgent.csproj'
$certSource = Join-Path $PSScriptRoot 'lan-cert\hsis-lan.cer'

$serverOut = Join-Path $PublishFolder 'Server'
$uiOut = Join-Path $PublishFolder 'UI'
$agentOut = Join-Path $PublishFolder 'Agent'
$clientPkgOut = Join-Path $PublishFolder 'ClientPackage'

Write-Host "============================================================" -ForegroundColor Cyan
Write-Host " 1. PUBLICACION DEL SERVIDOR (HSis.Server)" -ForegroundColor Cyan
Write-Host " Destino: $serverOut" -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan
dotnet publish $serverProject -c $Configuration -o $serverOut
if ($LASTEXITCODE -ne 0) { throw "Error al publicar HSis.Server." }
Write-Host "[OK] Servidor publicado en $serverOut.`n" -ForegroundColor Green

Write-Host "============================================================" -ForegroundColor Cyan
Write-Host " 2. PUBLICACION DE LA INTERFAZ DE USUARIO (HSis.UI)" -ForegroundColor Cyan
Write-Host " Destino: $uiOut" -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan
dotnet publish $uiProject -c $Configuration -r win-x64 --self-contained false -o $uiOut
if ($LASTEXITCODE -ne 0) { throw "Error al publicar HSis.UI." }

# Configurar appsettings.json de la UI
$uiSettingsPath = Join-Path $uiOut 'appsettings.json'
if (Test-Path $uiSettingsPath) {
    $uiJson = Get-Content $uiSettingsPath -Raw | ConvertFrom-Json
    $uiJson.ApiSettings.BaseUrl = "https://$ServerAddress"
    $uiJson.SignalR.ServerUrl = "https://$ServerAddress/notificationHub"
    $uiJson | ConvertTo-Json -Depth 10 | Set-Content $uiSettingsPath -Encoding UTF8
}
$devUiSettings = Join-Path $uiOut 'appsettings.Development.json'
if (Test-Path $devUiSettings) { Remove-Item $devUiSettings -Force }
Write-Host "[OK] UI publicada y configurada con https://$ServerAddress.`n" -ForegroundColor Green

Write-Host "============================================================" -ForegroundColor Cyan
Write-Host " 3. PUBLICACION DEL AGENTE (HSis.NotificationAgent)" -ForegroundColor Cyan
Write-Host " Destino: $agentOut" -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan
dotnet publish $agentProject -c $Configuration -r win-x64 --self-contained false -o $agentOut
if ($LASTEXITCODE -ne 0) { throw "Error al publicar HSis.NotificationAgent." }

# Configurar appsettings.json del Agente
$agentSettingsPath = Join-Path $agentOut 'appsettings.json'
if (Test-Path $agentSettingsPath) {
    $agentJson = Get-Content $agentSettingsPath -Raw | ConvertFrom-Json
    $agentJson.ApiSettings.BaseUrl = "https://$ServerAddress"
    $agentJson.SignalR.ServerUrl = "https://$ServerAddress/notificationHub"
    $agentJson | ConvertTo-Json -Depth 10 | Set-Content $agentSettingsPath -Encoding UTF8
}
$devAgentSettings = Join-Path $agentOut 'appsettings.Development.json'
if (Test-Path $devAgentSettings) { Remove-Item $devAgentSettings -Force }
Write-Host "[OK] Agente publicado y configurado con https://$ServerAddress.`n" -ForegroundColor Green

Write-Host "============================================================" -ForegroundColor Cyan
Write-Host " 4. EMPAQUETADO CLIENTE LAN (ClientPackage)" -ForegroundColor Cyan
Write-Host " Destino: $clientPkgOut" -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan
if (Test-Path $clientPkgOut) { Remove-Item $clientPkgOut -Recurse -Force }
New-Item -ItemType Directory -Force -Path $clientPkgOut | Out-Null

Copy-Item -Path $uiOut -Destination (Join-Path $clientPkgOut 'UI') -Recurse -Force
Copy-Item -Path $agentOut -Destination (Join-Path $clientPkgOut 'Agent') -Recurse -Force

if (Test-Path $certSource) {
    Copy-Item -Path $certSource -Destination $clientPkgOut -Force
    Write-Host "[OK] Certificado copiado a $clientPkgOut\hsis-lan.cer" -ForegroundColor Green
}

# Generar script de instalación cliente con comillas simples y sustitución limpia
$clientScriptTemplate = @'
#Requires -RunAsAdministrator
$certPath = Join-Path $PSScriptRoot 'hsis-lan.cer'
if (-not (Test-Path $certPath)) {
    throw 'No se encontro el certificado hsis-lan.cer en la misma carpeta.'
}

Write-Host 'Instalando certificado de confianza para HSis LAN...' -ForegroundColor Cyan
Import-Certificate -FilePath $certPath -CertStoreLocation Cert:\LocalMachine\Root | Out-Null
Write-Host '[OK] Certificado instalado con exito en Cert:\LocalMachine\Root.' -ForegroundColor Green

$targetUrl = "https://__SERVER_ADDRESS__"
Write-Host "Probando conexion con el servidor en $targetUrl ..." -ForegroundColor Cyan
try {
    $response = Invoke-WebRequest -Uri $targetUrl -Method Get -TimeoutSec 5 -SkipCertificateCheck:$false
    Write-Host '[OK] Conexion HTTPS establecida exitosamente.' -ForegroundColor Green
} catch {
    if ($_.Exception.Response) {
        Write-Host '[OK] Conexion HTTPS establecida exitosamente (Respuesta del servidor recibida).' -ForegroundColor Green
    } else {
        Write-Warning ('No se pudo conectar a ' + $targetUrl + ': ' + $_.Exception.Message)
    }
}
Write-Host 'Configuracion completada. Ya puedes ejecutar UI\HSis.UI.exe.' -ForegroundColor Green
pause
'@

$clientScriptFinal = $clientScriptTemplate.Replace('__SERVER_ADDRESS__', $ServerAddress)
$clientSetupPath = Join-Path $clientPkgOut 'Instalar-Certificado-Cliente.ps1'
[System.IO.File]::WriteAllText($clientSetupPath, $clientScriptFinal, [System.Text.Encoding]::UTF8)
Write-Host "[OK] Script de instalacion para clientes creado en $clientSetupPath." -ForegroundColor Green

Write-Host ""
Write-Host "============================================================" -ForegroundColor Green
Write-Host " PUBLICACION Y CLASIFICACION COMPLETADA EXITOSAMENTE" -ForegroundColor Green
Write-Host " Estructura en la carpeta publish:" -ForegroundColor Green
Write-Host "   Server\        (Servidor Web API / Kestrel)" -ForegroundColor Gray
Write-Host "   UI\            (Interfaz de escritorio WinForms)" -ForegroundColor Gray
Write-Host "   Agent\         (Agente de notificaciones Windows)" -ForegroundColor Gray
Write-Host "   ClientPackage\ (Paquete distribuible para clientes LAN)" -ForegroundColor Gray
Write-Host "============================================================" -ForegroundColor Green
