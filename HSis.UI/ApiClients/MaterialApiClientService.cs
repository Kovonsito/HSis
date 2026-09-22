using System.Net.Http;
using System.Net.Http.Json;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.UI.Services;

namespace HSis.UI.ApiClients
{
    public class MaterialApiClientService(HttpClient httpClient) : IMaterialService
    {
        public async Task ActualizarCostoMaterialAsync(int idMaterial, decimal nuevoCosto)
        {
            var response = await httpClient.PutAsJsonAsync($"api/Materiales/{idMaterial}/costo", nuevoCosto);
            await response.EnsureSuccessStatusCodeWithDetailsAsync();
        }

        public async Task<List<MaterialDto>> ObtenerMaterialesAsync()
        {
            return await httpClient.GetFromJsonAsync<List<MaterialDto>>("api/Materiales") ?? [];
        }

        public async Task<List<KardexMovimientoDto>> ObtenerKardexPorMaterialAsync(int idMaterial)
        {
            return await httpClient.GetFromJsonAsync<List<KardexMovimientoDto>>($"api/Materiales/{idMaterial}/kardex") ?? [];
        }

        public async Task RegistrarMovimientoAsync(KardexMovimientoDto movimiento)
        {
            var response = await httpClient.PostAsJsonAsync("api/Materiales/movimientos", movimiento);
            await response.EnsureSuccessStatusCodeWithDetailsAsync();
        }
    }
}
