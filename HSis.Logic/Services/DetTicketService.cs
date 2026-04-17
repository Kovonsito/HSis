using Microsoft.EntityFrameworkCore;
using HSis.Data.Models;

namespace HSis.Logic.Services
{
    /// <summary>
    /// Servicio para gestionar operaciones relacionadas con Detalles de Tickets.
    /// Incluye obtención, creación y gestión de materiales asociados a tickets.
    /// </summary>
    public class DetTicketService
    {
        // Obtener detalles de ticket - Async
        public async Task<List<DetTicket>> ObtenerDetallesTicketAsync(int idTicket)
        {
            using var db = new HSisDbContext();
            return await db.DetTickets
                .Include(dt => dt.IdMaterialNavigation)
                .Where(dt => dt.IdTicket == idTicket)
                .ToListAsync();
        }

        public async Task<DetTicket?> ObtenerDetallePorIdAsync(int idTicket, int idMaterial)
        {
            using var db = new HSisDbContext();
            return await db.DetTickets
                .Include(dt => dt.IdMaterialNavigation)
                .FirstOrDefaultAsync(dt => dt.IdTicket == idTicket && dt.IdMaterial == idMaterial);
        }

        // CRUD DetTicket - Async
        public async Task AgregarMaterialATicketAsync(DetTicket detTicket)
        {
            using var db = new HSisDbContext();
            db.DetTickets.Add(detTicket);
            await db.SaveChangesAsync();
        }

        public async Task ActualizarDetalleTicketAsync(DetTicket detTicket)
        {
            using var db = new HSisDbContext();
            db.DetTickets.Update(detTicket);
            await db.SaveChangesAsync();
        }

        public async Task EliminarMaterialDeTicketAsync(int idTicket, int idMaterial)
        {
            using var db = new HSisDbContext();
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
            using var db = new HSisDbContext();
            return await db.DetTickets
                .Where(dt => dt.IdTicket == idTicket)
                .SumAsync(dt => dt.CostoUnitarioAplicado * dt.Cantidad);
        }

        // Obtener todos los detalles - Async
        public async Task<List<DetTicket>> ObtenerTodosDetallesAsync()
        {
            using var db = new HSisDbContext();
            return await db.DetTickets
                .Include(dt => dt.IdMaterialNavigation)
                .Include(dt => dt.IdTicketNavigation)
                .ToListAsync();
        }
    }
}

