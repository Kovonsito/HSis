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

    // Notificar creación de un nuevo ticket a Técnicos y Administradores
    public async Task NotifyTicketCreated(int ticketId, string ticketFolio, string titulo)
    {
        var message = $"Se ha registrado un nuevo ticket TK-{ticketFolio}: \"{titulo}\".";
        await Clients.Group("Role_Técnico").SendAsync("ReceiveNotification", "NuevoTicket", ticketId, message);
        await Clients.Group("Role_Administrador").SendAsync("ReceiveNotification", "NuevoTicket", ticketId, message);
        await Clients.Group("Role_Admin").SendAsync("ReceiveNotification", "NuevoTicket", ticketId, message);
    }

    // Notificar cambio de estado de ticket a un cliente específico y a Técnicos/Admins
    public async Task NotifyTicketStatusChanged(int clientUserId, int ticketId, string ticketFolio, string newStatus)
    {
        var message = $"El ticket TK-{ticketFolio} ha cambiado al estatus: {newStatus}.";
        await Clients.Group($"User_{clientUserId}").SendAsync("ReceiveNotification", "EstadoTicket", ticketId, message);
        await Clients.Group("Role_Técnico").SendAsync("ReceiveNotification", "EstadoTicket", ticketId, message);
        await Clients.Group("Role_Administrador").SendAsync("ReceiveNotification", "EstadoTicket", ticketId, message);
        await Clients.Group("Role_Admin").SendAsync("ReceiveNotification", "EstadoTicket", ticketId, message);
    }

    // Notificar calificación de ticket a un técnico específico o a administradores si no hay técnico
    public async Task NotifyTicketRated(int technicianUserId, int ticketId, string ticketFolio, int rating, string comment)
    {
        var stars = new string('⭐', rating);
        var message = $"El cliente calificó el ticket TK-{ticketFolio} con {stars} ({rating}/5). Comentario: \"{comment}\"";
        
        if (technicianUserId > 0)
        {
            await Clients.Group($"User_{technicianUserId}").SendAsync("ReceiveNotification", "Calificacion", ticketId, message);
        }

        // También notificar a administradores
        await Clients.Group("Role_Administrador").SendAsync("ReceiveNotification", "Calificacion", ticketId, message);
        await Clients.Group("Role_Admin").SendAsync("ReceiveNotification", "Calificacion", ticketId, message);
    }
}
