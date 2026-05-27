using System;
using System.IO;
using System.Windows.Forms;
using HSis.Logic.DTOs;
using HSis.Logic.Services;

namespace HSis.UI
{
    public partial class frmGeneradorReportes : Form
    {
        private readonly TicketService _ticketService;
        private readonly ReportExportService _reportExportService;

        public frmGeneradorReportes(TicketService ticketService, ReportExportService reportExportService)
        {
            InitializeComponent();
            _ticketService = ticketService;
            _reportExportService = reportExportService;
        }

        private void frmGeneradorReportes_Load(object sender, EventArgs e)
        {
            // Inicializar las fechas (por defecto el mes actual)
            dtpInicio.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpFin.Value = DateTime.Today;
        }

        private async void btnExcel_Click(object sender, EventArgs e)
        {
            try
            {
                btnExcel.Enabled = false;
                btnPdf.Enabled = false;
                Cursor = Cursors.WaitCursor;

                var inicio = dtpInicio.Value.Date;
                var fin = dtpFin.Value.Date;

                // 1. Obtener datos
                var kpis = await _ticketService.ObtenerReporteKpisAsync(inicio, fin);
                
                // Para el detalle, filtramos todos los tickets del periodo
                var filtro = new TicketFilterDto
                {
                    FechaAltaInicio = inicio,
                    FechaAltaFin = fin.AddDays(1).AddTicks(-1),
                    RangoTemporal = VistaTemporal.Todos
                };
                var tickets = await _ticketService.ObtenerTicketsFiltradosAsync(filtro);

                // 2. Generar Excel
                var bytes = _reportExportService.GenerarExcel(kpis, tickets, inicio, fin);

                // 3. Guardar Archivo
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Archivos de Excel (*.xlsx)|*.xlsx";
                    sfd.FileName = $"Reporte_Tickets_{inicio:yyyyMMdd}_a_{fin:yyyyMMdd}.xlsx";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        await File.WriteAllBytesAsync(sfd.FileName, bytes);
                        MessageBox.Show("Reporte en Excel generado y guardado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el reporte en Excel: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnExcel.Enabled = true;
                btnPdf.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private async void btnPdf_Click(object sender, EventArgs e)
        {
            try
            {
                btnExcel.Enabled = false;
                btnPdf.Enabled = false;
                Cursor = Cursors.WaitCursor;

                var inicio = dtpInicio.Value.Date;
                var fin = dtpFin.Value.Date;

                // 1. Obtener datos
                var kpis = await _ticketService.ObtenerReporteKpisAsync(inicio, fin);
                
                var filtro = new TicketFilterDto
                {
                    FechaAltaInicio = inicio,
                    FechaAltaFin = fin.AddDays(1).AddTicks(-1),
                    RangoTemporal = VistaTemporal.Todos
                };
                var tickets = await _ticketService.ObtenerTicketsFiltradosAsync(filtro);

                // 2. Generar PDF
                var bytes = _reportExportService.GenerarPdf(kpis, tickets, inicio, fin);

                // 3. Guardar Archivo
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Archivos PDF (*.pdf)|*.pdf";
                    sfd.FileName = $"Reporte_Tickets_{inicio:yyyyMMdd}_a_{fin:yyyyMMdd}.pdf";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        await File.WriteAllBytesAsync(sfd.FileName, bytes);
                        MessageBox.Show("Reporte en PDF generado y guardado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el reporte en PDF: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnExcel.Enabled = true;
                btnPdf.Enabled = true;
                Cursor = Cursors.Default;
            }
        }
    }
}
