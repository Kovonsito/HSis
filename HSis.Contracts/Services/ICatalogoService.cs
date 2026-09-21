using System.Linq.Expressions;

namespace HSis.Logic.Services
{
    public interface ICatalogoService
    {
        Task<List<T>> ObtenerTodosAsync<T>() where T : class;
        Task<List<T>> ObtenerFiltradoAsync<T>(Expression<Func<T, bool>> predicado) where T : class;
        Task CrearAsync<T>(T entidad) where T : class;
        Task ActualizarAsync<T>(T entidad) where T : class;
        Task EliminarAsync<T>(object id) where T : class;
        Task<List<object>> ObtenerTodosPorTipoAsync(Type tipoEntidad);
        Task<int> ObtenerSiguienteIdAsync(Type tipoEntidad, string nombrePropiedadId);
    }
}

