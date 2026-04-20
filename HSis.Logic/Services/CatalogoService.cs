using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HSis.Data.Models;

namespace HSis.Logic.Services
{
    public class CatalogoService
    {
        public async Task<List<T>> ObtenerTodosAsync<T>() where T : class
        {
            using var db = new HSisDbContext();
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

        public async Task CrearAsync<T>(T entidad) where T : class
        {
            using var db = new HSisDbContext();
            db.Set<T>().Add(entidad);
            await db.SaveChangesAsync();
        }

        public async Task ActualizarAsync<T>(T entidad) where T : class
        {
            using var db = new HSisDbContext();
            db.Set<T>().Update(entidad);
            await db.SaveChangesAsync();
        }

        public async Task EliminarAsync<T>(object id) where T : class
        {
            using var db = new HSisDbContext();
            var entity = await db.Set<T>().FindAsync(id);
            if (entity != null)
            {
                db.Set<T>().Remove(entity);
                await db.SaveChangesAsync();
            }
        }
    }
}
