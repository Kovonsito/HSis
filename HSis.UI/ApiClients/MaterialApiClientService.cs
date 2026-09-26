using System.Net.Http;
using System.Net.Http.Json;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;

namespace HSis.UI.ApiClients
{
    public class MaterialApiClientService(HttpClient httpClient) : IMaterialService
    {
        public async Task<MaterialDto> CrearMaterialAsync(MaterialCatalogoRequestDto request)
        {
            using var response = await httpClient.PostAsJsonAsync("api/Catalogos/materiales", request);
            return await response.ReadRequiredJsonWithDetailsAsync<MaterialDto>("material creado");
        }

        public async Task<MaterialDto?> ActualizarMaterialAsync(int idMaterial, MaterialCatalogoRequestDto request)
        {
            using var response = await httpClient.PutAsJsonAsync($"api/Catalogos/materiales/{idMaterial}", request);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            return await response.ReadFromJsonWithDetailsAsync<MaterialDto>();
        }

        public async Task<bool> EliminarMaterialAsync(int idMaterial)
        {
            using var response = await httpClient.DeleteAsync($"api/Catalogos/materiales/{idMaterial}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }

            await response.EnsureSuccessStatusCodeWithDetailsAsync();
            return true;
        }

        public async Task ActualizarCostoMaterialAsync(int idMaterial, decimal nuevoCosto)
        {
            using var response = await httpClient.PutAsJsonAsync($"api/Materiales/{idMaterial}/costo", nuevoCosto);
            await response.EnsureSuccessStatusCodeWithDetailsAsync();
        }

        public async Task<List<MaterialDto>> ObtenerMaterialesAsync()
        {
            return await httpClient.GetFromJsonWithDetailsAsync<List<MaterialDto>>("api/Catalogos/materiales") ?? [];
        }

        public async Task<List<KardexMovimientoDto>> ObtenerKardexPorMaterialAsync(int idMaterial)
        {
            return await httpClient.GetFromJsonWithDetailsAsync<List<KardexMovimientoDto>>($"api/Materiales/{idMaterial}/kardex") ?? [];
        }

        public async Task RegistrarMovimientoAsync(KardexMovimientoDto movimiento)
        {
            using var response = await httpClient.PostAsJsonAsync("api/Materiales/movimientos", movimiento);
            await response.EnsureSuccessStatusCodeWithDetailsAsync();
        }
    }
}
