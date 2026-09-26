using System.Runtime.Versioning;
using Microsoft.Win32;

namespace HSis.Desktop.Infrastructure;

[SupportedOSPlatform("windows")]
public sealed class RegistroInicioAutomatico
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";

    public bool Registrar(
        string nombreAplicacion,
        string rutaEjecutable,
        string? argumentos = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombreAplicacion);
        ArgumentException.ThrowIfNullOrWhiteSpace(rutaEjecutable);

        try
        {
            using var key = Registry.CurrentUser.CreateSubKey(RunKeyPath, writable: true);
            if (key is null)
            {
                return false;
            }

            key.SetValue(
                nombreAplicacion,
                ConstruirComando(rutaEjecutable, argumentos),
                RegistryValueKind.String);
            return true;
        }
        catch (IOException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }

    public bool Eliminar(string nombreAplicacion)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombreAplicacion);

        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true);
            if (key is null)
            {
                return true;
            }

            key.DeleteValue(nombreAplicacion, throwOnMissingValue: false);
            return true;
        }
        catch (IOException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }

    public bool EstaRegistrado(string nombreAplicacion)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombreAplicacion);

        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: false);
            return key?.GetValue(nombreAplicacion) is string value &&
                   !string.IsNullOrWhiteSpace(value);
        }
        catch (IOException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }

    public string? ObtenerComando(string nombreAplicacion)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombreAplicacion);

        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: false);
            return key?.GetValue(nombreAplicacion) as string;
        }
        catch (IOException)
        {
            return null;
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
    }

    private static string ConstruirComando(string rutaEjecutable, string? argumentos)
    {
        var rutaEscapada = rutaEjecutable.Replace("\"", "\\\"", StringComparison.Ordinal);
        var comando = $"\"{rutaEscapada}\"";
        return string.IsNullOrWhiteSpace(argumentos)
            ? comando
            : $"{comando} {argumentos}";
    }
}
