#nullable enable

namespace HSis.Contracts.DTOs;

/// <summary>
/// Registro de catálogo independiente de las entidades de persistencia.
/// Las claves coinciden con las propiedades serializadas por la API.
/// </summary>
public sealed class CatalogoRegistroDto
{
    public Dictionary<string, object?> Valores { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public object? Obtener(string propiedad)
    {
        return Valores.TryGetValue(propiedad, out var valor) ? valor : null;
    }
}
