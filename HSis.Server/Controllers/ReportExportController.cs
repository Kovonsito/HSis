using HSis.Logic.DTOs;
using HSis.Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace HSis.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportExportController(IReportExportService reportExportService) : ControllerBase
    {
        [HttpPost("excel")]
        public IActionResult ExportarExcel([FromBody] ReporteExportRequestDto request)
        {
            var bytes = reportExportService.GenerarExcel(request.Kpis, request.Tickets, request.Inicio, request.Fin);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte_HSis.xlsx");
        }

        [HttpPost("pdf")]
        public IActionResult ExportarPdf([FromBody] ReporteExportRequestDto request)
        {
            var bytes = reportExportService.GenerarPdf(request.Kpis, request.Tickets, request.Inicio, request.Fin);
            return File(bytes, "application/pdf", "Reporte_HSis.pdf");
        }
    }
}
