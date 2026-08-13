namespace HSis.Logic.Services;

public interface INotificadorTicket
{
    Task NotificarTicketCreadoAsync(int idTicket, string folio, string descripcion);
}
