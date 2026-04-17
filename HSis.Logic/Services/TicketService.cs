using Microsoft.EntityFrameworkCore;
using HSis.Data.Models;
using HSis.Logic.DTOs;

namespace HSis.Logic.Services
{
    public class TicketService
    {
        // Obtener todos los tickets - Async
        public async Task<List<Ticket>> ObtenerTicketsAsync()
        {
            using var db = new HSisDbContext();
            return await db.Tickets
                .Include(t => t.IdUsuarioNavigation)
                .Include(t => t.IdTecnicoNavigation)
                .ToListAsync();
        }

        // Obtener un ticket por su id - Async
        public async Task<Ticket?> ObtenerTicketPorIdAsync(int id)
        {
            using var db = new HSisDbContext();
            return await db.Tickets
                .Include(t => t.IdUsuarioNavigation)
                .Include(t => t.IdTecnicoNavigation)
                .FirstOrDefaultAsync(t => t.IdTicket == id);
        }

        // Obtener tickets filtrados por SLA (urgentes/no urgentes) - Async
        public async Task<List<Ticket>> ObtenerTicketsPorSLAAsync(bool esUrgente)
        {
            using var db = new HSisDbContext();
            DateTime limite = DateTime.Now.AddHours(-48);

            var query = esUrgente 
                ? db.Tickets.Where(t => t.Status == ConstantesEstatus.ABIERTO && t.Alta < limite)
                : db.Tickets.Where(t => t.Status == ConstantesEstatus.ABIERTO && t.Alta >= limite);

            return await query
                .Include(t => t.IdUsuarioNavigation)
                .Include(t => t.IdTecnicoNavigation)
                .ToListAsync();
        }

        // Obtener tickets por estatus - Async
        public async Task<List<Ticket>> ObtenerTicketsPorEstatusAsync(string estatus)
        {
            using var db = new HSisDbContext();
            return await db.Tickets
                .Where(t => t.Status == estatus)
                .Include(t => t.IdUsuarioNavigation)
                .Include(t => t.IdTecnicoNavigation)
                .ToListAsync();
        }

        // Contar tickets por SLA - Async
        public async Task<int> ObtenerCountTicketsPorSLAAsync(bool esUrgente)
        {
            using var db = new HSisDbContext();
            DateTime limite = DateTime.Now.AddHours(-48);

            var query = esUrgente
                ? db.Tickets.Where(t => t.Status == ConstantesEstatus.ABIERTO && t.Alta < limite)
                : db.Tickets.Where(t => t.Status == ConstantesEstatus.ABIERTO && t.Alta >= limite);

            return await query.CountAsync();
        }

        // Contar tickets por estatus - Async
        public async Task<int> ObtenerCountTicketsPorEstatusAsync(string estatus)
        {
            using var db = new HSisDbContext();
            return await db.Tickets.CountAsync(t => t.Status == estatus);
        }

        public async Task<List<object>> ObtenerHistorialPorTicketAsync(int idTicket)
        {
            using var db = new HSisDbContext();

            // Filtramos por el ticket actual y ordenamos del más reciente al más antiguo
            var historial = await db.HistorialCambiosTickets
                .Include(h => h.IdUsuarioCambioNavigation)
                .Where(h => h.IdTicket == idTicket)
                .OrderByDescending(h => h.FechaMovimiento)
                .Select(h => new HistorialCambiosDto {
                    IdTicket = h.IdTicket,
                    UsuarioCambio = h.IdUsuarioCambioNavigation.Nombre ?? "-",
                    FechaMovimiento = h.FechaMovimiento,
                    CampoModificado = h.CampoModificado,
                    ValorAnterior = h.ValorAnterior ?? "-",
                    ValorNuevo = h.ValorNuevo ?? "-"
                })
                .ToListAsync<object>();

            return historial;
        }

