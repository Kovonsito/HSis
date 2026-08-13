using HSis.Logic.DTOs;

namespace HSis.Logic.Services
{
    public interface IReportExportService
    {
        byte[] GenerarExcel(ReporteKpisDto kpis, List<TicketDto> tickets, DateTime inicio, DateTime fin);
        byte[] GenerarPdf(ReporteKpisDto kpis, List<TicketDto> tickets, DateTime inicio, DateTime fin);
    }
}
