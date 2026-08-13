namespace HSis.Logic.Services;

public interface IServerNotificationDispatcher : INotificadorTicket
{
    Task NotifyTicketCreatedAsync(int ticketId, string ticketFolio, string titulo);
    Task NotifyTicketStatusChangedAsync(int clientUserId, int ticketId, string ticketFolio, string newStatus);
    Task NotifyTicketRatedAsync(int technicianUserId, int ticketId, string ticketFolio, int rating, string comment);

    Task INotificadorTicket.NotificarTicketCreadoAsync(int idTicket, string folio, string descripcion)
    {
        return NotifyTicketCreatedAsync(idTicket, folio, descripcion);
    }

}
