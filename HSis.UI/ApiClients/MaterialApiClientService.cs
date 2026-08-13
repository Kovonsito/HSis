using System.Net.Http;
using System.Net.Http.Json;
using HSis.Logic.Services;

namespace HSis.UI.ApiClients
{
    public class MaterialApiClientService(HttpClient httpClient) : IMaterialService
    {

        public async Task ActualizarCostoMaterialAsync(int idMaterial, decimal nuevoCosto)
        {
            await httpClient.PutAsJsonAsync($"api/Materiales/{idMaterial}/costo", nuevoCosto);
        }

    }
}

