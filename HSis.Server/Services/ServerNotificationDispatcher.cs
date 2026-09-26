using HSis.Contracts.Constants;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.Logic.Services;
using HSis.Server.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace HSis.Server.Services
{
    public class ServerNotificationDispatcher(
        IHubContext<NotificationHub, INotificationClient> hubContext) : IServerNotificationDispatcher
    {
        public async Task NotifyTicketCreatedAsync(int ticketId, string ticketFolio, string titulo)
        {
            var notification = CrearNotificacion(
                ticketId,
                ConstantesTiposNotificacion.NuevoTicket,
                NotificacionFactory.CrearMensajeNuevoTicket(ticketId, ticketFolio, titulo));
            await hubContext.Clients.Groups(
                new[] { GruposNotificaciones.Tecnicos, GruposNotificaciones.Administradores })
                .ReceiveNotification(notification);
        }

        public async Task NotifyTicketStatusChangedAsync(int clientUserId, int ticketId, string ticketFolio, string newStatus)
        {
            var notification = CrearNotificacion(
                ticketId,
                ConstantesTiposNotificacion.EstadoTicket,
                NotificacionFactory.CrearMensajeCambioEstado(ticketId, ticketFolio, newStatus));
            await hubContext.Clients.Group(GruposNotificaciones.Usuario(clientUserId))
                .ReceiveNotification(notification);
        }

        public async Task NotifyTicketRatedAsync(int technicianUserId, int ticketId, string ticketFolio, int rating, string comment)
        {
            var notification = CrearNotificacion(
                ticketId,
                ConstantesTiposNotificacion.Calificacion,
                NotificacionFactory.CrearMensajeCalificacion(ticketId, ticketFolio, rating, comment));

            if (technicianUserId > 0)
            {
                await hubContext.Clients.Group(GruposNotificaciones.Usuario(technicianUserId))
                    .ReceiveNotification(notification);
            }

            await hubContext.Clients.Group(GruposNotificaciones.Administradores)
                .ReceiveNotification(notification);
        }

        public Task NotifyTicketChangeAsync(
            IReadOnlyList<int> recipientUserIds,
            int ticketId,
            string notificationType,
            string message)
        {
            ArgumentNullException.ThrowIfNull(recipientUserIds);
            ArgumentException.ThrowIfNullOrWhiteSpace(notificationType);
            ArgumentException.ThrowIfNullOrWhiteSpace(message);

            var grupos = recipientUserIds
                .Where(userId => userId > 0)
                .Distinct()
                .Select(GruposNotificaciones.Usuario)
                .ToArray();

            if (grupos.Length == 0)
            {
                return Task.CompletedTask;
            }

            return hubContext.Clients.Groups(grupos).ReceiveNotification(
                CrearNotificacion(ticketId, notificationType, message));
        }

        public Task NotifyMaterialChangeAsync(
            IReadOnlyList<int> recipientUserIds,
            int? ticketId,
            int? materialId,
            string notificationType,
            string message)
        {
            ArgumentNullException.ThrowIfNull(recipientUserIds);
            ArgumentException.ThrowIfNullOrWhiteSpace(notificationType);
            ArgumentException.ThrowIfNullOrWhiteSpace(message);

            var grupos = recipientUserIds
                .Where(userId => userId > 0)
                .Distinct()
                .Select(GruposNotificaciones.Usuario)
                .ToArray();

            if (grupos.Length == 0)
            {
                return Task.CompletedTask;
            }

            return hubContext.Clients.Groups(grupos).ReceiveNotification(
                CrearNotificacion(ticketId, materialId, notificationType, message));
        }

        private static NotificacionDto CrearNotificacion(int ticketId, string tipo, string mensaje)
            => CrearNotificacion(ticketId, null, tipo, mensaje);

        private static NotificacionDto CrearNotificacion(
            int? ticketId,
            int? materialId,
            string tipo,
            string mensaje)
            => new(0, ticketId, tipo, mensaje, DateTimeOffset.Now, false, materialId);
    }
}

