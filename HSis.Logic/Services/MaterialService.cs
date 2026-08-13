using HSis.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HSis.Logic.Services
{
    /// <summary>
    /// Servicio para gestionar operaciones relacionadas con Materiales.
    /// Incluye obtención, creación y gestión de inventario de materiales.
    /// </summary>
    public class MaterialService(IDbContextFactory<HSisDbContext> dbContextFactory) : IMaterialService
    {

        // Gestión de costos - Async
        public async Task ActualizarCostoMaterialAsync(int idMaterial, decimal nuevoCosto)
        {
            using var db = dbContextFactory.CreateDbContext();
            var material = await db.Materials.FindAsync(idMaterial);
            if (material != null)
            {
                material.Costo = nuevoCosto;
                db.Materials.Update(material);
                await db.SaveChangesAsync();
            }
        }
    }

}
