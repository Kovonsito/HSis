using System.IO;
using HSis.Logic.DTOs;
using HSis.Logic.Services;

namespace HSis.UI.Forms.Otros
{
    public partial class GeneradorReportesForm : Form
    {
        private readonly ITicketService _ticketService;
        private readonly IReportExportService _reportExportService;

        public GeneradorReportesForm(ITicketService ticketService, IReportExportService reportExportService)
        {
            InitializeComponent();
            _ticketService = ticketService;
            _reportExportService = reportExportService;
            InicializarLayoutReportes();
        }

        private void InicializarLayoutReportes()
        {
            var tblPrincipal = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 4,
                ColumnCount = 1,
                Padding = new Padding(20),
                Name = "tblPrincipal",
                BackColor = System.Drawing.Color.White
            };
            tblPrincipal.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Fila 0: Titulo
            tblPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Fila 1: Grid de campos
            tblPrincipal.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F)); // Fila 2: Separador
            tblPrincipal.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Fila 3: Botones

            lblTitle.Dock = DockStyle.Fill;
            lblTitle.Margin = new Padding(0, 0, 0, 15);

            // Grid de Fechas
            var tblFechas = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 2,
                Margin = new Padding(0),
                BackColor = System.Drawing.Color.White
            };
            tblFechas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
            tblFechas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblFechas.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tblFechas.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            lblInicio.Dock = DockStyle.Fill;
            lblInicio.TextAlign = ContentAlignment.MiddleLeft;
            dtpInicio.Dock = DockStyle.Fill;

            lblFin.Dock = DockStyle.Fill;
            lblFin.TextAlign = ContentAlignment.MiddleLeft;
            dtpFin.Dock = DockStyle.Fill;

            tblFechas.Controls.Add(lblInicio, 0, 0);
            tblFechas.Controls.Add(dtpInicio, 1, 0);
            tblFechas.Controls.Add(lblFin, 0, 1);
            tblFechas.Controls.Add(dtpFin, 1, 1);

            // FlowLayoutPanel para botones
            var flpBotones = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Margin = new Padding(0),
                BackColor = System.Drawing.Color.White
            };
            btnExcel.Margin = new Padding(0, 0, 15, 0);
            btnExcel.Dock = DockStyle.None;
            btnPdf.Margin = new Padding(0);
            btnPdf.Dock = DockStyle.None;
            flpBotones.Controls.Add(btnExcel);
            flpBotones.Controls.Add(btnPdf);

            tblPrincipal.Controls.Add(lblTitle, 0, 0);
            tblPrincipal.Controls.Add(tblFechas, 0, 1);
            tblPrincipal.Controls.Add(new Panel { Dock = DockStyle.Fill, Margin = new Padding(0), BackColor = System.Drawing.Color.White }, 0, 2); // Espaciador
            tblPrincipal.Controls.Add(flpBotones, 0, 3);

            this.Controls.Remove(pnlBackground);
            this.Controls.Add(tblPrincipal);
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
