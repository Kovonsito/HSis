using HSis.Logic.DTOs;

namespace HSis.Logic.Services
{
    public interface ITicketQueryService
    {
        Task<List<TicketDto>> ObtenerTicketsAsync();
        Task<TicketDto?> ObtenerTicketPorIdAsync(int id);
        Task<List<TicketDto>> ObtenerTicketsPorSLAAsync(bool esUrgente);
        Task<List<TicketDto>> ObtenerTicketsPorEstatusAsync(string estatus);
        Task<int> ObtenerCountTicketsPorSLAAsync(bool esUrgente);
        Task<int> ObtenerCountTicketsPorEstatusAsync(string estatus);
        Task<List<HistorialCambiosDto>> ObtenerHistorialPorTicketAsync(int idTicket);
        Task<List<TicketDto>> ObtenerTicketsPorUsuarioAsync(int idUsuario);
        Task<List<TicketDto>> ObtenerTicketsAsignadosATecnicoAsync(int idTecnico);
        Task<List<TicketDto>> ObtenerTicketsCerradosPorTecnicoAsync(int idTecnico);
        Task<List<TicketDto>> ObtenerTicketsDisponiblesAsync();
        Task<List<TicketDto>> ObtenerTicketsFiltradosAsync(TicketFilterDto filtros);
        Task<PaginatedResultDto<TicketDto>> ObtenerTicketsFiltradosPaginadosAsync(TicketFilterDto filtros, int pageNumber, int pageSize);
    }
}
