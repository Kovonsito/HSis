using Microsoft.EntityFrameworkCore;
using HSis.Data.Models;

namespace HSis.Logic.Services
{
    /// <summary>
    /// Servicio para gestionar operaciones relacionadas con Ingresos de Materiales.
    /// Incluye registro y obtención de ingresos de inventario.
    /// </summary>
    public class IngresoService
    {
        // Obtener ingresos - Async
        public async Task<List<Ingreso>> ObtenerIngresosAsync()
        {
            using var db = new HSisDbContext();
            return await db.Ingresos
                .Include(i => i.IdMaterialNavigation)
                .OrderByDescending(i => i.UltimoIngreso)
                .ToListAsync();
        }

        public async Task<Ingreso?> ObtenerIngresoPorIdAsync(int idIngreso)
        {
            using var db = new HSisDbContext();
            return await db.Ingresos
                .Include(i => i.IdMaterialNavigation)
                .FirstOrDefaultAsync(i => i.IdIngreso == idIngreso);
        }

        public async Task<Ingreso?> ObtenerIngresoPorMaterialAsync(int idMaterial)
        {
            using var db = new HSisDbContext();
            return await db.Ingresos
                .Include(i => i.IdMaterialNavigation)
                .FirstOrDefaultAsync(i => i.IdMaterial == idMaterial);
        }

        // Registrar ingreso - Async
        public async Task RegistrarIngresoAsync(Ingreso ingreso)
        {
            using var db = new HSisDbContext();
            ingreso.UltimoIngreso = DateTime.Now;
            db.Ingresos.Add(ingreso);
            await db.SaveChangesAsync();
        }

        // Actualizar ingreso - Async
        public async Task ActualizarIngresoAsync(Ingreso ingreso)
        {
            using var db = new HSisDbContext();
            ingreso.UltimoIngreso = DateTime.Now;
            db.Ingresos.Update(ingreso);
            await db.SaveChangesAsync();
        }

        // Obtener ingresos por rango de fechas - Async
        public async Task<List<Ingreso>> ObtenerIngresosPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            using var db = new HSisDbContext();
            return await db.Ingresos
                .Include(i => i.IdMaterialNavigation)
                .Where(i => i.UltimoIngreso >= fechaInicio && i.UltimoIngreso <= fechaFin)
                .OrderByDescending(i => i.UltimoIngreso)
                .ToListAsync();
        }
    }
}

