namespace HSis.Contracts.Services
{
    public interface INotificadorTicket
    {
        Task NotificarTicketCreadoAsync(int idTicket, string folio, string descripcion);
    }
}

