using HSis.Logic.DTOs;

namespace HSis.Logic.Services
{
    public interface ITicketCommandService
    {
        Task<TicketDto> CrearTicketAsync(TicketCreateDto ticketDto);
        Task ActualizarTicketAsync(TicketUpdateDto ticketDto);
        Task<bool> RegistrarCalificacionAsync(int idTicket, int calificacion, string? comentario);
    }
}
