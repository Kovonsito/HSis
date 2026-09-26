using HSis.Contracts.Services;
using HSis.Data.Models;
using HSis.Contracts.DTOs;
using HSis.Contracts.Constants;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace HSis.Logic.Services
{
    /// <summary>
    /// Servicio para gestionar operaciones relacionadas con Detalles de Tickets.
    /// Incluye obtención, creación y gestión de materiales asociados a tickets.
    /// </summary>
    public class TicketDetalleService(
        IDbContextFactory<HSisDbContext> dbContextFactory,
        INotificacionDestinatariosService? notificationRecipients = null,
        IServerNotificationDispatcher? notificationDispatcher = null,
        ILogger<TicketDetalleService>? logger = null) : ITicketDetalleService
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
            var ticket = await db.Tickets.FindAsync(detalleDto.IdTicket)
                ?? throw new KeyNotFoundException($"No existe el ticket {detalleDto.IdTicket}.");
            var material = await db.Materials.FindAsync(detalleDto.IdMaterial)
                ?? throw new KeyNotFoundException($"No existe el material {detalleDto.IdMaterial}.");

            // Consultar el costo actual del material para este egreso
            var detTicket = new DetTicket
            {
                IdTicket = detalleDto.IdTicket,
                IdMaterial = detalleDto.IdMaterial,
                Cantidad = detalleDto.Cantidad,
                CostoUnitarioAplicado = material.Costo
            };

            var destinatarios = notificationRecipients is null
                ? Array.Empty<int>()
                : await notificationRecipients.ObtenerDestinatariosCambioMaterialTicketAsync(
                    ticket.IdUsuario,
                    ticket.IdTecnico);
            var mensaje = NotificacionFactory.CrearMensajeMaterialTicket(
                ticket.IdTicket,
                ticket.IdTicket.ToString(),
                material.Nombre,
                detTicket.Cantidad,
                agregado: true);

            await EjecutarEnTransaccionAsync(db, async () =>
            {
                db.DetTickets.Add(detTicket);
                await db.SaveChangesAsync();
                await GuardarNotificacionesAsync(
                    db,
                    destinatarios,
                    ticket.IdTicket,
                    material.IdMaterial,
                    mensaje);
            });

            await PublicarNotificacionSeguraAsync(
                destinatarios,
                ticket.IdTicket,
                material.IdMaterial,
                mensaje,
                "agregar material al ticket");
        }

        public async Task ActualizarDetalleTicketAsync(TicketDetalleDto detalleDto)
        {
            using var db = dbContextFactory.CreateDbContext();
            var detTicket = await db.DetTickets
                .SingleOrDefaultAsync(item =>
                    item.IdTicket == detalleDto.IdTicket &&
                    item.IdMaterial == detalleDto.IdMaterial);
            if (detTicket is null)
            {
                return;
            }

            var cantidadAnterior = detTicket.Cantidad;
            detTicket.Cantidad = detalleDto.Cantidad;
            detTicket.CostoUnitarioAplicado = detalleDto.CostoUnitarioAplicado;

            if (cantidadAnterior == detTicket.Cantidad)
            {
                await db.SaveChangesAsync();
                return;
            }

            var ticket = await db.Tickets.FindAsync(detalleDto.IdTicket)
                ?? throw new KeyNotFoundException($"No existe el ticket {detalleDto.IdTicket}.");
            var material = await db.Materials.FindAsync(detalleDto.IdMaterial)
                ?? throw new KeyNotFoundException($"No existe el material {detalleDto.IdMaterial}.");
            var destinatarios = notificationRecipients is null
                ? Array.Empty<int>()
                : await notificationRecipients.ObtenerDestinatariosCambioMaterialTicketAsync(
                    ticket.IdUsuario,
                    ticket.IdTecnico);
            var mensaje = NotificacionFactory.CrearMensajeActualizacionMaterialTicket(
                ticket.IdTicket,
                ticket.IdTicket.ToString(),
                material.Nombre,
                cantidadAnterior,
                detTicket.Cantidad);

            await EjecutarEnTransaccionAsync(db, async () =>
            {
                await db.SaveChangesAsync();
                await GuardarNotificacionesAsync(
                    db,
                    destinatarios,
                    ticket.IdTicket,
                    material.IdMaterial,
                    mensaje);
            });

            await PublicarNotificacionSeguraAsync(
                destinatarios,
                ticket.IdTicket,
                material.IdMaterial,
                mensaje,
                "actualizar material del ticket");
        }

        public async Task EliminarMaterialDeTicketAsync(int idTicket, int idMaterial)
        {
            using var db = dbContextFactory.CreateDbContext();
            var detTicket = await db.DetTickets
                .SingleOrDefaultAsync(item => item.IdTicket == idTicket && item.IdMaterial == idMaterial);
            if (detTicket is null)
            {
                return;
            }

            if (notificationRecipients is null)
            {
                db.DetTickets.Remove(detTicket);
                await db.SaveChangesAsync();
                return;
            }

            var ticket = await db.Tickets.FindAsync(idTicket)
                ?? throw new KeyNotFoundException($"No existe el ticket {idTicket}.");
            var material = await db.Materials.FindAsync(idMaterial)
                ?? throw new KeyNotFoundException($"No existe el material {idMaterial}.");
            var destinatarios = notificationRecipients is null
                ? Array.Empty<int>()
                : await notificationRecipients.ObtenerDestinatariosCambioMaterialTicketAsync(
                    ticket.IdUsuario,
                    ticket.IdTecnico);
            var mensaje = NotificacionFactory.CrearMensajeMaterialTicket(
                ticket.IdTicket,
                ticket.IdTicket.ToString(),
                material.Nombre,
                detTicket.Cantidad,
                agregado: false);

            await EjecutarEnTransaccionAsync(db, async () =>
            {
                db.DetTickets.Remove(detTicket);
                await db.SaveChangesAsync();
                await GuardarNotificacionesAsync(
                    db,
                    destinatarios,
                    ticket.IdTicket,
                    material.IdMaterial,
                    mensaje);
            });

            await PublicarNotificacionSeguraAsync(
                destinatarios,
                ticket.IdTicket,
                material.IdMaterial,
                mensaje,
                "retirar material del ticket");
        }

        // Cálculos - Async
        public async Task<decimal> ObtenerCostoTotalMaterialesTicketAsync(int idTicket)
        {
            using var db = dbContextFactory.CreateDbContext();
            return await db.DetTickets
                .Where(dt => dt.IdTicket == idTicket)
                .SumAsync(dt => dt.CostoUnitarioAplicado * dt.Cantidad);
        }

        private static async Task GuardarNotificacionesAsync(
            HSisDbContext db,
            IReadOnlyList<int> destinatarios,
            int ticketId,
            int materialId,
            string mensaje)
        {
            if (destinatarios.Count == 0)
            {
                return;
            }

            db.Notificaciones.AddRange(destinatarios.Select(idUsuario => NotificacionFactory.Crear(
                idUsuario,
                ticketId,
                ConstantesTiposNotificacion.MaterialTicket,
                mensaje,
                materialId: materialId)));
            await db.SaveChangesAsync();
        }

        private static async Task EjecutarEnTransaccionAsync(
            HSisDbContext db,
            Func<Task> operacion)
        {
            IDbContextTransaction? transaction = null;
            if (db.Database.IsRelational())
            {
                transaction = await db.Database.BeginTransactionAsync();
            }

            try
            {
                await operacion();
                if (transaction is not null)
                {
                    await transaction.CommitAsync();
                }
            }
            catch
            {
                if (transaction is not null)
                {
                    await transaction.RollbackAsync();
                }

                throw;
            }
            finally
            {
                if (transaction is not null)
                {
                    await transaction.DisposeAsync();
                }
            }
        }

        private async Task PublicarNotificacionSeguraAsync(
            IReadOnlyList<int> destinatarios,
            int ticketId,
            int materialId,
            string mensaje,
            string evento)
        {
            if (notificationDispatcher is null || destinatarios.Count == 0)
            {
                return;
            }

            try
            {
                await notificationDispatcher.NotifyMaterialChangeAsync(
                    destinatarios,
                    ticketId,
                    materialId,
                    ConstantesTiposNotificacion.MaterialTicket,
                    mensaje);
            }
            catch (Exception ex)
            {
                logger?.LogError(
                    ex,
                    "No se pudo publicar la notificación de {Evento} para el ticket {TicketId} y material {MaterialId}. El historial persistido queda disponible para sincronización.",
                    evento,
                    ticketId,
                    materialId);
            }
        }

    }
}

