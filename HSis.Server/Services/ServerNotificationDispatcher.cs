using HSis.Contracts.Services;
using HSis.Server.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace HSis.Server.Services
{
    public class ServerNotificationDispatcher(IHubContext<NotificationHub> hubContext) : IServerNotificationDispatcher
    {
        private static string ObtenerFolioNumerico(string? folio)
        {
            if (string.IsNullOrWhiteSpace(folio))
            {
                return "0";
            }

            var digitos = new string(folio.Where(char.IsDigit).ToArray()).TrimStart('0');
            return string.IsNullOrEmpty(digitos) ? "0" : digitos;
        }

        public async Task NotifyTicketCreatedAsync(int ticketId, string ticketFolio, string titulo)
        {
            var message = $"Se ha registrado un nuevo ticket {ObtenerFolioNumerico(ticketFolio)}: \"{titulo}\".";
            await hubContext.Clients.Group("Role_Técnico").SendAsync("ReceiveNotification", "NuevoTicket", ticketId, message);
            await hubContext.Clients.Group("Role_Administrador").SendAsync("ReceiveNotification", "NuevoTicket", ticketId, message);
        }

        public async Task NotifyTicketStatusChangedAsync(int clientUserId, int ticketId, string ticketFolio, string newStatus)
        {
            var message = $"El ticket {ObtenerFolioNumerico(ticketFolio)} ha cambiado al estatus: {newStatus}.";
            await hubContext.Clients.Group($"User_{clientUserId}").SendAsync("ReceiveNotification", "EstadoTicket", ticketId, message);
        }

        public async Task NotifyTicketRatedAsync(int technicianUserId, int ticketId, string ticketFolio, int rating, string comment)
        {
            var stars = new string('⭐', rating);
            var message = $"El cliente calificó el ticket {ObtenerFolioNumerico(ticketFolio)} con {stars} ({rating}/5). Comentario: \"{comment}\"";

            if (technicianUserId > 0)
            {
                await hubContext.Clients.Group($"User_{technicianUserId}").SendAsync("ReceiveNotification", "Calificacion", ticketId, message);
            }

            await hubContext.Clients.Group("Role_Administrador").SendAsync("ReceiveNotification", "Calificacion", ticketId, message);
        }
    }
}

