; Script de Inno Setup para HSis Client (UI + Agente de Notificaciones)
; Permite empaquetar en un único instalador con cierre automático de versiones previas,
; instalación silenciosa del certificado SSL LAN con certutil, y reinicio del agente.

#define MyAppName "HSis"
#define MyAppVersion "1.0.5.0"
#define MyAppPublisher "HSis"
#define MyAppExeName "HSis.UI.exe"
#define MyAgentExeName "HSis.NotificationAgent.exe"
#define SourceClientPkg "C:\HSis\publish\ClientPackage"

[Setup]
AppId={{B528A6B3-57A1-47BD-9F02-4A7B5F09B1A1}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\HSis
DefaultGroupName={#MyAppName}
OutputDir=C:\HSis\publish\Installer
OutputBaseFilename=Setup_HSis_Cliente
Compression=lzma2/ultra64
SolidCompression=yes
; Requiere privilegios de administrador para escribir en Program Files y registrar el certificado en LocalMachine\Root
PrivilegesRequired=admin
PrivilegesRequiredOverridesAllowed=dialog
; Cierra automáticamente HSis.UI y el Agente si están corriendo para evitar archivos bloqueados
CloseApplications=yes
CloseApplicationsFilter=*.exe
RestartApplications=no

[Code]
function PrepareToInstall(var NeedsRestart: Boolean): String;
var
  ResultCode: Integer;
begin
  Result := '';
  // Forzar el cierre de procesos existentes para evitar archivos bloqueados (in use)
  Exec('taskkill.exe', '/F /IM {#MyAgentExeName} /IM {#MyAppExeName}', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  Sleep(500);
end;

[Languages]
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"
Name: "startupagent"; Description: "Iniciar agente de notificaciones con Windows"; GroupDescription: "Segundo plano:"; Flags: checkedonce

[Files]
; Archivos de la interfaz de usuario (UI)
Source: "{#SourceClientPkg}\UI\*"; DestDir: "{app}\UI"; Flags: ignoreversion recursesubdirs createallsubdirs
; Archivos del Agente de Notificaciones
Source: "{#SourceClientPkg}\Agent\*"; DestDir: "{app}\Agent"; Flags: ignoreversion recursesubdirs createallsubdirs
; Certificado SSL LAN del servidor para confianza HTTPS
Source: "{#SourceClientPkg}\hsis-lan.cer"; DestDir: "{app}\Cert"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\UI\{#MyAppExeName}"
Name: "{group}\Desinstalar {#MyAppName}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\UI\{#MyAppExeName}"; Tasks: desktopicon

[Registry]
; Registrar inicio automático del agente con Windows si la tarea está marcada
Root: HKLM; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "HSis.NotificationAgent"; ValueData: """{app}\Agent\{#MyAgentExeName}"""; Tasks: startupagent; Flags: uninsdeletevalue

[Run]
; 1. Instalar el certificado de seguridad en la raíz de certificados de confianza de la máquina (Root)
Filename: "certutil.exe"; Parameters: "-addstore -f ""Root"" ""{app}\Cert\hsis-lan.cer"""; \
    StatusMsg: "Configurando certificado de confianza HTTPS para la red local..."; \
    Flags: runhidden waituntilterminated

; 2. Iniciar el Agente de Notificaciones en segundo plano tras instalar
Filename: "{app}\Agent\{#MyAgentExeName}"; \
    Description: "Iniciar agente de notificaciones"; \
    Flags: nowait postinstall skipifsilent runhidden

; 3. Opción para lanzar la aplicación principal inmediatamente
Filename: "{app}\UI\{#MyAppExeName}"; \
    Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; \
    Flags: nowait postinstall skipifsilent

[UninstallRun]
; Al desinstalar, detener procesos si están corriendo
Filename: "taskkill.exe"; Parameters: "/F /IM {#MyAgentExeName} /IM {#MyAppExeName}"; Flags: runhidden; RunOnceId: "TerminarProcesosHSis"
