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
        private readonly IAdministradorSesionUsuario _contextoSesion;

        public KardexForm(IMaterialService materialService, IAdministradorSesionUsuario contextoSesion)
        {
            InitializeComponent();
            _materialService = materialService;
            _contextoSesion = contextoSesion;
            dgvKardex.AplicarTemaModerno();
            panelTop.Resize += (_, _) => AjustarAnchoCombo();
        }

        // El anclaje a la derecha se desfasa con el escalado de DPI; se recalcula el ancho para que no sobresalga del formulario.
        private void AjustarAnchoCombo()
        {
            cbMaterial.Width = Math.Max(120, panelTop.ClientSize.Width - cbMaterial.Left - dgvKardex.Left);
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
            if (col1 != null) col1.Visible = false;

            var col2 = dgvKardex.Columns["IdMaterial"];
            if (col2 != null) col2.Visible = false;

            var colIdUsuario = dgvKardex.Columns["IdUsuario"];
            if (colIdUsuario != null) colIdUsuario.Visible = false;

            var colMat = dgvKardex.Columns["MaterialNombre"];
            if (colMat != null)
            {
                colMat.HeaderText = "Material";
                colMat.FillWeight = 25;
            }

            var colUsuario = dgvKardex.Columns["UsuarioNombre"];
            if (colUsuario != null)
            {
                colUsuario.HeaderText = "Responsable";
                colUsuario.FillWeight = 25;
            }

            var colFecha = dgvKardex.Columns["Fecha"];
            if (colFecha != null)
            {
                colFecha.HeaderText = "Fecha";
                colFecha.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                colFecha.FillWeight = 20;
            }

            var colCant = dgvKardex.Columns["Cantidad"];
            if (colCant != null)
            {
                colCant.HeaderText = "Cantidad";
                colCant.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                colCant.FillWeight = 12;
            }

            var colTipo = dgvKardex.Columns["TipoMovimiento"];
            if (colTipo != null)
            {
                colTipo.HeaderText = "Tipo";
                colTipo.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                colTipo.FillWeight = 12;
            }

            var colMotivo = dgvKardex.Columns["Motivo"];
            if (colMotivo != null)
            {
                colMotivo.HeaderText = "Motivo";
                colMotivo.FillWeight = 30;
            }

            var col3 = dgvKardex.Columns["CostoUnitario"];
            if (col3 != null)
            {
                col3.HeaderText = "Costo Unit.";
                col3.DefaultCellStyle.Format = "C2";
                col3.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                col3.Visible = _contextoSesion.EsAdmin;
                col3.FillWeight = 15;
            }
        }

        #region Form Events
        private async void FrmKardex_Load(object? sender, EventArgs e)
        {
            AjustarAnchoCombo();
            picIcon.Image = FontAwesome.Sharp.IconChar.BoxesStacked.ToBitmap(TemaVisual.Primario, 24);
            await this.EjecutarOperacionAsync(async () =>
            {
                var materiales = await _materialService.ObtenerMaterialesAsync();
                CargarMateriales(materiales);
            }, "Error al cargar materiales", "kardex-materiales", cbMaterial);
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
                }, "Error al cargar Kardex", "kardex-historial", cbMaterial);
            }
            else
            {
                dgvKardex.DataSource = null;
                lblEstadoKardex.Text = "Selecciona un material para consultar sus movimientos.";
                lblEstadoKardex.Visible = true;
            }
        }
        #endregion

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED: composición nativa sin parpadeos
                return cp;
            }
        }
    }
}
