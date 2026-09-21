#nullable enable
using System.Runtime.Versioning;
using FontAwesome.Sharp;
using HSis.Logic.DTOs;
using HSis.UI.Helpers;
using HSis.UI.Presenters;

namespace HSis.UI.Forms.Otros
{
    [SupportedOSPlatform("windows")]
    public partial class KardexForm : Form, IKardexView
    {
        private readonly KardexPresenter _presenter;

        public KardexForm(KardexPresenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;
            _presenter.SetView(this);
        }

        #region Propiedades de IKardexView
        public void CargarMateriales(List<MaterialDto> materiales)
        {
            cbMaterial.SelectedIndexChanged -= CbMaterial_SelectedIndexChanged;
            cbMaterial.DataSource = materiales;
            cbMaterial.DisplayMember = "Nombre";
            cbMaterial.ValueMember = "IdMaterial";
            cbMaterial.SelectedIndex = -1;
            cbMaterial.SelectedIndexChanged += CbMaterial_SelectedIndexChanged;
        }

        public void CargarHistorialKardex(List<KardexMovimientoDto> historial)
        {
            dgvKardex.DataSource = new ListaVinculableOrdenable<KardexMovimientoDto>(historial);
            dgvKardex.AplicarTemaModerno();

            var col1 = dgvKardex.Columns["IdMovimiento"];
            if (col1 != null) col1.Visible = false;

            var col2 = dgvKardex.Columns["IdMaterial"];
            if (col2 != null) col2.Visible = false;

            var col3 = dgvKardex.Columns["CostoUnitario"];
            if (col3 != null) col3.DefaultCellStyle.Format = "C2";
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


        public void MostrarCargando(bool cargando)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => MostrarCargando(cargando)));
                return;
            }
            cbMaterial.Enabled = !cargando;
            this.UseWaitCursor = cargando;
        }
        #endregion

        #region Form Events
        private async void FrmKardex_Load(object? sender, EventArgs e)
        {
            picIcon.Image = FontAwesome.Sharp.IconChar.BoxesStacked.ToBitmap(Color.FromArgb(37, 99, 235), 24);
            await _presenter.CargarMaterialesAsync();
        }

        private void PanelTop_Paint(object? sender, PaintEventArgs e)
        {
            using var pen = new Pen(Color.FromArgb(226, 232, 240), 1f);
            e.Graphics.DrawLine(pen, 0, panelTop.Height - 1, panelTop.Width, panelTop.Height - 1);
        }

        private async void CbMaterial_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cbMaterial.SelectedValue != null && cbMaterial.SelectedValue is int idMaterial)
            {
                await _presenter.CargarKardexPorMaterialAsync(idMaterial);
            }
            else
            {
                dgvKardex.DataSource = null;
            }
        }
        #endregion
    }
}

