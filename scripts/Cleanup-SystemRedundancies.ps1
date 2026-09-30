#Requires -RunAsAdministrator
<#
.SYNOPSIS
Limpia configuraciones redundantes y obsoletas del sistema:
1. Elimina el servicio obsoleto 'HSisNotificationServer' (ruta inexistente C:\HSis_Publish).
2. Limpia reglas duplicadas del Firewall de Windows para el puerto 5000 y asegura la regla HTTPS (443).
3. Asegura la confianza del certificado TLS LAN (hsis-lan.cer) en Cert:\LocalMachine\Root.
4. Valida y sincroniza el estado del servicio 'HSis.Server'.
#>
param(
    [string]$ServerAddress = '192.168.34.107',
    [int]$HttpsPort = 443,
    [string]$CertPath = (Join-Path $PSScriptRoot 'lan-cert\hsis-lan.cer')
)

$ErrorActionPreference = 'Stop'

Write-Host "============================================================" -ForegroundColor Cyan
Write-Host " 1. LIMPIEZA DE SERVICIOS HUERFANOS / OBSOLETOS" -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan

$orphanService = Get-Service -Name 'HSisNotificationServer' -ErrorAction SilentlyContinue
if ($orphanService) {
    Write-Host "Detectado servicio obsoleto 'HSisNotificationServer'. Deteniendo y eliminando..." -ForegroundColor Yellow
    if ($orphanService.Status -eq 'Running') {
        Stop-Service -Name 'HSisNotificationServer' -Force
    }
    & sc.exe delete 'HSisNotificationServer' | Out-Null
    Write-Host "[OK] Servicio 'HSisNotificationServer' eliminado exitosamente." -ForegroundColor Green
}
else {
    Write-Host "[OK] No existe el servicio obsoleto 'HSisNotificationServer'." -ForegroundColor Green
}

Write-Host "`n============================================================" -ForegroundColor Cyan
Write-Host " 2. DEPURACION DE REGLAS DUPLICADAS DEL FIREWALL" -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan

$duplicateRules = @(
    'HSis Notification Server (Port 5000)',
    'HSis API - Acceso HTTP',
    'HSis API Server 5000',
    'HSis Server Port 5000'
)

foreach ($ruleName in $duplicateRules) {
    $rule = Get-NetFirewallRule -DisplayName $ruleName -ErrorAction SilentlyContinue
    if ($rule) {
        Remove-NetFirewallRule -DisplayName $ruleName
        Write-Host "[OK] Regla redundante eliminada: '$ruleName'" -ForegroundColor Yellow
    }
}

# Mantener una sola regla limpia y controlada para desarrollo en LAN (puerto 5000)
$devRule = Get-NetFirewallRule -DisplayName "HSis Server HTTP Dev (Port 5000)" -ErrorAction SilentlyContinue
if (-not $devRule) {
    New-NetFirewallRule -DisplayName "HSis Server HTTP Dev (Port 5000)" `
        -Description "Permite trafico HTTP de desarrollo hacia Kestrel desde la subred local." `
        -Direction Inbound `
        -LocalPort 5000 `
        -Protocol TCP `
        -Action Allow `
        -RemoteAddress LocalSubnet | Out-Null
    Write-Host "[OK] Creada regla unica limpia para desarrollo en LAN: 'HSis Server HTTP Dev (Port 5000)'." -ForegroundColor Green
}
else {
    Write-Host "[OK] Regla de desarrollo 'HSis Server HTTP Dev (Port 5000)' activa." -ForegroundColor Green
}

# Asegurar la regla de produccion HTTPS (puerto 443)
$prodRule = Get-NetFirewallRule -DisplayName "HSis Server HTTPS (LAN)" -ErrorAction SilentlyContinue
if (-not $prodRule) {
    New-NetFirewallRule -DisplayName "HSis Server HTTPS (LAN)" `
        -Description "Permite trafico HTTPS de produccion hacia HSis Server desde la subred local." `
        -Direction Inbound `
        -LocalPort $HttpsPort `
        -Protocol TCP `
        -Action Allow `
        -RemoteAddress LocalSubnet | Out-Null
    Write-Host "[OK] Creada regla oficial de produccion: 'HSis Server HTTPS (LAN)' (Puerto $HttpsPort)." -ForegroundColor Green
}
else {
    Write-Host "[OK] Regla oficial de produccion 'HSis Server HTTPS (LAN)' activa." -ForegroundColor Green
}

Write-Host "`n============================================================" -ForegroundColor Cyan
Write-Host " 3. CONFIANZA DEL CERTIFICADO TLS DEL SERVIDOR" -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan

if (Test-Path $CertPath) {
    $certObj = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2 $CertPath
    $alreadyTrusted = Get-ChildItem Cert:\LocalMachine\Root | Where-Object { $_.Thumbprint -eq $certObj.Thumbprint }
    if (-not $alreadyTrusted) {
        Import-Certificate -FilePath $CertPath -CertStoreLocation Cert:\LocalMachine\Root | Out-Null
        Write-Host "[OK] Certificado $CertPath importado a Cert:\LocalMachine\Root." -ForegroundColor Green
    }
    else {
        Write-Host "[OK] El certificado '$($certObj.Subject)' (Huella: $($certObj.Thumbprint)) ya es de confianza en Cert:\LocalMachine\Root." -ForegroundColor Green
    }
}
else {
    Write-Warning "No se encontro el certificado en $CertPath."
}

Write-Host "`n============================================================" -ForegroundColor Cyan
Write-Host " 4. ESTADO DEL SERVICIO PRINCIPAL 'HSis.Server'" -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan

$service = Get-Service -Name 'HSis.Server' -ErrorAction SilentlyContinue
if ($service) {
    Write-Host "Servicio 'HSis.Server' configurado. Estado actual: $($service.Status)" -ForegroundColor Cyan
    Write-Host "Puedes iniciarlo con: Start-Service HSis.Server" -ForegroundColor Yellow
}
else {
    Write-Warning "El servicio 'HSis.Server' no esta registrado. Ejecuta primero Install-HSisServer.ps1."
}

Write-Host "`nLimpieza y saneamiento de configuraciones completado exitosamente." -ForegroundColor Green

