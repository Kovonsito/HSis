#nullable enable
using System.Runtime.Versioning;
using FontAwesome.Sharp;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.UI.Helpers;

namespace HSis.UI.Forms.Otros
{
    [SupportedOSPlatform("windows")]
    public partial class KardexForm : Form
    {
        private readonly IMaterialService _materialService;

        public KardexForm(IMaterialService materialService)
        {
            InitializeComponent();
            _materialService = materialService;
            dgvKardex.AplicarTemaModerno();
        }

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
            lblEstadoKardex.Text = historial.Count == 0
                ? "No hay movimientos registrados para este material."
                : string.Empty;
            lblEstadoKardex.Visible = historial.Count == 0;

            var col1 = dgvKardex.Columns["IdMovimiento"];
            col1?.Visible = false;

            var col2 = dgvKardex.Columns["IdMaterial"];
            col2?.Visible = false;

            var col3 = dgvKardex.Columns["CostoUnitario"];
            col3?.DefaultCellStyle.Format = "C2";
        }

        #region Form Events
        private async void FrmKardex_Load(object? sender, EventArgs e)
        {
            picIcon.Image = FontAwesome.Sharp.IconChar.BoxesStacked.ToBitmap(TemaVisual.Primario, 24);
            await this.EjecutarOperacionAsync(async () =>
            {
                var materiales = await _materialService.ObtenerMaterialesAsync();
                CargarMateriales(materiales);
            }, "Error al cargar materiales", cbMaterial);
        }

        private void PanelTop_Paint(object? sender, PaintEventArgs e)
        {
            using var pen = new Pen(TemaVisual.BordeSutil, 1f);
            e.Graphics.DrawLine(pen, 0, panelTop.Height - 1, panelTop.Width, panelTop.Height - 1);
        }

        private async void CbMaterial_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cbMaterial.SelectedValue != null && cbMaterial.SelectedValue is int idMaterial)
            {
                await this.EjecutarOperacionAsync(async () =>
                {
                    var historial = await _materialService.ObtenerKardexPorMaterialAsync(idMaterial);
                    CargarHistorialKardex(historial);
                }, "Error al cargar Kardex", cbMaterial);
            }
            else
            {
                dgvKardex.DataSource = null;
                lblEstadoKardex.Text = "Selecciona un material para consultar sus movimientos.";
                lblEstadoKardex.Visible = true;
            }
        }
        #endregion
    }
}
