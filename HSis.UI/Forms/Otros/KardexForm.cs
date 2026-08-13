#nullable enable
using System.Runtime.Versioning;
using HSis.Data.Models;
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
        public void CargarMateriales(List<Material> materiales)
        {
            cbMaterial.SelectedIndexChanged -= CbMaterial_SelectedIndexChanged;
            cbMaterial.DataSource = materiales;
            cbMaterial.DisplayMember = "Nombre";
            cbMaterial.ValueMember = "IdMaterial";
            cbMaterial.SelectedIndex = -1;
            cbMaterial.SelectedIndexChanged += CbMaterial_SelectedIndexChanged;
        }

        public void CargarHistorialKardex(List<VHistorialInventario> historial)
        {
            dgvKardex.DataSource = new ListaVinculableOrdenable<VHistorialInventario>(historial);

            var col1 = dgvKardex.Columns["IdMovimientoUnico"];
            if (col1 != null) col1.Visible = false;

            var col2 = dgvKardex.Columns["IdMaterial"];
            if (col2 != null) col2.Visible = false;

            var col3 = dgvKardex.Columns["CostoUnitario"];
            if (col3 != null) col3.DefaultCellStyle.Format = "C2";

            var col4 = dgvKardex.Columns["ValorTotalMovimiento"];
            if (col4 != null) col4.DefaultCellStyle.Format = "C2";
        }

        public void MostrarError(string mensaje)
        {
            MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        public void MostrarCargando(bool cargando)
        {
            cbMaterial.Enabled = !cargando;
            this.UseWaitCursor = cargando;
        }
        #endregion

        #region Form Events
        private async void FrmKardex_Load(object? sender, EventArgs e)
        {
            await _presenter.CargarMaterialesAsync();
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
