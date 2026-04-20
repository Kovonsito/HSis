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

