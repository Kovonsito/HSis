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
            return await db.Set<T>().ToListAsync();
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
