using System.Net.Http;
using HSis.Logic.DTOs;
using HSis.Logic.Services;

namespace HSis.UI.ApiClients
{
#pragma warning disable CS9113 // Parameter is required by AddHttpClient DI registration
    public class ReportExportApiClientService(HttpClient httpClient) : IReportExportService
#pragma warning restore CS9113
    {
        private readonly ReportExportService _exportService = new();

        public byte[] GenerarExcel(ReporteKpisDto kpis, List<TicketDto> tickets, DateTime inicio, DateTime fin)
        {
            return _exportService.GenerarExcel(kpis, tickets, inicio, fin);
        }


        public byte[] GenerarPdf(ReporteKpisDto kpis, List<TicketDto> tickets, DateTime inicio, DateTime fin)
        {
            return _exportService.GenerarPdf(kpis, tickets, inicio, fin);
        }

    }
}

