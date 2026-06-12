#nullable enable
using System.Windows.Forms;

namespace HSis.UI
{
    public static class DataGridViewExtensions
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
    }
}
