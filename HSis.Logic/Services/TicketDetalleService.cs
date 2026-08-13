using HSis.Data.Models;
using HSis.Logic.DTOs;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace HSis.Logic.Services
{
    /// <summary>
    /// Servicio para gestionar operaciones relacionadas con Detalles de Tickets.
    /// Incluye obtención, creación y gestión de materiales asociados a tickets.
    /// </summary>
    public class TicketDetalleService(IDbContextFactory<HSisDbContext> dbContextFactory) : ITicketDetalleService
    {

        // Obtener detalles de ticket - Async
        public async Task<List<TicketDetalleDto>> ObtenerDetallesTicketAsync(int idTicket)
        {
            using var db = dbContextFactory.CreateDbContext();
            var detalles = await db.DetTickets
                .Include(dt => dt.Material)
                .Where(dt => dt.IdTicket == idTicket)
                .ToListAsync();

            return detalles.Adapt<List<TicketDetalleDto>>();
        }

        public async Task<TicketDetalleDto?> ObtenerDetallePorIdAsync(int idTicket, int idMaterial)
        {
            using var db = dbContextFactory.CreateDbContext();
            var detalle = await db.DetTickets
                .Include(dt => dt.Material)
                .FirstOrDefaultAsync(dt => dt.IdTicket == idTicket && dt.IdMaterial == idMaterial);

            return detalle?.Adapt<TicketDetalleDto>();
        }

        // CRUD DetTicket - Async
        public async Task AgregarMaterialATicketAsync(TicketDetalleDto detalleDto)
        {
            using var db = dbContextFactory.CreateDbContext();
            var detTicket = detalleDto.Adapt<DetTicket>();

            // Consultar el costo actual del material para este egreso
            var material = await db.Materials.FindAsync(detTicket.IdMaterial);
            if (material != null)
            {
                detTicket.CostoUnitarioAplicado = material.Costo;
            }

            db.DetTickets.Add(detTicket);
            await db.SaveChangesAsync();
        }

        public async Task ActualizarDetalleTicketAsync(TicketDetalleDto detalleDto)
        {
            using var db = dbContextFactory.CreateDbContext();
            var detTicket = detalleDto.Adapt<DetTicket>();
            db.DetTickets.Update(detTicket);
            await db.SaveChangesAsync();
        }

        public async Task EliminarMaterialDeTicketAsync(int idTicket, int idMaterial)
        {
            using var db = dbContextFactory.CreateDbContext();
            var detTicket = await db.DetTickets.FindAsync(idTicket, idMaterial);
            if (detTicket != null)
            {
                db.DetTickets.Remove(detTicket);
                await db.SaveChangesAsync();
            }
        }

        // Cálculos - Async
        public async Task<decimal> ObtenerCostoTotalMaterialesTicketAsync(int idTicket)
        {
            using var db = dbContextFactory.CreateDbContext();
            return await db.DetTickets
                .Where(dt => dt.IdTicket == idTicket)
                .SumAsync(dt => dt.CostoUnitarioAplicado * dt.Cantidad);
        }

    }
}

