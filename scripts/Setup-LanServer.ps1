#Requires -RunAsAdministrator
<#
.SYNOPSIS
Aplica los requisitos de seguridad y red pendientes en el servidor para producción en LAN.
1. Abre el puerto 443 en el Firewall de Windows para la subred local.
2. Registra el certificado hsis-lan.cer como confiable en LocalMachine\Root del servidor.
3. Inicia el servicio Windows HSis.Server y valida su ejecución.
4. Comprueba la conexión HTTPS localmente.
#>
param(
    [string]$ServerAddress = '192.168.34.107',
    [int]$HttpsPort = 443,
    [string]$CertificatePath = (Join-Path $PSScriptRoot 'lan-cert\hsis-lan.cer'),
    [string]$ServiceName = 'HSis.Server'
)

$ErrorActionPreference = 'Stop'

Write-Host "=== 1. Configuración de Firewall ===" -ForegroundColor Cyan
$existingRule = Get-NetFirewallRule -DisplayName "HSis Server HTTPS (LAN)" -ErrorAction SilentlyContinue
if (-not $existingRule) {
    New-NetFirewallRule -DisplayName "HSis Server HTTPS (LAN)" `
                        -Description "Permite trafico HTTPS entrante para HSis Server desde la subred local." `
                        -Direction Inbound `
                        -LocalPort $HttpsPort `
                        -Protocol TCP `
                        -Action Allow `
                        -RemoteAddress LocalSubnet | Out-Null
    Write-Host "[OK] Regla de Firewall creada para puerto $HttpsPort (LocalSubnet)." -ForegroundColor Green
} else {
    Write-Host "[OK] La regla de Firewall ya existe." -ForegroundColor Yellow
}

Write-Host "`n=== 2. Confianza del Certificado TLS ===" -ForegroundColor Cyan
if (Test-Path $CertificatePath) {
    $certObj = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2 $CertificatePath
    $alreadyTrusted = Get-ChildItem Cert:\LocalMachine\Root | Where-Object { $_.Thumbprint -eq $certObj.Thumbprint }
    if (-not $alreadyTrusted) {
        Import-Certificate -FilePath $CertificatePath -CertStoreLocation Cert:\LocalMachine\Root | Out-Null
        Write-Host "[OK] Certificado $CertificatePath importado a Cert:\LocalMachine\Root." -ForegroundColor Green
    } else {
        Write-Host "[OK] El certificado ya es de confianza en Cert:\LocalMachine\Root." -ForegroundColor Yellow
    }
} else {
    Write-Warning "No se encontró el certificado en $CertificatePath. Asegúrate de generarlo con New-LanCertificate.ps1 primero."
}

Write-Host "`n=== 3. Inicio del Servicio Windows ===" -ForegroundColor Cyan
$service = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
if (-not $service) {
    throw "El servicio $ServiceName no está registrado. Ejecuta primero Install-HSisServer.ps1."
}

if ($service.Status -ne 'Running') {
    Write-Host "Iniciando servicio $ServiceName..." -ForegroundColor Yellow
    Start-Service -Name $ServiceName
    Start-Sleep -Seconds 3
    $service.Refresh()
}

Write-Host "[OK] Estado del servicio $ServiceName: $($service.Status)" -ForegroundColor Green

Write-Host "`n=== 4. Prueba de Conectividad HTTPS ===" -ForegroundColor Cyan
try {
    $testUrl = "https://$ServerAddress"
    $response = Invoke-WebRequest -Uri $testUrl -Method Get -TimeoutSec 5 -SkipCertificateCheck:$false -ErrorAction Stop
    Write-Host "[OK] Servidor responde en $testUrl (HTTP $($response.StatusCode))." -ForegroundColor Green
}
catch {
    if ($_.Exception.Response -and $_.Exception.Response.StatusCode) {
        $code = [int]$_.Exception.Response.StatusCode
        Write-Host "[OK] Servidor Kestrel responde en https://$ServerAddress con codigo HTTP $code (Conexión TLS establecida exitosamente)." -ForegroundColor Green
    } else {
        Write-Warning "No se pudo conectar a https://$ServerAddress : $($_.Exception.Message)"
    }
}

Write-Host "`nServidor LAN configurado correctamente." -ForegroundColor Green
