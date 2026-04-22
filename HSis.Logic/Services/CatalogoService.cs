using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HSis.Data.Models;

namespace HSis.Logic.Services
{
    public class CatalogoService
    {
        private readonly IDbContextFactory<HSisDbContext> _dbContextFactory;

        public CatalogoService(IDbContextFactory<HSisDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<List<T>> ObtenerTodosAsync<T>() where T : class
        {
            using var db = _dbContextFactory.CreateDbContext();
            IQueryable<T> query = db.Set<T>();
            var navigations = db.Model.FindEntityType(typeof(T))?.GetNavigations();
            if (navigations != null)
            {
                foreach (var nav in navigations)
                {
                    // No incluimos colecciones (relaciones uno a muchos inversas)
                    if (!nav.IsCollection)
                    {
                        query = query.Include(nav.Name);
                    }
                }
            }
            return await query.ToListAsync();
        }

        public async Task<List<T>> ObtenerFiltradoAsync<T>(Expression<System.Func<T, bool>> predicado) where T : class
        {
            using var db = _dbContextFactory.CreateDbContext();
            IQueryable<T> query = db.Set<T>();
            var navigations = db.Model.FindEntityType(typeof(T))?.GetNavigations();
            if (navigations != null)
            {
                foreach (var nav in navigations)
                {
                    if (!nav.IsCollection)
                    {
                        query = query.Include(nav.Name);
                    }
                }
            }
            return await query.Where(predicado).ToListAsync();
        }

        public async Task CrearAsync<T>(T entidad) where T : class
        {
            using var db = _dbContextFactory.CreateDbContext();
            db.Set<T>().Add(entidad);
            await db.SaveChangesAsync();
        }

        public async Task ActualizarAsync<T>(T entidad) where T : class
        {
            using var db = _dbContextFactory.CreateDbContext();
            db.Set<T>().Update(entidad);
            await db.SaveChangesAsync();
        }

        public async Task EliminarAsync<T>(object id) where T : class
        {
            using var db = _dbContextFactory.CreateDbContext();
            var entity = await db.Set<T>().FindAsync(id);
            if (entity != null)
            {
                db.Set<T>().Remove(entity);
                await db.SaveChangesAsync();
            }
        }

        // --- Métodos dinámicos por Tipo (Ocultan la reflexión del DbContext a la capa UI) ---
        
        public async Task<List<object>> ObtenerTodosPorTipoAsync(System.Type tipoEntidad)
        {
            using var db = _dbContextFactory.CreateDbContext();
            
            // Invocamos Set<T>() dinámicamente
            var queryableMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set), System.Type.EmptyTypes)?.MakeGenericMethod(tipoEntidad);
            var dbSet = queryableMethod?.Invoke(db, null);
            
            if (dbSet != null)
            {
                // Invocamos ToListAsync dinámicamente para IQueryable<T>
                var toListAsyncMethod = typeof(EntityFrameworkQueryableExtensions)
                    .GetMethod(nameof(EntityFrameworkQueryableExtensions.ToListAsync))
                    ?.MakeGenericMethod(tipoEntidad);

                if (toListAsyncMethod != null)
                {
                    // ToListAsync devuelve Task<List<T>>. Esperamos dinámicamente.
                    var task = (Task)toListAsyncMethod.Invoke(null, new object[] { dbSet, default(System.Threading.CancellationToken) })!;
                    await task;

                    // Extraemos el Result del Task
                    var resultProperty = task.GetType().GetProperty("Result");
                    var list = resultProperty?.GetValue(task) as System.Collections.IEnumerable;

                    if (list != null)
                    {
                        return list.Cast<object>().ToList();
                    }
                }
            }

            return new List<object>();
        }

        public async Task<int> ObtenerSiguienteIdAsync(System.Type tipoEntidad, string nombrePropiedadId)
        {
            var registros = await ObtenerTodosPorTipoAsync(tipoEntidad);
            if (registros.Any())
            {
                var propiedadId = tipoEntidad.GetProperty(nombrePropiedadId);
                if (propiedadId != null)
                {
                    int lastId = registros.Max(o => System.Convert.ToInt32(propiedadId.GetValue(o)));
                    return lastId + 1;
                }
            }
            return 1;
        }
    }
}
