using Microsoft.EntityFrameworkCore;
using HSis.Data.Models;

namespace HSis.Logic.Services
{
    /// <summary>
    /// Servicio para gestionar operaciones relacionadas con Materiales.
    /// Incluye obtención, creación y gestión de inventario de materiales.
    /// </summary>
    public class MaterialService
    {
        // Obtener materiales - Async
        public async Task<List<Material>> ObtenerMaterialesAsync()
        {
            using var db = new HSisDbContext();
            return await db.Materials.OrderBy(m => m.Nombre).ToListAsync();
        }

        public async Task<Material?> ObtenerMaterialPorIdAsync(int idMaterial)
        {
            using var db = new HSisDbContext();
            return await db.Materials.FirstOrDefaultAsync(m => m.IdMaterial == idMaterial);
        }

        // CRUD Material - Async
        public async Task CrearMaterialAsync(Material material)
        {
            using var db = new HSisDbContext();
            db.Materials.Add(material);
            await db.SaveChangesAsync();
        }

        public async Task ActualizarMaterialAsync(Material material)
        {
            using var db = new HSisDbContext();
            db.Materials.Update(material);
            await db.SaveChangesAsync();
        }

        public async Task EliminarMaterialAsync(int idMaterial)
        {
            using var db = new HSisDbContext();
            var material = await db.Materials.FindAsync(idMaterial);
            if (material != null)
            {
                db.Materials.Remove(material);
                await db.SaveChangesAsync();
            }
        }

        // Gestión de costos - Async
        public async Task ActualizarCostoMaterialAsync(int idMaterial, decimal nuevoCosto)
        {
            using var db = new HSisDbContext();
            var material = await db.Materials.FindAsync(idMaterial);
            if (material != null)
            {
                material.CostoAnterior = material.Costo;
                material.Costo = nuevoCosto;
                db.Materials.Update(material);
                await db.SaveChangesAsync();
            }
        }
    }
}