        // Actualizar ticket con registro en historial - Transaccional
        public async Task ActualizarTicketConHistorialAsync(Ticket ticketEditado, int idUsuarioModifica)
        {
            using var db = new HSisDbContext();
            using var transaction = db.Database.BeginTransaction();

            try
            {
                // Obtener el ticket original sin tracking para comparación
                var ticketOriginal = await db.Tickets
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.IdTicket == ticketEditado.IdTicket);

                if (ticketOriginal == null)
                    throw new Exception("El ticket no existe.");

                // Comparar campos y registrar cambios en el historial
                if (ticketOriginal.Status != ticketEditado.Status)
                {
                    await RegistrarCambioHistorialAsync(db, ticketEditado.IdTicket, 
                        "Status", ticketOriginal.Status, ticketEditado.Status, idUsuarioModifica);
                }

                if (ticketOriginal.IdTecnico != ticketEditado.IdTecnico)
                {
                    await RegistrarCambioHistorialAsync(db, ticketEditado.IdTicket,
                        "IdTecnico", ticketOriginal.IdTecnico?.ToString() ?? "null", 
                        ticketEditado.IdTecnico?.ToString() ?? "null", idUsuarioModifica);
                }

                if (ticketOriginal.Solución != ticketEditado.Solución)
                {
                    await RegistrarCambioHistorialAsync(db, ticketEditado.IdTicket,
                        "Solución", ticketOriginal.Solución ?? "", ticketEditado.Solución ?? "", idUsuarioModifica);
                }

                // Actualizar el ticket
                db.Tickets.Update(ticketEditado);
                await db.SaveChangesAsync();

                // Confirmar la transacción
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Obtener tickets por usuario - Async
        public async Task<List<Ticket>> ObtenerTicketsPorUsuarioAsync(int idUsuario)
        {
            using var db = new HSisDbContext();
            return await db.Tickets
                .Where(t => t.IdUsuario == idUsuario)
                .Include(t => t.IdTecnicoNavigation)
                .OrderByDescending(t => t.Alta)
                .ToListAsync();
        }

        // Obtener tickets asignados a un técnico (no cerrados) - Async
        public async Task<List<Ticket>> ObtenerTicketsAsignadosATecnicoAsync(int idTecnico)
        {
            using var db = new HSisDbContext();
            return await db.Tickets
                .Where(t => t.IdTecnico == idTecnico && t.Status != ConstantesEstatus.CERRADO)
                .Include(t => t.IdUsuarioNavigation)
                .OrderByDescending(t => t.Alta)
                .ToListAsync();
        }

        // Obtener tickets disponibles (abiertos sin técnico asignado) - Async
        public async Task<List<Ticket>> ObtenerTicketsDisponiblesAsync()
        {
            using var db = new HSisDbContext();
            return await db.Tickets
                .Where(t => t.Status == ConstantesEstatus.ABIERTO && t.IdTecnico == null)
                .Include(t => t.IdUsuarioNavigation)
                .OrderByDescending(t => t.Alta)
                .ToListAsync();
        }

        // Crear un nuevo ticket - Async
        public async Task<Ticket> CrearTicketAsync(Ticket ticket)
        {
            using var db = new HSisDbContext();
            db.Tickets.Add(ticket);
            await db.SaveChangesAsync();
            return ticket;
        }

        // Método auxiliar para registrar cambios en el historial
        private async Task RegistrarCambioHistorialAsync(HSisDbContext db, int idTicket, 
            string campoModificado, string valorAnterior, string valorNuevo, int idUsuarioCambio)
        {
            var historial = new HistorialCambiosTicket
            {
                IdTicket = idTicket,
                CampoModificado = campoModificado,
                ValorAnterior = valorAnterior,
                ValorNuevo = valorNuevo,
                IdUsuarioCambio = idUsuarioCambio,
                FechaMovimiento = DateTime.Now
            };

            db.HistorialCambiosTickets.Add(historial);
            await db.SaveChangesAsync();
        }
    }
}
