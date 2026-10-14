<#
.SYNOPSIS
Script unificado de compilación, empaquetado y distribución de HSis.
Publica los proyectos en C:\HSis\publish\:
  - Server\        -> Servidor ASP.NET Core / Kestrel
  - UI\            -> Aplicación de Escritorio WinForms
  - Agent\         -> Agente de Notificaciones de Windows
  - ClientPackage\ -> Paquete clasificado (UI + Agent + Certificado LAN)
  - Installer\     -> Instalador ejecutable (.exe) compilado con Inno Setup

.PARAMETER ServerAddress
Dirección IP o Host del servidor LAN (por defecto: 192.168.34.107).

.PARAMETER SoloClientes
Omite la compilación y publicación del servidor, compilando únicamente la UI y el Agente.

.PARAMETER SoloServidor
Omite la compilación de clientes y del instalador, compilando únicamente el servidor API.

.PARAMETER CompilarInstalador
Compila el instalador único con Inno Setup (ISCC.exe) y actualiza los archivos en wwwroot/updates.

.PARAMETER CerrarProcesos
Detiene el servicio HSis.Server y cierra procesos UI/Agent que bloqueen archivos en publish.
#>
param(
    [string]$ServerAddress = '192.168.34.107',
    [string]$PublishFolder = 'C:\HSis\publish',
    [string]$Configuration = 'Release',
    [switch]$SoloClientes,
    [switch]$SoloServidor,
    [switch]$CompilarInstalador = $true,
    [switch]$CerrarProcesos
)

$ErrorActionPreference = 'Stop'
$repoRoot = (Get-Item $PSScriptRoot).Parent.FullName

$serverProject = Join-Path $repoRoot 'HSis.Server\HSis.Server.csproj'
$uiProject = Join-Path $repoRoot 'HSis.UI\HSis.UI.csproj'
$agentProject = Join-Path $repoRoot 'HSis.NotificationAgent\HSis.NotificationAgent.csproj'
$certSource = Join-Path $PSScriptRoot 'lan-cert\hsis-lan.cer'
$issPath = Join-Path $PSScriptRoot 'HSis_Client_Setup.iss'

$serverOut = Join-Path $PublishFolder 'Server'
$uiOut = Join-Path $PublishFolder 'UI'
$agentOut = Join-Path $PublishFolder 'Agent'
$clientPkgOut = Join-Path $PublishFolder 'ClientPackage'
$installerOut = Join-Path $PublishFolder 'Installer'

