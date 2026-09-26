using System.Runtime.Versioning;

namespace HSis.Desktop.Infrastructure;

[SupportedOSPlatform("windows")]
public static class ConfiguracionEscritorio
{
    public const string NombreAplicacion = "HSis";
    public const string ArchivoCredenciales = "session.bin";
    public const string ArchivoPresenciaUi = "ui-presence.json";
    public const string MutexPresenciaUi = "Local\\HSis.UI.Presence";
    public const string MutexUi = "Local\\HSis.UI";
    public const string MutexAgenteNotificaciones = "Local\\HSis.NotificationAgent";
    public const string CanalActivacionUi = "HSis.UI.Activation";
    public const string NombreInicioAgente = "HSis.NotificationAgent";

    public static string DirectorioDatos => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        NombreAplicacion);

    public static string ObtenerRutaDatos(string nombreArchivo)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombreArchivo);
        return Path.Combine(DirectorioDatos, nombreArchivo);
    }
}
