using HSis.UI.Factories;
using HSis.UI.Forms.Tickets;

namespace HSis.UI.Helpers
{
    public static class ExtensionesDataGridView
    {
        public static void AutoajustarAnchosMinimos(this DataGridView dgv)
        {
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                if (col.Visible)
                {
                    int prefHeader = col.GetPreferredWidth(DataGridViewAutoSizeColumnMode.ColumnHeader, true);
                    col.MinimumWidth = Math.Max(col.MinimumWidth, prefHeader);
                }
            }
        }

        public static void ConfigurarOcultarColumnas(this DataGridView dgv, params string[] nombresColumnas)
        {
            foreach (var nombre in nombresColumnas)
            {
                if (dgv.Columns[nombre] is DataGridViewColumn col)
                {
                    col.Visible = false;
                }
            }
        }

        public static void ConfigurarColumnas(this DataGridView dgv, params (string NombrePropiedad, string Encabezado, int? Ancho)[] columnas)
        {
            dgv.ConfigurarColumnas(columnas.Select(c => (c.NombrePropiedad, c.Encabezado, c.Ancho, (string?)null)).ToArray());
        }

        public static void ConfigurarColumnas(this DataGridView dgv, params (string NombrePropiedad, string Encabezado, int? Ancho, string? Formato)[] columnas)
        {
            foreach (var (nombre, encabezado, ancho, formato) in columnas)
            {
                if (dgv.Columns[nombre] is DataGridViewColumn col)
                {
                    col.HeaderText = encabezado;
                    col.Visible = true;
                    if (ancho.HasValue)
                    {
                        col.FillWeight = ancho.Value;
                        col.MinimumWidth = Math.Min(ancho.Value, 75);
                    }
                    if (!string.IsNullOrEmpty(formato))
                    {
                        col.DefaultCellStyle.Format = formato;
                    }
                }
            }
        }

        public static int? ObtenerIdSeleccionado(this DataGridView dgv, int indiceFila, string nombreColumna = "IdTicket")
        {
            if (indiceFila >= 0 && dgv.Rows[indiceFila].Cells[nombreColumna]?.Value is object val)
            {
                if (int.TryParse(val.ToString(), out int id))
                {
                    return id;
                }
            }
            return null;
        }

        public static async Task ManejarDetalleTicketAsync(this DataGridView dgv, int indiceFila, IFabricaFormularios fabricaFormularios, Func<Task> retornoRecargar, string nombreColumna = "IdTicket", bool esCliente = false, bool abrirEnRetroalimentacion = false)
        {
            var id = dgv.ObtenerIdSeleccionado(indiceFila, nombreColumna);
            if (id.HasValue)
            {
                using var frm = esCliente
                    ? (Form)fabricaFormularios.CrearDetalleCliente(id.Value)
                    : (Form)fabricaFormularios.CrearTicketDetalle(id.Value);

                if (abrirEnRetroalimentacion && frm is TicketDetalleForm detalleTicket)
                {
                    detalleTicket.SeleccionarPestanaRetroalimentacion();
                }

                frm.ShowDialog();
                await retornoRecargar();
            }
        }
    }
}

