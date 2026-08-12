using System.IO;
using HSis.Logic.Constants;
using HSis.Logic.DTOs;
using HSis.Logic.Services;

namespace HSis.UI.Presenters
{
    public class GeneradorReportesPresenter(
        ITicketService ticketService,
        IReportExportService reportExportService)
    {
        private IGeneradorReportesView? _view;

        public void SetView(IGeneradorReportesView view)
        {
            _view = view;
        }

        public async Task GenerarReporteExcelAsync(string rutaArchivo)
        {
            if (_view == null) return;
            try
            {
                _view.MostrarCargando(true);

                var inicio = _view.FechaInicio.Date;
                var fin = _view.FechaFin.Date;

                var kpis = await ticketService.ObtenerReporteKpisAsync(inicio, fin);
                var filtro = new TicketFilterDto
                {
                    FechaAltaInicio = inicio,
                    FechaAltaFin = fin.AddDays(1).AddTicks(-1),
                    RangoTemporal = VistaTemporal.Todos
                };
                var tickets = await ticketService.ObtenerTicketsFiltradosAsync(filtro);

                var bytes = reportExportService.GenerarExcel(kpis, tickets, inicio, fin);
                await File.WriteAllBytesAsync(rutaArchivo, bytes);

                _view.MostrarExito("Reporte en Excel generado y guardado correctamente.");
            }
            catch (Exception ex)
            {
                _view.MostrarError($"Error al generar el reporte en Excel: {ex.Message}");
            }
            finally
            {
                _view.MostrarCargando(false);
            }
        }

        public async Task GenerarReportePdfAsync(string rutaArchivo)
        {
            if (_view == null) return;
            try
            {
                _view.MostrarCargando(true);

                var inicio = _view.FechaInicio.Date;
                var fin = _view.FechaFin.Date;

                var kpis = await ticketService.ObtenerReporteKpisAsync(inicio, fin);
                var filtro = new TicketFilterDto
                {
                    FechaAltaInicio = inicio,
                    FechaAltaFin = fin.AddDays(1).AddTicks(-1),
                    RangoTemporal = VistaTemporal.Todos
                };
                var tickets = await ticketService.ObtenerTicketsFiltradosAsync(filtro);

                var bytes = reportExportService.GenerarPdf(kpis, tickets, inicio, fin);
                await File.WriteAllBytesAsync(rutaArchivo, bytes);

                _view.MostrarExito("Reporte en PDF generado y guardado correctamente.");
            }
            catch (Exception ex)
            {
                _view.MostrarError($"Error al generar el reporte en PDF: {ex.Message}");
            }
            finally
            {
                _view.MostrarCargando(false);
            }
        }
    }
}
