using System;
using System.Drawing;
using System.Windows.Forms;
using HSis.Data.Models;
using HSis.Logic.Services;
using System.Threading.Tasks;

namespace HSis.UI
{
    public partial class frmKardex : Form
    {
        private readonly CatalogoService _catalogoService;

        public frmKardex(CatalogoService catalogoService)
        {
            _catalogoService = catalogoService;
            InitializeComponent();
        }

        private async void FrmKardex_Load(object sender, EventArgs e)
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

        private async void CbMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbMaterial.SelectedValue != null && cbMaterial.SelectedValue is int idMaterial)
            {
                try
                {
                    // Obtenemos los registros filtrados asíncronamente y los ordenamos en memoria
                    var historialCompleto = await _catalogoService.ObtenerFiltradoAsync<VHistorialInventario>(h => h.IdMaterial == idMaterial);
                    var historialFiltradoYOrdenado = System.Linq.Enumerable.ToList(System.Linq.Enumerable.OrderByDescending(historialCompleto, h => h.Fecha));
                    dgvKardex.DataSource = historialFiltradoYOrdenado;
                    
                    // Formatear columnas
                    if (dgvKardex.Columns["IdMovimientoUnico"] != null)
                        dgvKardex.Columns["IdMovimientoUnico"].Visible = false;
                        
                    if (dgvKardex.Columns["IdMaterial"] != null)
                        dgvKardex.Columns["IdMaterial"].Visible = false;

                    if (dgvKardex.Columns["CostoUnitario"] != null)
                        dgvKardex.Columns["CostoUnitario"].DefaultCellStyle.Format = "C2";
                        
                    if (dgvKardex.Columns["ValorTotalMovimiento"] != null)
                        dgvKardex.Columns["ValorTotalMovimiento"].DefaultCellStyle.Format = "C2";
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
