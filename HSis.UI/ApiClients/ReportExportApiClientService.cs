using System.Net.Http;
using System.Net.Http.Json;
using HSis.Logic.DTOs;
using HSis.Logic.Services;

namespace HSis.UI.ApiClients
{
    public class ReportExportApiClientService(HttpClient httpClient) : IReportExportService
    {
        public async Task<byte[]> GenerarExcelAsync(ReporteKpisDto kpis, List<TicketDto> tickets, DateTime inicio, DateTime fin)
        {
            var request = new ReporteExportRequestDto { Kpis = kpis, Tickets = tickets, Inicio = inicio, Fin = fin };
            var response = await httpClient.PostAsJsonAsync("api/ReportExport/excel", request);
            await response.EnsureSuccessStatusCodeWithDetailsAsync();
            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<byte[]> GenerarPdfAsync(ReporteKpisDto kpis, List<TicketDto> tickets, DateTime inicio, DateTime fin)
        {
            var request = new ReporteExportRequestDto { Kpis = kpis, Tickets = tickets, Inicio = inicio, Fin = fin };
            var response = await httpClient.PostAsJsonAsync("api/ReportExport/pdf", request);
            await response.EnsureSuccessStatusCodeWithDetailsAsync();
            return await response.Content.ReadAsByteArrayAsync();
        }

        public byte[] GenerarExcel(ReporteKpisDto kpis, List<TicketDto> tickets, DateTime inicio, DateTime fin)
            => GenerarExcelAsync(kpis, tickets, inicio, fin).GetAwaiter().GetResult();

        public byte[] GenerarPdf(ReporteKpisDto kpis, List<TicketDto> tickets, DateTime inicio, DateTime fin)
            => GenerarPdfAsync(kpis, tickets, inicio, fin).GetAwaiter().GetResult();
    }
}
