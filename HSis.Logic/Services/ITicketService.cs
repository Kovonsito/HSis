using HSis.Logic.DTOs;

namespace HSis.Logic.Services
{
    public interface ITicketService
    {
        Task<List<TicketDto>> ObtenerTicketsAsync();
        Task<TicketDto?> ObtenerTicketPorIdAsync(int id);
        Task<List<TicketDto>> ObtenerTicketsPorSLAAsync(bool esUrgente);
        Task<List<TicketDto>> ObtenerTicketsPorEstatusAsync(string estatus);
        Task<int> ObtenerCountTicketsPorSLAAsync(bool esUrgente);
        Task<int> ObtenerCountTicketsPorEstatusAsync(string estatus);
        Task<List<object>> ObtenerHistorialPorTicketAsync(int idTicket);
        Task ActualizarTicketAsync(TicketUpdateDto ticketDto);
        Task<List<TicketDto>> ObtenerTicketsPorUsuarioAsync(int idUsuario);
        Task<List<TicketDto>> ObtenerTicketsAsignadosATecnicoAsync(int idTecnico);
        Task<List<TicketDto>> ObtenerTicketsCerradosPorTecnicoAsync(int idTecnico);
        Task<List<TicketDto>> ObtenerTicketsDisponiblesAsync();
        Task<TicketDto> CrearTicketAsync(TicketCreateDto ticketDto);
        Task<List<TicketDto>> ObtenerTicketsFiltradosAsync(TicketFilterDto filtros);
        Task<PaginatedResultDto<TicketDto>> ObtenerTicketsFiltradosPaginadosAsync(TicketFilterDto filtros, int pageNumber, int pageSize);
        Task<ReporteKpisDto> ObtenerReporteKpisAsync(DateTime inicio, DateTime fin);
        Task<bool> RegistrarCalificacionAsync(int idTicket, int calificacion, string? comentario);
        Task<double> ObtenerPromedioCalificacionTecnicoAsync(int idTecnico);
        Task<List<TicketDto>> ObtenerFeedbackTecnicoAsync(int idTecnico);
    }
}
