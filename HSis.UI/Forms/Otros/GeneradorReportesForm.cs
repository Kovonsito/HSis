using System.ComponentModel;
using System.Drawing.Drawing2D;
using FontAwesome.Sharp;
using HSis.UI.Helpers;
using HSis.UI.Presenters;

namespace HSis.UI.Forms.Otros
{
    public partial class GeneradorReportesForm : Form, IGeneradorReportesView
    {
        private readonly GeneradorReportesPresenter _presenter;

        public GeneradorReportesForm(GeneradorReportesPresenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;
            _presenter.SetView(this);
        }

        #region Propiedades de IGeneradorReportesView
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTime FechaInicio
        {
            get => dtpInicio.Value;
            set => dtpInicio.Value = value;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTime FechaFin
        {
            get => dtpFin.Value;
            set => dtpFin.Value = value;
        }

        public void MostrarError(string mensaje)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => MostrarError(mensaje)));
                return;
            }
            MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void MostrarExito(string mensaje)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => MostrarExito(mensaje)));
                return;
            }
            MessageBox.Show(mensaje, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void MostrarCargando(bool cargando)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => MostrarCargando(cargando)));
                return;
            }
            btnExcel.Enabled = !cargando;
            btnPdf.Enabled = !cargando;
            Cursor = cargando ? Cursors.WaitCursor : Cursors.Default;
        }
        #endregion

        #region Form Events
        private void frmGeneradorReportes_Load(object sender, EventArgs e)
        {
            dtpInicio.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpFin.Value = DateTime.Today;
            picHeaderIcon.Image = IconChar.ChartPie.ToBitmap(Color.FromArgb(37, 99, 235), 28);
        }

        private void PnlHeader_Paint(object? sender, PaintEventArgs e)
        {
            using var pen = new Pen(Color.FromArgb(226, 232, 240), 1f);
            e.Graphics.DrawLine(pen, 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1);
        }

        private void PnlCard_Paint(object? sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(0, 0, pnlCard.Width - 1, pnlCard.Height - 1);
            using var path = TemaVisual.CrearRectanguloRedondeado(rect, 10);
            using var pen = new Pen(Color.FromArgb(226, 232, 240), 1.5f);
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
                await _presenter.GenerarReporteExcelAsync(sfd.FileName);
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
                await _presenter.GenerarReportePdfAsync(sfd.FileName);
            }
        }
        #endregion
    }
}
