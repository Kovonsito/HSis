using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Json;
using HSis.Logic.Services;

namespace HSis.UI.ApiClients
{
    public class CatalogoApiClientService(HttpClient httpClient) : ICatalogoService
    {

        public async Task<List<T>> ObtenerTodosAsync<T>() where T : class
        {
            var entityName = typeof(T).Name;
            return await httpClient.GetFromJsonAsync<List<T>>($"api/Catalogos/{entityName}") ?? [];
        }

        public Task<List<T>> ObtenerFiltradoAsync<T>(Expression<Func<T, bool>> predicado) where T : class => throw new NotImplementedException();
        public Task CrearAsync<T>(T entidad) where T : class => throw new NotImplementedException();
        public Task ActualizarAsync<T>(T entidad) where T : class => throw new NotImplementedException();
        public Task EliminarAsync<T>(object id) where T : class => throw new NotImplementedException();
        public Task<List<object>> ObtenerTodosPorTipoAsync(Type tipoEntidad) => throw new NotImplementedException();
        public Task<int> ObtenerSiguienteIdAsync(Type tipoEntidad, string nombrePropiedadId) => throw new NotImplementedException();
    }
}
