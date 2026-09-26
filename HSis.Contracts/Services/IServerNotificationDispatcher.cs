namespace HSis.Contracts.Services;

public interface IServerNotificationDispatcher
{
    Task NotifyTicketCreatedAsync(int ticketId, string ticketFolio, string titulo);
    Task NotifyTicketStatusChangedAsync(int clientUserId, int ticketId, string ticketFolio, string newStatus);
    Task NotifyTicketRatedAsync(int technicianUserId, int ticketId, string ticketFolio, int rating, string comment);
    Task NotifyTicketChangeAsync(
        IReadOnlyList<int> recipientUserIds,
        int ticketId,
        string notificationType,
        string message);

    Task NotifyMaterialChangeAsync(
        IReadOnlyList<int> recipientUserIds,
        int? ticketId,
        int? materialId,
        string notificationType,
        string message);
}

