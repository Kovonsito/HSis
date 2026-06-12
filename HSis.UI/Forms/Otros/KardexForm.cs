#nullable enable
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;
using HSis.Data.Models;
using HSis.Logic.Services;
using HSis.UI.Helpers;

namespace HSis.UI.Forms.Otros
{
    [SupportedOSPlatform("windows")]
    public partial class KardexForm : Form
    {
        private readonly ICatalogoService _catalogoService;

        public KardexForm(ICatalogoService catalogoService)
        {
            _catalogoService = catalogoService;
            InitializeComponent();
        }

        private async void FrmKardex_Load(object? sender, EventArgs e)
        {
            var materiales = await _catalogoService.ObtenerTodosAsync<Material>();

            // Desenlazamos el evento temporalmente para que no se dispare al cargar datos
            cbMaterial.SelectedIndexChanged -= CbMaterial_SelectedIndexChanged;

            cbMaterial.DataSource = materiales;
            cbMaterial.DisplayMember = "Nombre";
            cbMaterial.ValueMember = "IdMaterial";
            cbMaterial.SelectedIndex = -1; // Sin selección por defecto

            cbMaterial.SelectedIndexChanged += CbMaterial_SelectedIndexChanged;
        }

        private async void CbMaterial_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cbMaterial.SelectedValue != null && cbMaterial.SelectedValue is int idMaterial)
            {
                try
                {
                    // Obtenemos los registros filtrados asíncronamente y los ordenamos en memoria
                    var historialCompleto = await _catalogoService.ObtenerFiltradoAsync<VHistorialInventario>(h => h.IdMaterial == idMaterial);
                    var historialFiltradoYOrdenado = Enumerable.ToList(Enumerable.OrderByDescending(historialCompleto, h => h.Fecha));
                    dgvKardex.DataSource = new ListaVinculableOrdenable<VHistorialInventario>(historialFiltradoYOrdenado);

                    // Formatear columnas
                    var col1 = dgvKardex.Columns["IdMovimientoUnico"];
                    if (col1 != null) col1.Visible = false;

                    var col2 = dgvKardex.Columns["IdMaterial"];
                    if (col2 != null) col2.Visible = false;

                    var col3 = dgvKardex.Columns["CostoUnitario"];
                    if (col3 != null) col3.DefaultCellStyle.Format = "C2";

                    var col4 = dgvKardex.Columns["ValorTotalMovimiento"];
                    if (col4 != null) col4.DefaultCellStyle.Format = "C2";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar el Kardex: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                dgvKardex.DataSource = null;
            }
        }
    }
}
