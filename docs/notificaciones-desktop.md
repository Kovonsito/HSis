# Despliegue de la UI y del agente de notificaciones

## Componentes

La solución se distribuye con dos ejecutables Windows independientes:

- `HSis.UI.exe`: aplicación WinForms y destino de las activaciones de tickets.
- `HSis.NotificationAgent.exe`: proceso residente que recibe SignalR, sondea REST y muestra Toasts nativos.

El agente usa Windows App SDK 2.5.1 en modo no empaquetado y autocontenido. El perfil de publicación incluye el runtime de Windows App SDK junto con el agente, por lo que no se necesita instalarlo por separado en el equipo destino.

## Publicación

Publicar cada ejecutable por separado desde la raíz del repositorio:

```powershell
dotnet publish .\HSis.UI\HSis.UI.csproj -c Release -p:PublishProfile=win-x64
dotnet publish .\HSis.NotificationAgent\HSis.NotificationAgent.csproj -c Release -p:PublishProfile=win-x64
```

Los perfiles están en:

- `HSis.UI/Properties/PublishProfiles/win-x64.pubxml`
- `HSis.NotificationAgent/Properties/PublishProfiles/win-x64.pubxml`

El resultado se genera en el directorio `bin/publish/win-x64` de cada proyecto. No se debe publicar la solución completa como una única aplicación: el agente y la UI tienen ciclos de vida distintos.

## Estructura instalada recomendada

Copiar los contenidos publicados a una estructura estable, por ejemplo:

```text
C:\Program Files\HSis\
  UI\
	HSis.UI.exe
	appsettings.json
	...
  Agent\
	HSis.NotificationAgent.exe
	appsettings.json
	...
```

En `UI/appsettings.json`, configurar la ruta relativa del agente:

```json
{
  "NotificationAgent": {
	"ExecutablePath": "..\\Agent\\HSis.NotificationAgent.exe",
	"LaunchOnLogin": true,
	"RegisterStartup": true
  }
}
```

En `Agent/appsettings.json`, configurar la ruta relativa de la UI y el mismo servidor de API/SignalR:

```json
{
  "ApiSettings": {
	"BaseUrl": "https://servidor-hsis"
  },
  "SignalR": {
	"ServerUrl": "https://servidor-hsis/notificationHub"
  },
  "NotificationAgent": {
	"UIExecutablePath": "..\\UI\\HSis.UI.exe",
	"RegisterStartup": true,
	"EnableNativeNotifications": true
  }
}
```

Las rutas relativas se resuelven desde el directorio del ejecutable correspondiente. Si el instalador usa otra estructura, se pueden sustituir por rutas absolutas administradas por el instalador.

## Inicio automático y sesión

1. La UI guarda las credenciales cifradas con DPAPI en el perfil del usuario actual después de un login correcto.
2. La UI registra y lanza el agente después del login manual o automático.
3. El agente registra `HSis.NotificationAgent` en `HKCU\Software\Microsoft\Windows\CurrentVersion\Run` de forma idempotente.
4. En el siguiente inicio de Windows, el agente arranca aunque la UI no esté abierta. Si todavía no existe una sesión DPAPI, permanece residente y reintenta hasta que la UI complete el login.
5. El mutex `Local\\HSis.NotificationAgent` evita que se ejecuten dos agentes en la misma sesión de Windows.
6. El mutex `Local\\HSis.UI` y la tubería `HSis.UI.Activation` evitan abrir una segunda UI cuando se activa un Toast.

El registro de inicio automático es por usuario y no requiere privilegios de administrador. El instalador debe escribir los archivos en una ubicación con permisos de lectura y ejecución para el usuario que ejecutará HSis.

## Flujo de Toast

- Si la UI está abierta, el agente envía `--open-ticket <id>` por la tubería local y la instancia existente abre el formulario correspondiente al rol.
- Si la UI no está abierta, el agente inicia `HSis.UI.exe --open-ticket <id>`.
- Los clientes abren `DetalleClienteForm`; los técnicos y administradores abren `TicketDetalleForm`.
- La UI solo procesa la solicitud después de autenticar al usuario y activar su presencia.

## Validación posterior a la instalación

1. Ejecutar `HSis.UI.exe` y completar un login.
2. Comprobar que aparece `HSis.NotificationAgent.exe` en el Administrador de tareas.
3. Comprobar que la entrada `HKCU\Software\Microsoft\Windows\CurrentVersion\Run\HSis.NotificationAgent` apunta al ejecutable instalado.
4. Cerrar la UI sin cerrar sesión y generar una notificación para el usuario; el agente debe mostrar un Toast.
5. Activar el Toast y comprobar que se reutiliza la UI existente o se inicia una sola UI si estaba cerrada.
6. Reiniciar Windows y comprobar que el agente permanece esperando si no hay credenciales, sin terminar el proceso.
7. Revisar los logs del agente y confirmar que el API y SignalR usan el mismo host configurado.

## Requisitos y límites

- Windows x64 y Windows 10 versión 2004, compilación 19041 o posterior, conforme al TFM del agente.
- Firmar los ejecutables antes de distribuirlos para evitar advertencias de SmartScreen.
- No almacenar tokens ni contraseñas en `appsettings.json`, argumentos de proceso o el registro. Las credenciales se transportan mediante el archivo DPAPI de la sesión del usuario.
- Si se opta por publicación dependiente del framework en lugar del perfil autocontenido, hay que instalar y mantener el runtime de Windows App SDK en el equipo destino.

Referencias oficiales:

- [Despliegue autocontenido de Windows App SDK](https://learn.microsoft.com/windows/apps/package-and-deploy/self-contained-deploy/deploy-self-contained-apps)
- [Distribución de aplicaciones Windows App SDK no empaquetadas](https://learn.microsoft.com/windows/apps/package-and-deploy/unpackage-winui-app)
