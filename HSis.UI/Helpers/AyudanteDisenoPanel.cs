#nullable enable
using System.Drawing;
using System.Windows.Forms;

namespace HSis.UI.Helpers
{
    public static class AyudanteDisenoPanel
    {
        public static TableLayoutPanel CrearPanelPrincipal(Size tamanoCliente, bool incluirFiltros)
        {
            var tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Name = "tblPrincipal",
                RowCount = incluirFiltros ? 5 : 4,
                ColumnCount = 1,
                Size = tamanoCliente
            };

            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // Fila 0: Título (lblTitulo)
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 120F)); // Fila 1: Indicadores
            if (incluirFiltros)
            {
                tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // Fila 2: Panel de Filtros
                tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));  // Fila 3: Grid
                tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));  // Fila 4: Paginación
            }
            else
            {
                tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));  // Fila 2: Grid
                tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));  // Fila 3: Paginación
            }

            return tbl;
        }

        public static TableLayoutPanel CrearPanelIndicadores(string nombre, int columnas, params Control[] controles)
        {
            var tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Name = nombre,
                RowCount = 1,
                ColumnCount = columnas,
                Margin = new Padding(7, 0, 7, 0)
            };

            float percent = 100f / columnas;
            for (int i = 0; i < columnas; i++)
            {
                tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, percent));
            }

            for (int i = 0; i < controles.Length; i++)
            {
                var ctrl = controles[i];
                ctrl.Dock = DockStyle.Fill;
                ctrl.Margin = new Padding(5);
                tbl.Controls.Add(ctrl, i, 0);
            }

            return tbl;
        }

        public static void ReubicarControles(Control padre, Control tblPrincipal, params Control?[] controlesARemover)
        {
            foreach (var ctrl in controlesARemover)
            {
                if (ctrl != null)
                {
                    padre.Controls.Remove(ctrl);
                }
            }
            padre.Controls.Add(tblPrincipal);
        }
    }
}