# 0. Verificación y cierre de procesos bloqueantes
$publishRoot = [IO.Path]::GetFullPath($PublishFolder).TrimEnd('\') + '\'
function Get-ProcesosEnUso {
    @(Get-Process -ErrorAction SilentlyContinue | Where-Object {
        $_.Path -and $_.Path.StartsWith($publishRoot, [StringComparison]::OrdinalIgnoreCase)
    })
}

$enUso = Get-ProcesosEnUso
if ($enUso.Count -gt 0 -and $CerrarProcesos) {
    $servicio = Get-Service -Name 'HSis.Server' -ErrorAction SilentlyContinue
    if ($servicio -and $servicio.Status -ne 'Stopped' -and -not $SoloClientes) {
        Write-Host 'Deteniendo servicio HSis.Server...' -ForegroundColor Yellow
        Stop-Service -Name 'HSis.Server' -Force
    }
    foreach ($p in (Get-ProcesosEnUso)) {
        Write-Host "Cerrando $($p.ProcessName) (PID $($p.Id))..." -ForegroundColor Yellow
        Stop-Process -Id $p.Id -Force -ErrorAction SilentlyContinue
        [void]$p.WaitForExit(10000)
    }
    $enUso = Get-ProcesosEnUso
}

if ($enUso.Count -gt 0) {
    $detalle = ($enUso | ForEach-Object { "  - $($_.ProcessName) (PID $($_.Id)): $($_.Path)" }) -join [Environment]::NewLine
    throw "Cierre estos procesos antes de publicar o use -CerrarProcesos:$([Environment]::NewLine)$detalle"
}

# 1. Publicación del Servidor
if (-not $SoloClientes) {
    Write-Host "============================================================" -ForegroundColor Cyan
    Write-Host " 1. PUBLICACION DEL SERVIDOR (HSis.Server)" -ForegroundColor Cyan
    Write-Host " Destino: $serverOut" -ForegroundColor Cyan
    Write-Host "============================================================" -ForegroundColor Cyan
    dotnet publish $serverProject -c $Configuration -o $serverOut
    if ($LASTEXITCODE -ne 0) { throw "Error al publicar HSis.Server." }
    Write-Host "[OK] Servidor publicado en $serverOut.`n" -ForegroundColor Green
}

# 2. Publicación de Clientes
if (-not $SoloServidor) {
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

    # 4. Clasificación en ClientPackage
    Write-Host "============================================================" -ForegroundColor Cyan
    Write-Host " 4. CLASIFICACION EN ClientPackage" -ForegroundColor Cyan
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

    # 5. Compilación del Instalador con Inno Setup
    if ($CompilarInstalador) {
        Write-Host "`n============================================================" -ForegroundColor Cyan
        Write-Host " 5. COMPILACION DEL INSTALADOR (Inno Setup)" -ForegroundColor Cyan
        Write-Host "============================================================" -ForegroundColor Cyan

        # Detectar ruta de ISCC.exe
        $isccPath = $null
        $candidatos = @(
            "$env:LOCALAPPDATA\Programs\Inno Setup 6\ISCC.exe",
            "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe",
            "$env:ProgramFiles\Inno Setup 6\ISCC.exe"
        )
        foreach ($c in $candidatos) {
            if (Test-Path $c) { $isccPath = $c; break }
        }

        if (-not $isccPath) {
            $cmd = Get-Command iscc.exe -ErrorAction SilentlyContinue
            if ($cmd) { $isccPath = $cmd.Source }
        }

        if (-not $isccPath) {
            Write-Warning "No se encontró ISCC.exe (Inno Setup). Omitiendo compilación del instalador."
        } else {
            # Extraer versión del csproj de la UI
            [xml]$uiCsproj = Get-Content $uiProject
            $versionActual = $uiCsproj.Project.PropertyGroup.Version | Where-Object { $_ } | Select-Object -First 1
            if (-not $versionActual) { $versionActual = "1.0.5.0" }

            Write-Host "Ejecutando Inno Setup (Versión $versionActual)..." -ForegroundColor Yellow
            & $isccPath "/DMyAppVersion=$versionActual" $issPath
            if ($LASTEXITCODE -ne 0) { throw "Error al compilar el instalador con Inno Setup." }

            $setupExe = Join-Path $installerOut 'Setup_HSis_Cliente.exe'
            Write-Host "[OK] Instalador compilado exitosamente en: $setupExe" -ForegroundColor Green

            # 6. Sincronizar archivo con wwwroot/updates del servidor para AutoUpdater
            $serverUpdatesDir = Join-Path $serverOut 'wwwroot\updates'
            if (Test-Path $serverUpdatesDir) {
                Copy-Item -Path $setupExe -Destination $serverUpdatesDir -Force
                
                # Actualizar update.xml
                $xmlContent = @"
<?xml version="1.0" encoding="UTF-8"?>
<item>
    <version>$versionActual</version>
    <url>https://$ServerAddress/updates/Setup_HSis_Cliente.exe</url>
    <changelog>https://$ServerAddress/updates/changelog.html</changelog>
    <mandatory>false</mandatory>
</item>
"@
                $xmlPath = Join-Path $serverUpdatesDir 'update.xml'
                [System.IO.File]::WriteAllText($xmlPath, $xmlContent, [System.Text.Encoding]::UTF8)
                Write-Host "[OK] Actualizaciones sincronizadas en servidor: $serverUpdatesDir" -ForegroundColor Green
            }
        }
    }
}

Write-Host ""
Write-Host "============================================================" -ForegroundColor Green
Write-Host " PUBLICACION Y GENERACION DE INSTALADOR COMPLETADA" -ForegroundColor Green
Write-Host "============================================================" -ForegroundColor Green
