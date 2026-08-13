using System.ComponentModel;
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
            InicializarLayoutReportes();
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
            MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void MostrarExito(string mensaje)
        {
            MessageBox.Show(mensaje, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        public void MostrarCargando(bool cargando)
        {
            btnExcel.Enabled = !cargando;
            btnPdf.Enabled = !cargando;
            this.Cursor = cargando ? Cursors.WaitCursor : Cursors.Default;
        }
        #endregion

        #region Form Events
        private void frmGeneradorReportes_Load(object sender, EventArgs e)
        {
            dtpInicio.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpFin.Value = DateTime.Today;
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

