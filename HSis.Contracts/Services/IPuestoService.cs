using HSis.Contracts.DTOs;

namespace HSis.Contracts.Services;

public interface IPuestoService
{
    Task<List<PuestoDto>> ObtenerPuestosAsync();
    Task<PuestoDto> CrearPuestoAsync(PuestoCatalogoRequestDto request);
    Task<PuestoDto?> ActualizarPuestoAsync(int idPuesto, PuestoCatalogoRequestDto request);
    Task<bool> EliminarPuestoAsync(int idPuesto);
}
