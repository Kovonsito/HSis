#nullable enable
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
                RowCount = incluirFiltros ? 4 : 3,
                ColumnCount = 1,
                Padding = new Padding(16, 12, 16, 10),
                BackColor = Color.Transparent
            };

            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 105F)); // Fila 0: Indicadores (KPI Cards)
            if (incluirFiltros)
            {
                tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 74F));  // Fila 1: Barra de Filtros
                tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Fila 2: Grid
                tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));  // Fila 3: Paginación
            }
            else
            {
                tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Fila 1: Grid
                tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));  // Fila 2: Paginación
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
                Margin = new Padding(0, 0, 0, 8),
                BackColor = Color.Transparent
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
                ctrl.Margin = new Padding(i == 0 ? 0 : 6, 0, i == controles.Length - 1 ? 0 : 6, 0);
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
            tblPrincipal.BringToFront();
        }
    }
}

