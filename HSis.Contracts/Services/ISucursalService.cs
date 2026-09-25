using HSis.Contracts.DTOs;

namespace HSis.Contracts.Services;

public interface ISucursalService
{
    Task<List<SucursalDto>> ObtenerSucursalesAsync();
    Task<SucursalDto> CrearSucursalAsync(SucursalCatalogoRequestDto request);
    Task<SucursalDto?> ActualizarSucursalAsync(int idSucursal, SucursalCatalogoRequestDto request);
    Task<bool> EliminarSucursalAsync(int idSucursal);
}
