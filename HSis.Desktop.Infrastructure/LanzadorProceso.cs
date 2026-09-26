using System.Diagnostics;
using System.Runtime.Versioning;

namespace HSis.Desktop.Infrastructure;

[SupportedOSPlatform("windows")]
public static class LanzadorProceso
{
    public static Process? Iniciar(string rutaEjecutable, string? argumentos = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rutaEjecutable);

        var informacion = new ProcessStartInfo
        {
            FileName = rutaEjecutable,
            Arguments = argumentos ?? string.Empty,
            UseShellExecute = true,
            WorkingDirectory = Path.GetDirectoryName(rutaEjecutable) ?? AppContext.BaseDirectory
        };

        return Process.Start(informacion);
    }
}
