#nullable enable
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using HSis.Contracts.Services;
using HSis.UI.Controls;

namespace HSis.UI.Helpers
{
    /// <summary>
    /// Fábrica y ayudante para generar y vincular pestañas de catálogo dinámicas en el TabControl.
    /// Elimina el código repetitivo de instanciación manual de DataGridView, Paneles y Botones.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static class AyudanteCatalogoTab
    {
        public static TabPage CrearTabCatalogo(
            string nombreTab,
            Type tipoEntidad,
            ICatalogoService catalogoService,
            Action<Panel, DataGridView>? onConfigurarAccionesExtra = null)
        {
            var tab = new TabPage(nombreTab)
            {
                BackColor = TemaVisual.FondoApp
            };

            var dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                Name = "dgv" + nombreTab,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                MultiSelect = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgv.AplicarTemaModerno();

            var panelTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                BackColor = Color.White,
                Padding = new Padding(12, 10, 12, 10)
            };
            panelTop.Paint += (s, e) =>
            {
                using var pen = new Pen(TemaVisual.BordeSutil, 1f);
                e.Graphics.DrawLine(pen, 0, panelTop.Height - 1, panelTop.Width, panelTop.Height - 1);
            };

            var btnActualizar = new BotonModerno
            {
                Text = "Actualizar",
                Icono = FontAwesome.Sharp.IconChar.Rotate,
                IconoTamano = 14,
                Location = new Point(12, 10),
                Width = 130,
                Height = 36,
                Estilo = EstiloBotonModerno.Secundario
            };
            btnActualizar.Click += async (s, e) => await CargarDatosAsync(dgv, tipoEntidad, catalogoService);
            panelTop.Controls.Add(btnActualizar);

            onConfigurarAccionesExtra?.Invoke(panelTop, dgv);

            tab.Controls.Add(dgv);
            tab.Controls.Add(panelTop);

            ConfigurarFormateoDeCeldas(dgv, tipoEntidad);

            return tab;
        }

        public static async Task CargarDatosAsync(DataGridView dgv, Type tipoEntidad, ICatalogoService catalogoService)
        {
            try
            {
                var resultList = await catalogoService.ObtenerTodosPorTipoAsync(tipoEntidad);

                if (resultList != null)
                {
                    var bindingListType = typeof(ListaVinculableOrdenable<>).MakeGenericType(tipoEntidad);
                    var sortableList = Activator.CreateInstance(bindingListType, resultList);
                    dgv.DataSource = sortableList;
                }
                else
                {
                    dgv.DataSource = null;
                }

                string idPk = "Id" + (tipoEntidad.Name == "RolUsuario" ? "Rol" : tipoEntidad.Name);

                foreach (DataGridViewColumn col in dgv.Columns)
                {
                    if (col.Name.EndsWith("Navigation") ||
                        (col.ValueType?.IsGenericType == true && col.ValueType.GetGenericTypeDefinition() != typeof(Nullable<>)))
                    {
                        col.Visible = false;
                    }
                    else
                    {
                        if (col.Name.StartsWith("Id") && col.Name != idPk)
                        {
                            col.HeaderText = col.Name[2..];
                        }
                    }
                }

                dgv.AutoajustarAnchosMinimos();
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error al cargar datos del catálogo '{Tipo}'.", tipoEntidad.Name);
            }
        }

        private static void ConfigurarFormateoDeCeldas(DataGridView dgv, Type tipo)
        {
            dgv.CellFormatting += (s, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

                var columnName = dgv.Columns[e.ColumnIndex].Name;
                string idPk = "Id" + (tipo.Name == "RolUsuario" ? "Rol" : tipo.Name);

                if (columnName.StartsWith("Id") && columnName != idPk)
                {
                    var navPropName = columnName + "Navigation";
                    var entidad = dgv.Rows[e.RowIndex].DataBoundItem;
                    if (entidad != null)
                    {
                        var navProp = entidad.GetType().GetProperty(navPropName);
                        var navObj = navProp?.GetValue(entidad);
                        if (navObj != null)
                        {
                            var nombreProp = navObj.GetType().GetProperty("Nombre") ?? navObj.GetType().GetProperty("Descripcion");
                            if (nombreProp != null)
                            {
                                e.Value = nombreProp.GetValue(navObj);
                                e.FormattingApplied = true;
                            }
                        }
                    }
                }
            };
        }
    }
}
