using HSis.Logic.DTOs;

namespace HSis.Logic.Services
{
    public interface IMaterialService
    {
        Task ActualizarCostoMaterialAsync(int idMaterial, decimal nuevoCosto);
        Task<List<MaterialDto>> ObtenerMaterialesAsync();
        Task<List<KardexMovimientoDto>> ObtenerKardexPorMaterialAsync(int idMaterial);
        Task RegistrarMovimientoAsync(KardexMovimientoDto movimiento);
    }
}
