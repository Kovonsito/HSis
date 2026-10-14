#Requires -RunAsAdministrator
<#
Registra HSis.Server como servicio de Windows. No crea cuentas, certificados ni reglas de firewall.
Ejecutar tras publicar y tras crear el secreto DPAPI con la cuenta del servicio (docs\configuracion-segura.md).
#>
param(
	[Parameter(Mandatory)][string]$PublishPath,
	[pscredential]$ServiceCredential,
	[Parameter(Mandatory)][string]$DnsName,
	[Parameter(Mandatory)][string]$CertificateThumbprint,
	[Parameter(Mandatory)][string]$ProtectedSecretPath,
	[Parameter(Mandatory)][string]$SqlConnectionString,
	[int]$HttpsPort = 443
)

$ErrorActionPreference = 'Stop'
$serviceName = 'HSis.Server'
$exe = Join-Path $PublishPath 'HSis.Server.exe'
if (-not (Test-Path $exe)) { throw "No existe $exe. Publica el servidor primero." }
if ($SqlConnectionString -notmatch 'Integrated Security\s*=\s*(True|SSPI)') { throw 'La cadena SQL debe usar Integrated Security=True.' }

if (-not (Get-Service $serviceName -ErrorAction SilentlyContinue)) {
	if (-not $ServiceCredential) { throw "El servicio no existe aún. Debe proporcionar el parámetro -ServiceCredential para crearlo." }
	New-Service -Name $serviceName -BinaryPathName "`"$exe`"" -DisplayName 'HSis Server' `
		-StartupType Automatic -Credential $ServiceCredential | Out-Null
}
else {
	# Reejecutar el script actualiza ruta, inicio y opcionalmente credenciales
	$cim = Get-CimInstance Win32_Service -Filter "Name='$serviceName'"
	$argsChange = @{
		PathName  = "`"$exe`""
		StartMode = 'Automatic'
	}
	if ($ServiceCredential) {
		$userName = $ServiceCredential.UserName
		if ($userName -notmatch '\\') { $userName = ".\$userName" }
		$argsChange.StartName = $userName
		$argsChange.StartPassword = $ServiceCredential.GetNetworkCredential().Password
	}
	$resultado = Invoke-CimMethod -InputObject $cim -MethodName Change -Arguments $argsChange
	if ($resultado.ReturnValue -ne 0) { throw "No se pudo actualizar el servicio $serviceName (código $($resultado.ReturnValue))." }
}

# Las variables se guardan en el registro del servicio, no en archivos del repositorio.
$environment = @(
	'ASPNETCORE_ENVIRONMENT=Production',
	"AllowedHosts=$DnsName",
	"Kestrel__HttpsPort=$HttpsPort",
	"Kestrel__CertificateThumbprint=$CertificateThumbprint",
	"JwtSettings__ProtectedSecretPath=$ProtectedSecretPath",
	"ConnectionStrings__CadenaSQL=$SqlConnectionString"
)
Set-ItemProperty -Path "HKLM:\SYSTEM\CurrentControlSet\Services\$serviceName" -Name Environment -Value $environment -Type MultiString

sc.exe failure $serviceName reset= 86400 actions= restart/5000/restart/5000/restart/30000 | Out-Null
Write-Host "Servicio $serviceName configurado. Si ya estaba en ejecución, reinícialo con: Restart-Service $serviceName"
