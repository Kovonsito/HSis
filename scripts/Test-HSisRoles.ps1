#Requires -Version 7.0
<#
Verifica contra un servidor HSis real que cada rol solo accede a lo que le corresponde.
Solo realiza peticiones GET y el login; no crea, modifica ni elimina datos.
Usa tres cuentas existentes (Administrador, Tecnico y Cliente) que se piden con Get-Credential;
las contraseñas no se escriben en pantalla ni en disco.
Para que el recorrido de tickets sea completo, el Cliente debe tener al menos un ticket propio,
el Tecnico al menos uno asignado y deben existir tickets de otros usuarios.
-SkipCertificateCheck solo debe usarse con el certificado autofirmado de una red de confianza.
Devuelve código de salida 1 si alguna comprobación falla.
#>
param(
	[Parameter(Mandatory)][string]$BaseUrl,
	[Parameter(Mandatory)][pscredential]$Administrador,
	[Parameter(Mandatory)][pscredential]$Tecnico,
	[Parameter(Mandatory)][pscredential]$Cliente,
	[switch]$SkipCertificateCheck
)

$ErrorActionPreference = 'Stop'
$BaseUrl = $BaseUrl.TrimEnd('/')
$resultados = [System.Collections.Generic.List[object]]::new()

function Invoke-Api([string]$Ruta, [string]$Token) {
	$parametros = @{
		Uri                = "$BaseUrl/$Ruta"
		Method             = 'GET'
		SkipHttpErrorCheck = $true
		SkipCertificateCheck = $SkipCertificateCheck.IsPresent
	}
	if ($Token) { $parametros.Headers = @{ Authorization = "Bearer $Token" } }
	$respuesta = Invoke-WebRequest @parametros
	$cuerpo = $null
	if ($respuesta.StatusCode -eq 200 -and $respuesta.Content) {
		try { $cuerpo = $respuesta.Content | ConvertFrom-Json -Depth 10 } catch { }
	}
	[pscustomobject]@{ Estado = [int]$respuesta.StatusCode; Cuerpo = $cuerpo }
}

