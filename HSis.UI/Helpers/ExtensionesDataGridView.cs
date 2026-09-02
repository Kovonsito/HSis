using HSis.UI.Factories;

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
                    col.MinimumWidth = col.GetPreferredWidth(DataGridViewAutoSizeColumnMode.ColumnHeader, true);
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
            foreach (var (nombre, encabezado, ancho) in columnas)
            {
                if (dgv.Columns[nombre] is DataGridViewColumn col)
                {
                    col.HeaderText = encabezado;
                    if (ancho.HasValue)
                    {
                        col.FillWeight = ancho.Value;
                        col.MinimumWidth = Math.Min(ancho.Value, 75);
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

        public static async Task ManejarDetalleTicketAsync(this DataGridView dgv, int indiceFila, IFabricaFormularios fabricaFormularios, Func<Task> retornoRecargar, string nombreColumna = "IdTicket", bool esCliente = false)
        {
            var id = dgv.ObtenerIdSeleccionado(indiceFila, nombreColumna);
            if (id.HasValue)
            {
                using var frm = esCliente
                    ? (Form)fabricaFormularios.CrearDetalleCliente(id.Value)
                    : (Form)fabricaFormularios.CrearTicketDetalle(id.Value);

                frm.ShowDialog();
                await retornoRecargar();
            }
        }
    }
}

