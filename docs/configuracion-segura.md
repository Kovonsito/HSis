# Configuración segura del servidor

`HSis.Server/appsettings.json` contiene solo configuración no sensible. No se deben versionar cadenas de conexión, contraseñas, claves JWT ni tokens.

## Desarrollo local

El proyecto utiliza .NET User Secrets. Desde la raíz del repositorio, configura valores privados para el perfil del desarrollador:

```powershell
dotnet user-secrets set "ConnectionStrings:CadenaSQL" "<cadena de conexión local>" --project .\HSis.Server\HSis.Server.csproj
dotnet user-secrets set "JwtSettings:SecretKey" "<clave aleatoria de al menos 64 bytes>" --project .\HSis.Server\HSis.Server.csproj
```

Genera una clave nueva con un generador criptográficamente seguro. No reutilices claves mostradas previamente o almacenadas en el historial Git. Configura el usuario SQL local con los permisos mínimos necesarios.

## Producción en Windows 11 Pro

`HSis.Server` se ejecuta como servicio de Windows y Kestrel termina TLS directamente. No se configura IIS ni un proxy inverso. El nombre DNS público, el puerto HTTPS y el certificado deben coincidir; el router/firewall debe dirigir el tráfico web al equipo y no debe exponer SQL Server.

1. Instala el runtime ASP.NET Core 10 si publicas dependiente del framework y publica el servidor: `dotnet publish .\HSis.Server\HSis.Server.csproj -c Release -o C:\HSis\publish\Server`.
2. Usa un nombre DNS público y un certificado TLS válido para ese nombre. Instala el certificado con clave privada en `Cert:\LocalMachine\My` y concede a la cuenta del servicio permiso de lectura sobre su clave privada.
3. Crea una cuenta Windows dedicada para el servicio. Registra `HSis.Server.exe` como servicio con esa cuenta y configura la recuperación ante fallos desde el administrador de servicios. No ejecutes también una instancia interactiva de producción.
4. Crea fuera de la carpeta publicada un archivo con la clave JWT cifrada con DPAPI `CurrentUser`. La operación de protección debe ejecutarse bajo la misma cuenta Windows que ejecutará el servicio. Configura `JwtSettings__ProtectedSecretPath` con su ruta absoluta y limita la ACL del archivo y su directorio a esa cuenta y a administradores.
5. Configura externamente `AllowedHosts` con el DNS real (sin `*`), `Kestrel__HttpsPort` (443 si corresponde) y `Kestrel__CertificateThumbprint` con la huella del certificado instalado. El valor versionado `localhost;127.0.0.1` es solo para desarrollo; producción falla al iniciar si no se reemplaza.
6. Configura `ConnectionStrings__CadenaSQL` con el servidor local, la base de datos y `Integrated Security=True`. En SQL Server crea un login para la cuenta Windows del servicio y otorga solo los permisos requeridos por la aplicación. Producción exige autenticación integrada, cifrado y validación del certificado SQL (`TrustServerCertificate=False`).
7. Autoriza únicamente el puerto HTTPS en Windows Firewall/router; no publiques el puerto SQL. Si el proveedor de Internet usa CGNAT, se necesitará una opción de publicación compatible antes de poder ofrecer acceso público.
8. Publica HSis.UI y HSis.NotificationAgent con `ApiSettings:BaseUrl` y `SignalR:ServerUrl` apuntando al mismo `https://<DNS-real>`; no desactives la validación TLS en clientes.
9. Prueba desde una red externa: TLS y DNS, API, autenticación JWT, conexión SQL integrada, SignalR/WebSockets y reinicio automático del servicio. Mantén Swagger limitado a Development.

Puedes usar `scripts\Install-HSisServer.ps1` (como administrador) para registrar el servicio, sus variables de entorno y la recuperación ante fallos; no crea la cuenta, el certificado, el secreto DPAPI ni reglas de firewall.

El servicio Windows solo cambia

### Crear o rotar el secreto DPAPI

Después de crear la cuenta del servicio, ejecuta PowerShell bajo esa identidad para crear el archivo protegido. El ejemplo genera una nueva clave de 64 bytes y no la muestra ni la escribe en claro. Sustituye la ruta por una ubicación fuera de la publicación y guarda la misma ruta como `JwtSettings__ProtectedSecretPath` para el servicio:

```powershell
$secretPath = 'C:\ProgramData\HSis\jwt-secret.dpapi'
New-Item -ItemType Directory -Force -Path (Split-Path $secretPath) | Out-Null
$random = [System.Security.Cryptography.RandomNumberGenerator]::Create()
$randomBytes = New-Object byte[] 64
$random.GetBytes($randomBytes)
$secretText = [Convert]::ToBase64String($randomBytes)
$plainBytes = [Text.Encoding]::UTF8.GetBytes($secretText)
$protectedBytes = [Security.Cryptography.ProtectedData]::Protect($plainBytes, $null, [Security.Cryptography.DataProtectionScope]::CurrentUser)
[IO.File]::WriteAllBytes($secretPath, $protectedBytes)
[Array]::Clear($randomBytes, 0, $randomBytes.Length)
[Array]::Clear($plainBytes, 0, $plainBytes.Length)
$random.Dispose()
```

