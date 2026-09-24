#nullable enable
using System.Drawing.Drawing2D;
using System.IO;
using FontAwesome.Sharp;
using HSis.Contracts.Constants;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.UI.Helpers;

namespace HSis.UI.Forms.Otros
{
    public partial class GeneradorReportesForm : Form
    {
        private readonly ITicketService _ticketService;
        private readonly IReportExportService _reportExportService;

        public GeneradorReportesForm(
            ITicketService ticketService,
            IReportExportService reportExportService)
        {
            InitializeComponent();
            _ticketService = ticketService;
            _reportExportService = reportExportService;
        }

        #region Form Events
        private void frmGeneradorReportes_Load(object sender, EventArgs e)
        {
            dtpInicio.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpFin.Value = DateTime.Today;
            picHeaderIcon.Image = IconChar.ChartPie.ToBitmap(TemaVisual.Primario, 28);
        }

        private void PnlHeader_Paint(object? sender, PaintEventArgs e)
        {
            using var pen = new Pen(TemaVisual.BordeSutil, 1f);
            e.Graphics.DrawLine(pen, 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1);
        }

        private void PnlCard_Paint(object? sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(0, 0, pnlCard.Width - 1, pnlCard.Height - 1);
            using var path = TemaVisual.CrearRectanguloRedondeado(rect, 10);
            using var pen = new Pen(TemaVisual.BordeSutil, 1.5f);
            e.Graphics.DrawPath(pen, path);
        }

        private async void btnExcel_Click(object sender, EventArgs e)
        {
            var inicio = dtpInicio.Value.Date;
            var fin = dtpFin.Value.Date;

            using var sfd = new SaveFileDialog
            {
                Filter = "Archivos de Excel (*.xlsx)|*.xlsx",
                FileName = $"Reporte_Tickets_{inicio:yyyyMMdd}_a_{fin:yyyyMMdd}.xlsx"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                await this.EjecutarOperacionAsync(async () =>
                {
                    var kpis = await _ticketService.ObtenerReporteKpisAsync(inicio, fin);
                    var filtro = new TicketFilterDto
                    {
                        FechaAltaInicio = inicio,
                        FechaAltaFin = fin.AddDays(1).AddTicks(-1),
                        RangoTemporal = VistaTemporal.Todos
                    };
                    var tickets = await _ticketService.ObtenerTicketsFiltradosAsync(filtro);
                    var bytes = await _reportExportService.GenerarExcelAsync(kpis, tickets, inicio, fin);
                    await File.WriteAllBytesAsync(sfd.FileName, bytes);
                    DialogoUIHelper.MostrarExito("Reporte en Excel generado y guardado correctamente.");
                }, "Error al generar el reporte en Excel", btnExcel, btnPdf);
            }
        }

        private async void btnPdf_Click(object sender, EventArgs e)
        {
            var inicio = dtpInicio.Value.Date;
            var fin = dtpFin.Value.Date;

            using var sfd = new SaveFileDialog
            {
                Filter = "Archivos PDF (*.pdf)|*.pdf",
                FileName = $"Reporte_Tickets_{inicio:yyyyMMdd}_a_{fin:yyyyMMdd}.pdf"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                await this.EjecutarOperacionAsync(async () =>
                {
                    var kpis = await _ticketService.ObtenerReporteKpisAsync(inicio, fin);
                    var filtro = new TicketFilterDto
                    {
                        FechaAltaInicio = inicio,
                        FechaAltaFin = fin.AddDays(1).AddTicks(-1),
                        RangoTemporal = VistaTemporal.Todos
                    };
                    var tickets = await _ticketService.ObtenerTicketsFiltradosAsync(filtro);
                    var bytes = await _reportExportService.GenerarPdfAsync(kpis, tickets, inicio, fin);
                    await File.WriteAllBytesAsync(sfd.FileName, bytes);
                    DialogoUIHelper.MostrarExito("Reporte en PDF generado y guardado correctamente.");
                }, "Error al generar el reporte en PDF", btnExcel, btnPdf);
            }
        }
        #endregion
    }
}
