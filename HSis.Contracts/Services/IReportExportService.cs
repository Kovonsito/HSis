using HSis.Contracts.DTOs;

namespace HSis.Contracts.Services
{
    public interface IReportExportService
    {
        Task<byte[]> GenerarExcelAsync(ReporteKpisDto kpis, List<TicketDto> tickets, DateTime inicio, DateTime fin);
        Task<byte[]> GenerarPdfAsync(ReporteKpisDto kpis, List<TicketDto> tickets, DateTime inicio, DateTime fin);
        byte[] GenerarExcel(ReporteKpisDto kpis, List<TicketDto> tickets, DateTime inicio, DateTime fin);
        byte[] GenerarPdf(ReporteKpisDto kpis, List<TicketDto> tickets, DateTime inicio, DateTime fin);
    }
}
