namespace HSis.Logic.Services;

public interface IServerNotificationDispatcher
{
    Task NotifyTicketCreatedAsync(int ticketId, string ticketFolio, string titulo);
    Task NotifyTicketStatusChangedAsync(int clientUserId, int ticketId, string ticketFolio, string newStatus);
    Task NotifyTicketRatedAsync(int technicianUserId, int ticketId, string ticketFolio, int rating, string comment);
}