function Connect-Rol([string]$Rol, [pscredential]$Credencial) {
	$cuerpo = @{ username = $Credencial.UserName; password = $Credencial.GetNetworkCredential().Password } | ConvertTo-Json
	$respuesta = Invoke-WebRequest -Uri "$BaseUrl/api/Auth/login" -Method POST -Body $cuerpo `
		-ContentType 'application/json' -SkipHttpErrorCheck -SkipCertificateCheck:$SkipCertificateCheck
	if ([int]$respuesta.StatusCode -ne 200) {
		throw "No se pudo iniciar sesión como $Rol ($($Credencial.UserName)): HTTP $([int]$respuesta.StatusCode)."
	}
	$login = $respuesta.Content | ConvertFrom-Json
	[pscustomobject]@{ Rol = $Rol; Token = $login.token; Id = [int]$login.usuario.idUsuario; IdRol = $login.usuario.idRol }
}

function Test-Estado([string]$Rol, [string]$Token, [string]$Ruta, [int[]]$Esperado, [string]$Descripcion) {
	$real = (Invoke-Api $Ruta $Token).Estado
	$ok = $real -in $Esperado
	$resultados.Add([pscustomobject]@{
			Rol = $Rol; Comprobacion = $Descripcion; Ruta = "GET $Ruta"
			Esperado = ($Esperado -join '/'); Real = $real; Resultado = $(if ($ok) { 'OK' } else { 'FALLA' })
		})
}

function Test-Condicion([string]$Rol, [string]$Descripcion, [bool]$Cumple, [string]$Detalle = '') {
	$resultados.Add([pscustomobject]@{
			Rol = $Rol; Comprobacion = $Descripcion; Ruta = $Detalle
			Esperado = 'cumple'; Real = $(if ($Cumple) { 'cumple' } else { 'no cumple' }); Resultado = $(if ($Cumple) { 'OK' } else { 'FALLA' })
		})
}

$admin = Connect-Rol 'Administrador' $Administrador
$tec = Connect-Rol 'Tecnico' $Tecnico
$cli = Connect-Rol 'Cliente' $Cliente
if ($admin.Id -in @($tec.Id, $cli.Id) -or $tec.Id -eq $cli.Id) { throw 'Las tres cuentas deben ser usuarios distintos.' }

Test-Estado 'Anonimo' '' 'api/Tickets' 401 'Sin token se rechaza'
Test-Estado 'Anonimo' '' 'api/Catalogos/usuarios' 401 'Sin token se rechaza'

# Administrador: catalogos, usuarios, materiales, reportes y todos los tickets.
$kpis = 'api/Tickets/kpis?inicio=2026-01-01&fin=2026-12-31'
foreach ($ruta in 'api/Tickets', 'api/Catalogos/materiales', 'api/Catalogos/usuarios', 'api/Materiales', 'api/Usuarios/rol/3', $kpis, 'api/Notificaciones') {
	Test-Estado 'Administrador' $admin.Token $ruta 200 'Acceso permitido'
}

# Tecnico: materiales y tickets propios; sin catalogos, usuarios completos ni vistas globales.
foreach ($ruta in 'api/Materiales', 'api/Usuarios/rol/3', 'api/Tickets/disponibles', "api/Tickets/tecnico/$($tec.Id)/asignados", 'api/Notificaciones') {
	Test-Estado 'Tecnico' $tec.Token $ruta 200 'Acceso permitido'
}
foreach ($ruta in 'api/Tickets', 'api/Catalogos/materiales', 'api/Catalogos/usuarios', $kpis, "api/Tickets/tecnico/$($admin.Id)/asignados") {
	Test-Estado 'Tecnico' $tec.Token $ruta 403 'Acceso denegado'
}

# Cliente: solo sus tickets y notificaciones.
foreach ($ruta in "api/Tickets/usuario/$($cli.Id)", 'api/Notificaciones') {
	Test-Estado 'Cliente' $cli.Token $ruta 200 'Acceso permitido'
}
foreach ($ruta in 'api/Tickets', 'api/Tickets/disponibles', 'api/Catalogos/materiales', 'api/Catalogos/usuarios',
	'api/Materiales', 'api/Usuarios/rol/2', $kpis, "api/Tickets/usuario/$($admin.Id)") {
	Test-Estado 'Cliente' $cli.Token $ruta 403 'Acceso denegado'
}

# Datos expuestos: el tecnico solo recibe identidad de los usuarios; el administrador el detalle.
$porTecnico = @((Invoke-Api 'api/Usuarios/rol/3' $tec.Token).Cuerpo)
$sinDetalle = $porTecnico.Count -gt 0 -and -not ($porTecnico | Where-Object { $null -ne $_.idDepartamento -or $null -ne $_.rolNombre -or $_.contraseña })
Test-Condicion 'Tecnico' 'Listas de usuarios sin departamento, rol ni contraseña' $sinDetalle 'api/Usuarios/rol/3'
$usuariosAdmin = @((Invoke-Api 'api/Catalogos/usuarios' $admin.Token).Cuerpo)
$conClave = @($usuariosAdmin | Where-Object { $_.contraseña -and $_.contraseña -notmatch '^\$|^PBKDF2|^v\d' }).Count
Test-Condicion 'Administrador' 'Catalogo de usuarios no devuelve contraseñas en texto legible' ($conClave -eq 0) 'api/Catalogos/usuarios'

# Alcance por ticket.
$todos = @((Invoke-Api 'api/Tickets' $admin.Token).Cuerpo)
$propios = @((Invoke-Api "api/Tickets/usuario/$($cli.Id)" $cli.Token).Cuerpo)
if ($propios.Count -gt 0) {
	$id = $propios[0].idTicket
	Test-Estado 'Cliente' $cli.Token "api/Tickets/$id" 200 'Ve su propio ticket'
	Test-Estado 'Cliente' $cli.Token "api/Tickets/$id/historial" 403 'No ve el historial interno'
	Test-Estado 'Cliente' $cli.Token "api/TicketDetalle/ticket/$id" 403 'No ve materiales ni costos del ticket'
} else {
	Write-Warning 'El Cliente no tiene tickets propios: se omiten las comprobaciones de su ticket.'
}
$ajenoCliente = $todos | Where-Object { $_.idUsuario -ne $cli.Id } | Select-Object -First 1
if ($ajenoCliente) { Test-Estado 'Cliente' $cli.Token "api/Tickets/$($ajenoCliente.idTicket)" 403 'No ve el ticket de otro usuario' }

$asignados = @((Invoke-Api "api/Tickets/tecnico/$($tec.Id)/asignados" $tec.Token).Cuerpo)
if ($asignados.Count -gt 0) {
	$id = $asignados[0].idTicket
	Test-Estado 'Tecnico' $tec.Token "api/Tickets/$id" 200 'Ve un ticket que tiene asignado'
	Test-Estado 'Tecnico' $tec.Token "api/TicketDetalle/ticket/$id" 200 'Ve los materiales de su ticket'
} else {
	Write-Warning 'El Tecnico no tiene tickets asignados: se omiten las comprobaciones de su ticket.'
}
$deOtroTecnico = $todos | Where-Object { $null -ne $_.idTecnico -and $_.idTecnico -ne $tec.Id } | Select-Object -First 1
if ($deOtroTecnico) {
	Test-Estado 'Tecnico' $tec.Token "api/Tickets/$($deOtroTecnico.idTicket)" 403 'No ve un ticket asignado a otro tecnico'
	Test-Estado 'Tecnico' $tec.Token "api/TicketDetalle/ticket/$($deOtroTecnico.idTicket)" 403 'No ve materiales de un ticket ajeno'
} else {
	Write-Warning 'No hay tickets asignados a otro tecnico: se omiten esas comprobaciones.'
}

$resultados | Format-Table Rol, Comprobacion, Ruta, Esperado, Real, Resultado -AutoSize -Wrap
$fallas = @($resultados | Where-Object Resultado -eq 'FALLA').Count
Write-Host ("Comprobaciones: {0}. Fallas: {1}." -f $resultados.Count, $fallas)
if ($fallas -gt 0) { exit 1 }
