using Microsoft.AspNetCore.SignalR;

namespace HSis.Server.Hubs;

public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var userIdStr = httpContext?.Request.Query["userId"].ToString();
        var role = httpContext?.Request.Query["role"].ToString() ?? string.Empty;

        if (int.TryParse(userIdStr, out int userId))
        {
            // Agregar a grupo de usuario individual
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");

            // Agregar a grupo de rol
            if (!string.IsNullOrEmpty(role))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Role_{role}");
            }
        }

        await base.OnConnectedAsync();
    }

    // Métodos para retransmitir notificaciones enviados por los clientes (Técnicos o Clientes)

    // Notificar cambio de estado de ticket a un cliente específico
    public async Task NotifyTicketStatusChanged(int clientUserId, int ticketId, string ticketFolio, string newStatus)
    {
        var message = $"El ticket TK-{ticketFolio} ha cambiado al estatus: {newStatus}.";
        await Clients.Group($"User_{clientUserId}").SendAsync("ReceiveNotification", "EstadoTicket", ticketId, message);
    }

    // Notificar calificación de ticket a un técnico específico
    public async Task NotifyTicketRated(int technicianUserId, int ticketId, string ticketFolio, int rating, string comment)
    {
        var stars = new string('⭐', rating);
        var message = $"El cliente calificó el ticket TK-{ticketFolio} con {stars} ({rating}/5). Comentario: \"{comment}\"";
        await Clients.Group($"User_{technicianUserId}").SendAsync("ReceiveNotification", "Calificacion", ticketId, message);
    }
}
