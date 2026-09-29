#Requires -RunAsAdministrator
<#
Crea un certificado TLS autofirmado para uso en red local, lo instala en LocalMachine\My
y exporta la parte publica (.cer) para distribuirla a los PC cliente.
Uso: .\New-LanCertificate.ps1 -ServerAddress 192.168.34.107 [-ServerName HSISPC] [-ServiceAccount 'DOMINIO\cuenta']
#>
param(
    [Parameter(Mandatory)][string]$ServerAddress,
    [string]$ServerName = $env:COMPUTERNAME,
    [string]$ServiceAccount,
    [string]$OutputPath = (Join-Path $PSScriptRoot 'lan-cert')
)

$ErrorActionPreference = 'Stop'
$cert = New-SelfSignedCertificate -Subject "CN=$ServerAddress" `
    -TextExtension @("2.5.29.17={text}IPAddress=$ServerAddress&IPAddress=127.0.0.1&DNS=$ServerName&DNS=localhost") `
    -KeyAlgorithm RSA -KeyLength 2048 -HashAlgorithm SHA256 -KeyExportPolicy NonExportable `
    -KeyUsage DigitalSignature, KeyEncipherment -CertStoreLocation Cert:\LocalMachine\My `
    -NotAfter (Get-Date).AddYears(2)

New-Item -ItemType Directory -Force -Path $OutputPath | Out-Null
$cerPath = Join-Path $OutputPath 'hsis-lan.cer'
Export-Certificate -Cert $cert -FilePath $cerPath | Out-Null

# Kestrel corre bajo la cuenta del servicio y necesita leer la clave privada.
if ($ServiceAccount) {
    $keyName = [System.Security.Cryptography.X509Certificates.RSACertificateExtensions]::GetRSAPrivateKey($cert).Key.UniqueName
    $keyFile = @("$env:ProgramData\Microsoft\Crypto\Keys", "$env:ProgramData\Microsoft\Crypto\RSA\MachineKeys") |
        ForEach-Object { Join-Path $_ $keyName } | Where-Object { Test-Path $_ } | Select-Object -First 1
    if (-not $keyFile) { throw "No se encontro el archivo de clave privada $keyName." }
    $acl = Get-Acl $keyFile
    $acl.AddAccessRule((New-Object Security.AccessControl.FileSystemAccessRule($ServiceAccount, 'Read', 'Allow')))
    Set-Acl $keyFile $acl
}

Write-Host "Huella (Kestrel__CertificateThumbprint): $($cert.Thumbprint)"
Write-Host "Copia $cerPath a cada PC cliente y ejecuta Trust-LanCertificate.ps1 como administrador."
