using HSis.Contracts.Services;
using HSis.Data.Models;
using HSis.Contracts.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HSis.Logic.Services
{
    /// <summary>
    /// Servicio para gestionar operaciones relacionadas con Materiales e inventario.
    /// </summary>
    public class MaterialService(IDbContextFactory<HSisDbContext> dbContextFactory) : IMaterialService
    {
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

        public async Task<List<MaterialDto>> ObtenerMaterialesAsync()
        {
            using var db = dbContextFactory.CreateDbContext();
            return await db.Materials
                .AsNoTracking()
                .Select(m => new MaterialDto
                {
                    IdMaterial = m.IdMaterial,
                    Nombre = m.Nombre,
                    CostoUnitario = m.Costo,
                    StockActual = m.Inventario,
                    UnidadMedida = m.UnidadMedida
                })
                .ToListAsync();
        }

        public async Task<List<KardexMovimientoDto>> ObtenerKardexPorMaterialAsync(int idMaterial)
        {
            using var db = dbContextFactory.CreateDbContext();
            return await db.VHistorialInventarios
                .AsNoTracking()
                .Where(h => h.IdMaterial == idMaterial)
                .OrderByDescending(h => h.Fecha)
                .Select(h => new KardexMovimientoDto
                {
                    IdMaterial = h.IdMaterial,
                    MaterialNombre = h.Material,
                    Fecha = h.Fecha ?? DateTime.Now,
                    TipoMovimiento = h.TipoMovimiento,
                    Cantidad = h.Cantidad ?? 0,
                    CostoUnitario = h.CostoUnitario,
                    UsuarioNombre = h.UsuarioResponsable ?? string.Empty,
                    Motivo = h.Motivo ?? string.Empty
                })
                .ToListAsync();
        }

        public async Task RegistrarMovimientoAsync(KardexMovimientoDto movimiento)
        {
            using var db = dbContextFactory.CreateDbContext();
            var entity = new MovimientoMaterial
            {
                IdMaterial = movimiento.IdMaterial,
                Cantidad = movimiento.Cantidad,
                CostoUnitario = movimiento.CostoUnitario,
                FechaMovimiento = movimiento.Fecha == default ? DateTime.Now : movimiento.Fecha,
                IdUsuario = movimiento.IdUsuario,
                Motivo = movimiento.Motivo
            };
            db.MovimientosMateriales.Add(entity);

            var mat = await db.Materials.FindAsync(movimiento.IdMaterial);
            if (mat != null)
            {
                mat.Inventario += movimiento.Cantidad;
                if (movimiento.CostoUnitario > 0)
                {
                    mat.Costo = movimiento.CostoUnitario;
                }
            }
            await db.SaveChangesAsync();
        }
    }
}
