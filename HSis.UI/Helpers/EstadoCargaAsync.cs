#nullable enable

namespace HSis.UI.Helpers;

public sealed class EstadoCargaAsync
{
    private readonly object _sincronizacion = new();
    private readonly HashSet<string> _operacionesActivas = new(StringComparer.Ordinal);

    public bool EstaCargando
    {
        get
        {
            lock (_sincronizacion)
            {
                return _operacionesActivas.Count > 0;
            }
        }
    }

    public bool EstaActiva(string claveOperacion = "general")
    {
        var clave = NormalizarClave(claveOperacion);
        lock (_sincronizacion)
        {
            return _operacionesActivas.Contains(clave);
        }
    }

    public bool IntentarIniciar(string claveOperacion = "general")
    {
        var clave = NormalizarClave(claveOperacion);
        lock (_sincronizacion)
        {
            return _operacionesActivas.Add(clave);
        }
    }

    public void Finalizar(string claveOperacion = "general")
    {
        var clave = NormalizarClave(claveOperacion);
        lock (_sincronizacion)
        {
            _operacionesActivas.Remove(clave);
        }
    }

    private static string NormalizarClave(string? claveOperacion)
        => string.IsNullOrWhiteSpace(claveOperacion) ? "general" : claveOperacion.Trim();
}
