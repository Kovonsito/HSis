using HSis.Logic.DTOs;

namespace HSis.Logic.Services
{
    public interface ITicketKpiService
    {
        Task<ReporteKpisDto> ObtenerReporteKpisAsync(DateTime inicio, DateTime fin);
        Task<double> ObtenerPromedioCalificacionTecnicoAsync(int idTecnico);
        Task<List<TicketDto>> ObtenerFeedbackTecnicoAsync(int idTecnico);
    }
}