Concede acceso de lectura al archivo únicamente a la cuenta del servicio y a administradores; elimina herencia/permisos para usuarios no autorizados. Para rotar la clave, genera un archivo nuevo con una sesión bajo la misma identidad, sustituye el archivo de forma controlada y reinicia el servicio. La rotación invalida JWT existentes, que deberán renovarse iniciando sesión de nuevo. Mantén protegida la identidad Windows: DPAPI `CurrentUser` no permite descifrar el archivo desde otra cuenta.

## Producción solo en red local (sin DNS ni acceso externo)

No requiere abrir puertos en el router ni un dominio. Usa la IP fija (o reservada por DHCP) del servidor en la LAN:

1. Publica el servidor (`dotnet publish .\HSis.Server -c Release -o C:\HSis\publish\Server`).
2. Como administrador: `.\scripts\New-LanCertificate.ps1 -ServerAddress <IP-LAN> -ServiceAccount '<cuenta>'`. Anota la huella que muestra.
3. Con la cuenta del servicio: `.\scripts\New-JwtSecret.ps1`.
4. Como administrador: `.\scripts\Install-HSisServer.ps1 -PublishPath ... -ServiceCredential (Get-Credential) -DnsName <IP-LAN> -CertificateThumbprint <huella> -ProtectedSecretPath C:\ProgramData\HSis\jwt-secret.dpapi -SqlConnectionString "Server=...;Database=...;Integrated Security=True"`.
5. Como administrador: Ejecuta `.\scripts\Setup-LanServer.ps1 -ServerAddress <IP-LAN>` para abrir el puerto 443 en el Firewall (subred local), confiar en el certificado localmente, iniciar el servicio `HSis.Server` y validar la conectividad HTTPS.
6. Publica y empaqueta las aplicaciones cliente ejecutando `.\scripts\Publish-LanClients.ps1 -ServerAddress <IP-LAN>`. Esto generará la carpeta `C:\HSis\publish\ClientPackage\` con `UI\`, `Agent\`, el certificado `hsis-lan.cer` y el script de instalación para puestos cliente `Instalar-Certificado-Cliente.ps1`.
7. En cada PC cliente: Copia la carpeta `ClientPackage`, ejecuta como administrador `Instalar-Certificado-Cliente.ps1` e inicia `UI\HSis.UI.exe`.

SQL Server en producción exige `Encrypt=True` y certificado confiable: si SQL usa su certificado autofirmado por defecto, instala ese certificado como confiable en el servidor o asígnale uno válido para el nombre de la cadena.

Desarrollo en LAN: el perfil `http` escucha en `0.0.0.0:5000`; otros equipos usan `http://<IP-LAN>:5000` en su `appsettings.Development.json`. `HSis.Server/appsettings.Development.json` permite `AllowedHosts=*` para admitir solicitudes LAN en desarrollo. Es solo para desarrollo, sin cifrado.

## Configuración local protegida

No guardes secretos ni archivos cifrados en el repositorio o la carpeta publicada. `JwtSettings:ProtectedSecretPath` debe apuntar a un archivo binario generado con `ProtectedData.Protect` y `DataProtectionScope.CurrentUser`; el servicio descifra el archivo como su identidad de ejecución. DPAPI `CurrentUser` no permite trasladar el archivo a otra cuenta/máquina sin volver a protegerlo bajo la identidad destino.

En producción se configuran externamente `JwtSettings__ProtectedSecretPath`, `ConnectionStrings__CadenaSQL`, `Kestrel__HttpsPort`, `Kestrel__CertificateThumbprint` y `AllowedHosts`, además de `JwtSettings__Issuer`/`Audience` si difieren de los valores versionados. No almacenes valores reales en scripts públicos, logs ni archivos versionados. En desarrollo continúa usando User Secrets; son específicos del usuario y no sustituyen el mecanismo productivo.

## CORS

Los clientes WinForms no requieren CORS cuando no envían el encabezado `Origin`. Si se habilita un cliente web, declara únicamente su origen en la configuración protegida del entorno, por ejemplo `Cors__AllowedOrigins__0=https://cliente.ejemplo.local`. No uses comodines ni reflejes el origen entrante junto con credenciales.

## HTTPS

Kestrel sirve HTTPS directamente en producción y no se confía en encabezados reenviados. Los certificados de desarrollo de localhost no son válidos para los clientes externos; estos deben conectarse al DNS cubierto por el certificado público.

## Credenciales que hayan sido versionadas

Considera comprometidos los secretos publicados en el repositorio. Rota la clave JWT y cualquier credencial SQL anterior; producción usa autenticación integrada en vez de login SQL con contraseña. La rotación debe completarse antes del despliegue.

La limpieza del historial requiere una copia de seguridad verificada y coordinación con todos los colaboradores. Después de la rotación, el propietario del repositorio debe purgar los valores del historial Git, coordinar el `force push` y pedir a los colaboradores que reclonen o resincronicen. Eliminar el valor del último commit no lo elimina del historial remoto.
