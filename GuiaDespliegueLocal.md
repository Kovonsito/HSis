# Guía de Despliegue y Distribución en Red Local (LAN)

Esta guía explica paso a paso cómo configurar el servidor central de tu red local y desplegar las aplicaciones cliente para que funcionen con actualizaciones automáticas y arranque desatendido.

---

## Paso 1: Configuración del Servidor de la Red Local (LAN)

El equipo que actuará como servidor albergará la base de datos SQL Server y el servicio SignalR.

### 1.1 IP Estática

Asegúrate de configurar una dirección IP estática en el servidor para evitar que cambie al reiniciar el router.

* *Ejemplo de IP recomendada:* `192.168.1.100`

### 1.2 Reglas del Firewall de Windows

Para que los clientes de la red puedan comunicarse con el servidor, debes abrir los puertos de entrada. Ejecuta los siguientes comandos en PowerShell como Administrador en el servidor:

```powershell
# Abrir puerto 5000 para el Hub de SignalR
New-NetFirewallRule -DisplayName "HSis SignalR Server" -Direction Inbound -LocalPort 5000 -Protocol TCP -Action Allow

# Abrir puerto 1433 para SQL Server (si los clientes requieren conexión directa a la BD)
New-NetFirewallRule -DisplayName "SQL Server" -Direction Inbound -LocalPort 1433 -Protocol TCP -Action Allow
```

---

## Paso 2: Registrar SignalR como Servicio de Windows

Para que el servidor de notificaciones SignalR (`HSis.Server`) arranque solo cuando el equipo se encienda y funcione sin necesidad de iniciar sesión:

### 2.1 Publicar la Aplicación

Compila el proyecto en modo Autocontenido para generar un único archivo ejecutable:

1. Abre la terminal en la carpeta raíz del proyecto.
2. Ejecuta el comando:

   ```bash
   dotnet publish HSis.Server\HSis.Server.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o C:\Despliegue\HSisServer
   ```

3. Esto creará el archivo `HSis.Server.exe` en `C:\Despliegue\HSisServer`.

### 2.2 Registrar en Windows

Abre PowerShell como **Administrador** en el servidor y ejecuta el siguiente comando para registrar el servicio:

```powershell
New-Service -Name "HSisSignalR" -BinaryPathName "C:\Despliegue\HSisServer\HSis.Server.exe" -DisplayName "HSis SignalR Notification Service" -StartupType Automatic
```

* **Para iniciar el servicio inmediatamente:**

  ```powershell
  Start-Service -Name "HSisSignalR"
  ```

* **Para verificar su estado:**

  Puedes abrir el Administrador de Servicios de Windows (`services.msc`) y confirmar que "HSis SignalR Notification Service" está en estado **Ejecución** y su inicio es **Automático**.

---

## Paso 3: Configurar appsettings.json del Cliente

Antes de compilar y distribuir la aplicación de escritorio clientes, asegúrate de configurar la IP del servidor en el archivo `appsettings.json` del proyecto `HSis.UI`:

```json
{
    "ConnectionStrings": {
        "CadenaSQL": "Server=192.168.1.100\\SQLEXPRESS; Initial Catalog=HSIS; User ID=sa; Password=12345; TrustServerCertificate=True;"
    },
    "SignalR": {
        "ServerUrl": "http://192.168.1.100:5000/notificationHub"
    }
}
```

---

## Paso 4: Despliegue de Clientes mediante ClickOnce (LAN)

ClickOnce permite publicar el sistema en una carpeta compartida del servidor local para que los clientes lo descarguen, instalen y se actualicen automáticamente al abrir la app.

### 4.1 Crear la Carpeta Compartida en el Servidor

1. Crea una carpeta en el servidor llamada `C:\HSisInstalador`.
2. Haz clic derecho en la carpeta -> **Propiedades** -> pestaña **Compartir** -> botón **Compartir...**
3. Añade a "Todos" (o usuarios de red autorizados) y dale permisos de **Lectura y Escritura**.
4. La ruta de red de la carpeta será algo como: `\\192.168.1.100\HSisInstalador`.

### 4.2 Publicar desde Visual Studio

1. Haz clic derecho en el proyecto **HSis.UI** -> **Publicar (Publish)**.
2. Selecciona **Carpeta (Folder)** o **ClickOnce** si la opción está explícita.
3. Configura los siguientes parámetros:

   * **Ruta de publicación (Publishing Folder):** `C:\HSisInstalador` (o la ruta local en la que quieras compilar).
   * **Ruta de instalación (Installation Folder):** Selecciona *Ruta UNC o recurso compartido* y escribe `\\192.168.1.100\HSisInstalador`.
   * **Configuración de actualizaciones:** Activa *"La aplicación debe buscar actualizaciones"* y selecciona *"Antes de iniciar la aplicación"*.

4. Haz clic en **Publicar**.

### 4.3 Instalación en los Equipos Clientes

Para instalar el sistema en cualquier equipo de la red local:

1. Desde el equipo del usuario, abre el Explorador de Archivos de Windows.
2. Accede a la ruta compartida: `\\192.168.1.100\HSisInstalador`.
3. Haz doble clic en `setup.exe`.
4. El sistema se instalará automáticamente y agregará un acceso directo en el escritorio.

Cada vez que publiques una nueva versión del sistema en la carpeta compartida, las aplicaciones de los usuarios se actualizarán de forma transparente en el siguiente inicio.
