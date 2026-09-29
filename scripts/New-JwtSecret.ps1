#Requires -RunAsAdministrator
<#
Crea el archivo de clave JWT protegido con DPAPI (CurrentUser). Debe ejecutarse con la MISMA
cuenta Windows que ejecutara el servicio (por ejemplo: runas /user:CUENTA powershell).
#>
param([string]$SecretPath = 'C:\ProgramData\HSis\jwt-secret.dpapi')

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Security
New-Item -ItemType Directory -Force -Path (Split-Path $SecretPath) | Out-Null
$randomBytes = New-Object byte[] 64
[System.Security.Cryptography.RandomNumberGenerator]::Fill($randomBytes)
$plainBytes = [Text.Encoding]::UTF8.GetBytes([Convert]::ToBase64String($randomBytes))
$protectedBytes = [Security.Cryptography.ProtectedData]::Protect($plainBytes, $null, 'CurrentUser')
[IO.File]::WriteAllBytes($SecretPath, $protectedBytes)
[Array]::Clear($randomBytes, 0, $randomBytes.Length)
[Array]::Clear($plainBytes, 0, $plainBytes.Length)
Write-Host "Secreto creado en $SecretPath. Restringe su ACL a la cuenta del servicio y administradores."
