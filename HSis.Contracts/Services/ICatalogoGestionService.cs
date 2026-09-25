#nullable enable
using HSis.Contracts.DTOs;

namespace HSis.Contracts.Services;

/// <summary>
/// Operaciones de catálogo que no exponen tipos de EF a la interfaz de usuario.
/// </summary>
public interface ICatalogoGestionService
{
    Task<IReadOnlyList<CatalogoRegistroDto>> ObtenerRegistrosAsync(string entidad);
    Task CrearRegistroAsync(string entidad, CatalogoRegistroDto registro);
    Task EliminarRegistroAsync(string entidad, string clave);
}
