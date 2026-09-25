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
            var response = await httpClient.PostAsJsonAsync("api/Catalogos/materiales", request);
            await response.EnsureSuccessStatusCodeWithDetailsAsync();
            return await response.Content.ReadFromJsonAsync<MaterialDto>()
                ?? throw new HttpRequestException("La API no devolvió el material creado.");
        }

        public async Task<MaterialDto?> ActualizarMaterialAsync(int idMaterial, MaterialCatalogoRequestDto request)
        {
            var response = await httpClient.PutAsJsonAsync($"api/Catalogos/materiales/{idMaterial}", request);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            await response.EnsureSuccessStatusCodeWithDetailsAsync();
            return await response.Content.ReadFromJsonAsync<MaterialDto>();
        }

        public async Task<bool> EliminarMaterialAsync(int idMaterial)
        {
            var response = await httpClient.DeleteAsync($"api/Catalogos/materiales/{idMaterial}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }

            await response.EnsureSuccessStatusCodeWithDetailsAsync();
            return true;
        }

        public async Task ActualizarCostoMaterialAsync(int idMaterial, decimal nuevoCosto)
        {
            var response = await httpClient.PutAsJsonAsync($"api/Materiales/{idMaterial}/costo", nuevoCosto);
            await response.EnsureSuccessStatusCodeWithDetailsAsync();
        }

        public async Task<List<MaterialDto>> ObtenerMaterialesAsync()
        {
            return await httpClient.GetFromJsonAsync<List<MaterialDto>>("api/Catalogos/materiales") ?? [];
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
