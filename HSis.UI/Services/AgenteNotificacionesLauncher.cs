using System.Runtime.Versioning;
using System.IO;
using HSis.Desktop.Infrastructure;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace HSis.UI.Services;

[SupportedOSPlatform("windows")]
public sealed class AgenteNotificacionesLauncher(IConfiguration configuration)
{
    private readonly IConfiguration _configuration = configuration;

    public void Iniciar()
    {
        if (!_configuration.GetValue("NotificationAgent:LaunchOnLogin", true))
        {
            return;
        }

        var executablePath = ObtenerRutaEjecutable();
        if (string.IsNullOrWhiteSpace(executablePath) || !File.Exists(executablePath))
        {
            Log.Warning(
                "No se inició el agente de notificaciones porque no existe la ruta configurada: {ExecutablePath}.",
                executablePath);
            return;
        }

        try
        {
            var registro = new RegistroInicioAutomatico();
            if (_configuration.GetValue("NotificationAgent:RegisterStartup", true) &&
                !registro.Registrar(ConfiguracionEscritorio.NombreInicioAgente, executablePath))
            {
                Log.Warning("No se pudo registrar el inicio automático del agente de notificaciones.");
            }

            // Verificar si el agente ya se encuentra en ejecución para evitar instancias duplicadas
            var yaEjecutandose = false;
            try
            {
                if (System.Threading.Mutex.TryOpenExisting(ConfiguracionEscritorio.MutexAgenteNotificaciones, out var existingMutex))
                {
                    existingMutex.Dispose();
                    yaEjecutandose = true;
                }
            }
            catch
            {
                // Si no se puede abrir el mutex, verificamos por nombre de proceso
            }

            if (!yaEjecutandose)
            {
                yaEjecutandose = System.Diagnostics.Process.GetProcessesByName(
                    Path.GetFileNameWithoutExtension(executablePath)).Length > 0;
            }

            if (yaEjecutandose)
            {
                Log.Information("El agente de notificaciones ya está activo. No se inicia una nueva instancia.");
                return;
            }

            string? argumentos = null;
            if (System.Diagnostics.Debugger.IsAttached || 
                string.Equals(Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT"), "Development", StringComparison.OrdinalIgnoreCase))
            {
                argumentos = $"--parent-pid {Environment.ProcessId}";
            }

            LanzadorProceso.Iniciar(executablePath, argumentos);
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "No se pudo iniciar el agente de notificaciones.");
        }
    }

    private string? ObtenerRutaEjecutable()
    {
        var configuredPath = _configuration["NotificationAgent:ExecutablePath"];
        if (!string.IsNullOrWhiteSpace(configuredPath))
        {
            return Path.GetFullPath(configuredPath, AppContext.BaseDirectory);
        }

        var defaultPath = Path.Combine(
            AppContext.BaseDirectory,
            "HSis.NotificationAgent.exe");
        return File.Exists(defaultPath) ? defaultPath : null;
    }
}
