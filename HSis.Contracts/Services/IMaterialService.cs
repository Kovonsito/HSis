using HSis.Contracts.DTOs;

namespace HSis.Contracts.Services
{
    public interface IMaterialService
    {
        Task<List<MaterialDto>> ObtenerMaterialesAsync();
        Task<MaterialDto> CrearMaterialAsync(MaterialCatalogoRequestDto request);
        Task<MaterialDto?> ActualizarMaterialAsync(int idMaterial, MaterialCatalogoRequestDto request);
        Task<bool> EliminarMaterialAsync(int idMaterial);
        Task ActualizarCostoMaterialAsync(int idMaterial, decimal nuevoCosto);
        Task<List<KardexMovimientoDto>> ObtenerKardexPorMaterialAsync(int idMaterial);
        Task RegistrarMovimientoAsync(KardexMovimientoDto movimiento);
    }
}
