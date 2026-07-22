using System.Net.Http;
using System.Net.Http.Json;
using HSis.Logic.DTOs;
using HSis.Logic.Services;

namespace HSis.UI.ApiClients
{
    public class ReportExportApiClientService(HttpClient httpClient) : IReportExportService
    {

        public byte[] GenerarExcel(ReporteKpisDto kpis, List<TicketDto> tickets, DateTime inicio, DateTime fin) => throw new NotImplementedException();
        public byte[] GenerarPdf(ReporteKpisDto kpis, List<TicketDto> tickets, DateTime inicio, DateTime fin) => throw new NotImplementedException();
    }
}
