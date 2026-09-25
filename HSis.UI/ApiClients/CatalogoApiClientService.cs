using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;

namespace HSis.UI.ApiClients
{
    public class CatalogoApiClientService(HttpClient httpClient) : ICatalogoService, ICatalogoGestionService
    {

        public async Task<List<T>> ObtenerTodosAsync<T>() where T : class
        {
            var entityName = typeof(T).Name;
            return await httpClient.GetFromJsonAsync<List<T>>($"api/Catalogos/{entityName}") ?? [];
        }

        public async Task<List<object>> ObtenerTodosPorTipoAsync(Type tipoEntidad)
        {
            var entityName = tipoEntidad.Name;
            var jsonElements = await httpClient.GetFromJsonAsync<List<System.Text.Json.JsonElement>>($"api/Catalogos/{entityName}") ?? [];
            var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var list = jsonElements.Select(e => System.Text.Json.JsonSerializer.Deserialize(e.GetRawText(), tipoEntidad, options)!).ToList();
            return list;
        }

        public async Task CrearAsync<T>(T entidad) where T : class
        {
            var entityName = typeof(T).Name;
            var response = await httpClient.PostAsJsonAsync($"api/Catalogos/{entityName}", entidad);
            await response.EnsureSuccessStatusCodeWithDetailsAsync();
        }

        public async Task ActualizarAsync<T>(T entidad) where T : class
        {
            var entityName = typeof(T).Name;
            var response = await httpClient.PutAsJsonAsync($"api/Catalogos/{entityName}", entidad);
            await response.EnsureSuccessStatusCodeWithDetailsAsync();
        }

        public async Task EliminarAsync<T>(object id) where T : class
        {
            var entityName = typeof(T).Name;
            var response = await httpClient.DeleteAsync($"api/Catalogos/{entityName}/{id}");
            await response.EnsureSuccessStatusCodeWithDetailsAsync();
        }

        public Task<List<T>> ObtenerFiltradoAsync<T>(Expression<Func<T, bool>> predicado) where T : class
        {
            throw new NotImplementedException();
        }


        public async Task<int> ObtenerSiguienteIdAsync(Type tipoEntidad, string nombrePropiedadId)
        {
            var list = await ObtenerTodosPorTipoAsync(tipoEntidad);
            if (list.Count > 0)
            {
                var prop = tipoEntidad.GetProperty(nombrePropiedadId);
                if (prop != null)
                {
                    int maxId = list.Select(x => Convert.ToInt32(prop.GetValue(x))).DefaultIfEmpty(0).Max();
                    return maxId + 1;
                }
            }
            return 1;
        }

        public async Task<IReadOnlyList<CatalogoRegistroDto>> ObtenerRegistrosAsync(string entidad)
        {
            var respuesta = await httpClient.GetFromJsonAsync<List<Dictionary<string, JsonElement>>>(
                $"api/Catalogos/{Uri.EscapeDataString(entidad)}/registros") ?? [];

            return respuesta
                .Select(valores => new CatalogoRegistroDto
                {
                    Valores = valores.ToDictionary(
                        par => par.Key,
                        par => (object?)ConvertirJsonElement(par.Value),
                        StringComparer.OrdinalIgnoreCase)
                })
                .ToList();
        }

        public async Task CrearRegistroAsync(string entidad, CatalogoRegistroDto registro)
        {
            var respuesta = await httpClient.PostAsJsonAsync(
                $"api/Catalogos/{Uri.EscapeDataString(entidad)}/registros",
                registro.Valores);
            await respuesta.EnsureSuccessStatusCodeWithDetailsAsync();
        }

        public async Task EliminarRegistroAsync(string entidad, string clave)
        {
            var respuesta = await httpClient.DeleteAsync(
                $"api/Catalogos/{Uri.EscapeDataString(entidad)}/registros/{Uri.EscapeDataString(clave)}");
            await respuesta.EnsureSuccessStatusCodeWithDetailsAsync();
        }

        private static object? ConvertirJsonElement(JsonElement valor)
        {
            return valor.ValueKind switch
            {
                JsonValueKind.String => valor.GetString(),
                JsonValueKind.Number when valor.TryGetInt64(out var entero) => entero,
                JsonValueKind.Number when valor.TryGetDecimal(out var decimalValue) => decimalValue,
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                _ => valor
            };
        }
    }
}

