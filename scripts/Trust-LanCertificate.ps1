#Requires -RunAsAdministrator
# Marca como confiable el certificado publico del servidor HSis en este PC cliente.
param([Parameter(Mandatory)][string]$CertificatePath)

$ErrorActionPreference = 'Stop'
Import-Certificate -FilePath $CertificatePath -CertStoreLocation Cert:\LocalMachine\Root | Out-Null
Write-Host 'Certificado instalado como confiable.'
