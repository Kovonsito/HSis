# Configuración segura del servidor

`HSis.Server/appsettings.json` contiene solo configuración no sensible. No se deben versionar cadenas de conexión, contraseñas, claves JWT ni tokens.

## Desarrollo local

El proyecto utiliza .NET User Secrets. Desde la raíz del repositorio, configura valores privados para el perfil del desarrollador:

```powershell
dotnet user-secrets set "ConnectionStrings:CadenaSQL" "<cadena de conexión local>" --project .\HSis.Server\HSis.Server.csproj
dotnet user-secrets set "JwtSettings:SecretKey" "<clave aleatoria de al menos 64 bytes>" --project .\HSis.Server\HSis.Server.csproj
```

Genera una clave nueva con un generador criptográficamente seguro. No reutilices claves mostradas previamente o almacenadas en el historial Git. Configura el usuario SQL local con los permisos mínimos necesarios.

## Despliegue

Inyecta los secretos mediante el gestor de secretos del entorno o variables de entorno protegidas:

- `ConnectionStrings__CadenaSQL`
- `JwtSettings__SecretKey`

Configura además `JwtSettings__Issuer`, `JwtSettings__Audience` y las URL/orígenes permitidos conforme al entorno. No almacenes los valores reales en archivos versionados, scripts de despliegue públicos ni logs.

## CORS

Los clientes WinForms no requieren CORS cuando no envían el encabezado `Origin`. Si se habilita un cliente web, declara únicamente su origen en la configuración protegida del entorno, por ejemplo `Cors__AllowedOrigins__0=https://cliente.ejemplo.local`. No uses comodines ni reflejes el origen entrante junto con credenciales.

## HTTPS

La configuración de los clientes de escritorio y del agente debe migrarse de forma coordinada a URLs HTTPS junto con la provisión de un certificado válido en Kestrel o en un proxy inverso de confianza. No expongas el listener HTTP actual a redes no confiables. Si se termina TLS en un proxy, configura únicamente proxies conocidos antes de confiar en encabezados reenviados.

## Credenciales que hayan sido versionadas

Considera comprometidos los secretos publicados en el repositorio. Rota la contraseña de base de datos y la clave JWT; usa una cuenta SQL dedicada con privilegios mínimos y revoca las credenciales anteriores. La rotación debe completarse antes de volver a desplegar con esos servicios.

La limpieza del historial requiere una copia de seguridad verificada y coordinación con todos los colaboradores. Después de la rotación, el propietario del repositorio debe purgar los valores del historial Git, coordinar el `force push` y pedir a los colaboradores que reclonen o resincronicen. Eliminar el valor del último commit no lo elimina del historial remoto.
