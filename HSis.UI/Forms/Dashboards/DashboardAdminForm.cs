#nullable enable
using System.Data;
using System.Runtime.Versioning;
using HSis.Data.Models;
using HSis.Logic.Constants;
using HSis.Logic.DTOs;
using HSis.Logic.Services;
using HSis.UI.Controls;
using HSis.UI.Factories;
using HSis.UI.Forms.Otros;
using HSis.UI.Forms.Tickets;
using HSis.UI.Helpers;
using HSis.UI.Presenters;

namespace HSis.UI.Forms.Dashboards
{
    [SupportedOSPlatform("windows")]
    public partial class DashboardAdminForm : Form, IDashboardAdminView
    {
        private readonly DashboardAdminPresenter _presenter;
        private readonly IFabricaFormularios _fabricaFormularios;
        private readonly ISessionCacheService _sessionCache;

        private bool _estaCargando = true;
        private PaginacionControl PaginacionControl = null!;
        private ControladorPaginacionGrid _controladorPaginacion = null!;
        private IndicadorControl? _ucCalificacion;

        public DashboardAdminForm(
            DashboardAdminPresenter presenter,
            IFabricaFormularios fabricaFormularios,
            ISessionCacheService sessionCache)
        {
            InitializeComponent();
            _presenter = presenter;
            _presenter.SetView(this);
            _fabricaFormularios = fabricaFormularios;
            _sessionCache = sessionCache;
        }

        private async void DashboardAdmin_Load(object sender, EventArgs e)
        {
            dgvTickets.AplicarTemaModerno();
            InicializarLayoutDashboard();
            _controladorPaginacion = new ControladorPaginacionGrid(PaginacionControl);
            _controladorPaginacion.Vincular(async () => { if (!_estaCargando) await FiltrarTicketsAsync(); });

            ConfigurarSidebar();
            ConfigurarFechasYFiltros();

            // Cargar los combos de filtros antes del grid
            await _presenter.CargarCombosFiltrosAsync();

            // Cargamos KPIs y Grid de tickets en paralelo
            await Task.WhenAll(
                _presenter.CargarKPIsAsync(SesionSistema.IdUsuario),
                CargarGridCompletoAsync()
            );

            await ConfigurarTabsCatalogosAsync();
        }

        private void ConfigurarSidebar()
        {
            sidebarAdmin.ConfigurarSesion(_sessionCache);
            sidebarAdmin.ConfigurarItems(new[]
            {
                new ItemSidebar { Clave = "tickets", Titulo = "Tickets", Icono = FontAwesome.Sharp.IconChar.TicketAlt },
                new ItemSidebar { Clave = "inventario", Titulo = "Inventario", Icono = FontAwesome.Sharp.IconChar.BoxesStacked },
                new ItemSidebar { Clave = "usuarios", Titulo = "Usuarios", Icono = FontAwesome.Sharp.IconChar.Users },
                new ItemSidebar { Clave = "departamentos", Titulo = "Departamentos", Icono = FontAwesome.Sharp.IconChar.Building },
                new ItemSidebar { Clave = "sucursales", Titulo = "Sucursales", Icono = FontAwesome.Sharp.IconChar.MapMarkerAlt },
                new ItemSidebar { Clave = "empresas", Titulo = "Empresas", Icono = FontAwesome.Sharp.IconChar.Landmark },
                new ItemSidebar { Clave = "puestos", Titulo = "Puestos", Icono = FontAwesome.Sharp.IconChar.Briefcase },
                new ItemSidebar { Clave = "roles", Titulo = "Roles", Icono = FontAwesome.Sharp.IconChar.Key },
                new ItemSidebar { Clave = "reportes", Titulo = "Reportes", Icono = FontAwesome.Sharp.IconChar.ChartBar }
            }, "tickets");

            sidebarAdmin.ItemSeleccionado += (s, clave) =>
            {
                if (clave == "reportes")
                {
                    btnAbrirReportes_Click(this, EventArgs.Empty);
                    sidebarAdmin.SeleccionarItem("tickets");
                    return;
                }

                string nombreTab = clave switch
                {
                    "tickets" => "Tickets",
                    "inventario" => "Materiales",
                    "usuarios" => "Usuarios",
                    "departamentos" => "Departamentos",
                    "sucursales" => "Sucursales",
                    "empresas" => "Empresas",
                    "puestos" => "Puestos",
                    "roles" => "RolesUsuario",
                    _ => "Tickets"
                };

                topBarAdmin.Titulo = nombreTab == "Tickets" ? "Panel de Control" : $"Catálogo: {nombreTab}";
                topBarAdmin.Subtitulo = nombreTab == "Tickets" ? "Mesa de Servicio y Gestión Global" : $"Administración de registros de {nombreTab}";

                foreach (TabPage tab in tabMain.TabPages)
                {
                    if (tab.Text.Equals(nombreTab, StringComparison.OrdinalIgnoreCase) ||
                        tab.Name.Equals("tab" + nombreTab, StringComparison.OrdinalIgnoreCase))
                    {
                        tabMain.SelectedTab = tab;
                        break;
                    }
                }
            };
        }

        private void ConfigurarFechasYFiltros()
        {
            filtroGenerico.InicializarFiltros(ConfiguracionFiltrosTickets.ObtenerCamposAdmin());
            filtroGenerico.FiltroCambiado += async (s, e) =>
            {
                if (!_estaCargando)
                {
                    _controladorPaginacion.ReiniciarAPrimeraPagina();
                    await FiltrarTicketsAsync();
                }
            };
        }

        public void UcNuevosUcIndicadorEvent(object sender, EventArgs e)
        {
            filtroGenerico.EstablecerValorFiltro("Estatus", "Nuevos");
        }

        private void UcUrgentes_ucIndicadorEvent(object sender, EventArgs e)
        {
            filtroGenerico.EstablecerValorFiltro("Estatus", "Urgentes");
        }

        private void UcEnProceso_ucIndicadorEvent(object sender, EventArgs e)
        {
            filtroGenerico.EstablecerValorFiltro("Estatus", ConstantesEstatus.EN_PROCESO);
        }

        private void UcCerrados_ucIndicadorEvent(object sender, EventArgs e)
        {
            filtroGenerico.EstablecerValorFiltro("Estatus", ConstantesEstatus.CERRADO);
        }

        private void UcReabiertos_ucIndicadorEvent(object sender, EventArgs e)
        {
            filtroGenerico.EstablecerValorFiltro("Estatus", ConstantesEstatus.REABIERTO);
        }

        private async Task FiltrarTicketsAsync()
        {
            if (_estaCargando) return;

            var filtros = ConfiguracionFiltrosTickets.MapearFiltrosAdmin(filtroGenerico.ObtenerValoresFiltros());
            await _presenter.FiltrarTicketsAsync(filtros, _controladorPaginacion.PaginaActual, _controladorPaginacion.TamanoPagina);
        }

        private async void btnLimpiarFiltros_Click(object sender, EventArgs e)
        {
            _estaCargando = true;
            filtroGenerico.LimpiarFiltros(ConfiguracionFiltrosTickets.ObtenerValoresDefecto());
            _controladorPaginacion.ReiniciarAPrimeraPagina();
            _estaCargando = false;

            await CargarGridCompletoAsync();
        }

        private void btnAbrirReportes_Click(object sender, EventArgs e)
        {
            var modal = _fabricaFormularios.Crear<GeneradorReportesForm>();
            modal.ShowDialog();
        }

        private async Task CargarGridCompletoAsync()
        {
            PaginacionControl.PaginaActual = 1;
            await FiltrarTicketsAsync();
        }

        private async void btnRecargar_Click(object sender, EventArgs e)
        {
            await CargarGridCompletoAsync();
            await _presenter.CargarKPIsAsync(SesionSistema.IdUsuario);
        }

        private async void dgvTickets_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            await dgvTickets.ManejarDetalleTicketAsync(e.RowIndex, _fabricaFormularios, () => Task.WhenAll(_presenter.CargarKPIsAsync(SesionSistema.IdUsuario), CargarGridCompletoAsync()), "Folio");
        }

        private async Task ConfigurarTabsCatalogosAsync()
        {
            var catalogos = new (string Nombre, Type Tipo)[] {
                ("Usuarios", typeof(Usuario)),
                ("Departamentos", typeof(Departamento)),
                ("Empresas", typeof(Empresa)),
                ("Materiales", typeof(Material)),
                ("Puestos", typeof(Puesto)),
                ("RolesUsuario", typeof(RolUsuario)),
                ("Sucursales", typeof(Sucursal))
            };

            foreach (var (nombre, tipo) in catalogos)
            {
                try
                {
                    await ConfigurarTabParaCatalogo(nombre, tipo);
                }
                catch (Exception ex)
                {
                    Serilog.Log.Error(ex, "Error al configurar o cargar la pestaña de catálogo '{Nombre}'.", nombre);
                }
            }
        }

        private async Task ConfigurarTabParaCatalogo(string nombre, Type tipo)
        {
            TabPage tab = new(nombre);

            DataGridView dgv = new()
            {
                Dock = DockStyle.Fill,
                Name = "dgv" + nombre,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                MultiSelect = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgv.AplicarTemaModerno();

            Panel panelTop = new() { Dock = DockStyle.Top, Height = 56, BackColor = Color.White, Padding = new Padding(12, 10, 12, 10) };
            BotonModerno btnCrear = new()
            {
                Text = "Nuevo Registro",
                Icono = FontAwesome.Sharp.IconChar.PlusCircle,
                IconoTamano = 14,
                Location = new Point(12, 10),
                Width = 150,
                Height = 36,
                Estilo = EstiloBotonModerno.Primario
            };
            BotonModerno btnEliminar = new()
            {
                Text = "Eliminar",
                Icono = FontAwesome.Sharp.IconChar.TrashAlt,
                IconoTamano = 14,
                Location = new Point(170, 10),
                Width = 115,
                Height = 36,
                Estilo = EstiloBotonModerno.Peligro
            };

            panelTop.Controls.Add(btnCrear);
            panelTop.Controls.Add(btnEliminar);

            if (tipo == typeof(Material))
            {
                AgregarControlesInventario(panelTop, dgv);
            }

            tab.Controls.Add(dgv);
            tab.Controls.Add(panelTop);
            tabMain.TabPages.Add(tab);

            ConfigurarFormateoDeCeldas(dgv, tipo);
            await CargarDatosCatalogo(tipo, dgv);

            btnCrear.Click += async (s, e) => await ManejarCreacionRegistro(tipo, dgv);
            btnEliminar.Click += async (s, e) => await ManejarEliminacionRegistro(tipo, dgv);
            dgv.CellDoubleClick += async (s, e) => await ManejarEdicionRegistro(e, tipo, dgv);
        }

        private void AgregarControlesInventario(Panel panelTop, DataGridView dgv)
        {
            BotonModerno btnIngreso = new()
            {
                Text = "Nuevo Movimiento",
                Icono = FontAwesome.Sharp.IconChar.Dolly,
                IconoTamano = 14,
                Location = new Point(295, 10),
                Width = 175,
                Height = 36,
                Estilo = EstiloBotonModerno.Exito
            };
            BotonModerno btnKardex = new()
            {
                Text = "Ver Kardex",
                Icono = FontAwesome.Sharp.IconChar.ClipboardList,
                IconoTamano = 14,
                Location = new Point(478, 10),
                Width = 130,
                Height = 36,
                Estilo = EstiloBotonModerno.Secundario
            };

            btnIngreso.Click += async (s, ev) =>
            {
                var nuevoMovimiento = new MovimientoMaterial
                {
                    IdUsuario = SesionSistema.IdUsuario,
                    FechaMovimiento = DateTime.Now,
                    Cantidad = 1,
                    Motivo = "Ingreso por Compra"
                };
                var frm = _fabricaFormularios.CrearEditorDinamico(nuevoMovimiento, "Nuevo Movimiento de Almacén");
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    await _presenter.CrearMovimientoMaterialAsync(nuevoMovimiento);
                    MessageBox.Show("Movimiento registrado con éxito.", "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _ = CargarDatosCatalogo(typeof(Material), dgv);
                }
            };

            btnKardex.Click += (s, ev) =>
            {
                var frmK = _fabricaFormularios.Crear<KardexForm>();
                frmK.ShowDialog();
            };

            panelTop.Controls.Add(btnIngreso);
            panelTop.Controls.Add(btnKardex);
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

        private async Task ManejarCreacionRegistro(Type tipo, DataGridView dgv)
        {
            object nuevaEntidad = Activator.CreateInstance(tipo)!;
            var frm = _fabricaFormularios.CrearEditorDinamico(nuevaEntidad, $"Crear {tipo.Name}");
            if (frm.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    await _presenter.CrearEntidadCatalogoAsync(tipo, nuevaEntidad);
                    await CargarDatosCatalogo(tipo, dgv);
                }
                catch (Exception ex)
                {
                    var realEx = ex is System.Reflection.TargetInvocationException ? ex.InnerException : ex;
                    if (realEx is FluentValidation.ValidationException vex)
                    {
                        string msg = string.Join("\n", vex.Errors.Select(e => "- " + e.ErrorMessage));
                        MessageBox.Show($"Datos inválidos:\n{msg}", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show($"Error al crear el registro: {realEx?.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async Task ManejarEliminacionRegistro(Type tipo, DataGridView dgv)
        {
            if (dgv.SelectedRows.Count > 0)
            {
                var row = dgv.SelectedRows[0];
                string idName = "Id" + (tipo.Name == "RolUsuario" ? "Rol" : tipo.Name);
                var idObj = row.Cells[idName]?.Value;
                if (idObj != null && MessageBox.Show("¿Seguro que deseas eliminar el registro?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        await _presenter.EliminarEntidadCatalogoAsync(tipo, idObj);
                        await CargarDatosCatalogo(tipo, dgv);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("No se pudo eliminar el registro (probablemente esté en uso). " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async Task ManejarEdicionRegistro(DataGridViewCellEventArgs e, Type tipo, DataGridView dgv)
        {
            if (e.RowIndex >= 0)
            {
                object? entidadExistente = dgv.Rows[e.RowIndex].DataBoundItem;
                if (entidadExistente is null) return;

                string? passwordHashOriginal = null;
                if (tipo == typeof(Usuario))
                {
                    var u = (Usuario)entidadExistente;
                    passwordHashOriginal = u.Contraseña;
                    u.Contraseña = "";
                }

                var frm = _fabricaFormularios.CrearEditorDinamico(entidadExistente, $"Editar {tipo.Name}");
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        await _presenter.ActualizarEntidadCatalogoAsync(tipo, entidadExistente);
                        await CargarDatosCatalogo(tipo, dgv);
                    }
                    catch (Exception ex)
                    {
                        var realEx = ex is System.Reflection.TargetInvocationException ? ex.InnerException : ex;
                        if (realEx is FluentValidation.ValidationException vex)
                        {
                            string msg = string.Join("\n", vex.Errors.Select(e => "- " + e.ErrorMessage));
                            MessageBox.Show($"Datos inválidos:\n{msg}", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            MessageBox.Show($"Error al actualizar el registro: {realEx?.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        // Restaurar contraseña original si falló la actualización
                        if (tipo == typeof(Usuario)) ((Usuario)entidadExistente).Contraseña = passwordHashOriginal;
                    }
                }
                else
                {
                    if (tipo == typeof(Usuario))
                    {
                        var u = (Usuario)entidadExistente;
                        u.Contraseña = passwordHashOriginal;
                    }
                }
            }
        }

        private async Task CargarDatosCatalogo(Type tipoEntidad, DataGridView dgv)
        {
            var resultList = await _presenter.ObtenerDatosCatalogoAsync(tipoEntidad);

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

            // Ocultar columnas no deseadas y renombrar cabeceras
            string idPk = "Id" + (tipoEntidad.Name == "RolUsuario" ? "Rol" : tipoEntidad.Name);

            foreach (DataGridViewColumn col in dgv.Columns)
            {
                if (col.Name.EndsWith("Navigation") || (col.ValueType?.IsGenericType == true && col.ValueType.GetGenericTypeDefinition() != typeof(Nullable<>)))
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

        private async void btnNuevoTicket_Click(object sender, EventArgs e)
        {
            try
            {
                using var frm = _fabricaFormularios.Crear<NuevoTicketForm>();
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    await Task.WhenAll(_presenter.CargarKPIsAsync(SesionSistema.IdUsuario), CargarGridCompletoAsync());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al abrir el formulario de nuevo ticket: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void UcCalificacion_Click(object? sender, EventArgs e)
        {
            try
            {
                var promedio = await _presenter.ObtenerPromedioCalificacionAsync(SesionSistema.IdUsuario);
                MessageBox.Show($"Tu calificación promedio como Administrador resolviendo tickets es: {promedio:F1} de 5.0 ⭐", "Mi Calificación", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener la calificación: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Implementación IDashboardAdminView (MVP)

        public void MostrarKPIs(int nuevos, int urgentes, int enProceso, int cerrados, int reabiertos, double calificacion)
        {
            ucNuevos.Titulo = "Nuevos";
            ucNuevos.Cantidad = nuevos.ToString();
            ucNuevos.ColorFondo = TemaVisual.TicketNuevo;
            ucNuevos.ImagenFondo = Properties.Resources.Nuevo;

            ucUrgentes.Titulo = "Urgentes";
            ucUrgentes.Cantidad = urgentes.ToString();
            ucUrgentes.ColorFondo = TemaVisual.TicketUrgente;
            ucUrgentes.ImagenFondo = Properties.Resources.Urgente;

            ucEnProceso.Titulo = "En proceso";
            ucEnProceso.Cantidad = enProceso.ToString();
            ucEnProceso.ColorFondo = TemaVisual.TicketEnProceso;
            ucEnProceso.ImagenFondo = Properties.Resources.En_proceso;

            ucCerrados.Titulo = "Cerrados";
            ucCerrados.Cantidad = cerrados.ToString();
            ucCerrados.ColorFondo = TemaVisual.TicketCerrado;
            ucCerrados.ImagenFondo = Properties.Resources.Cerrado;

            ucReabiertos.Titulo = "Reabiertos";
            ucReabiertos.Cantidad = reabiertos.ToString();
            ucReabiertos.ColorFondo = TemaVisual.TicketReabierto;

            if (_ucCalificacion != null)
            {
                _ucCalificacion.Titulo = "Mi Calificación";
                _ucCalificacion.Cantidad = calificacion > 0 ? $"⭐ {calificacion:F1}" : "⭐ N/A";
                _ucCalificacion.ColorFondo = Color.FromArgb(139, 92, 246);
            }
        }

        public void MostrarTickets(List<TicketDto> tickets, int totalCount)
        {
            var listaMapeada = tickets.Select(t => new TicketGridDto
            {
                Folio = t.IdTicket,
                NombreUsuario = t.NombreUsuario,
                Estatus = t.Estatus ?? "N/A",
                Prioridad = t.Prioridad ?? "N/A",
                FechaAlta = t.FechaAlta,
                FechaAtencion = t.FechaAtencion,
                FechaCierre = t.FechaCierre ?? DateTime.Now,
                TecnicoAsignado = t.NombreTecnico,
                Descripcion = t.Descripcion ?? "N/A",
                Solucion = t.Solucion ?? "N/A"
            }).ToList();

            dgvTickets.DataSource = new ListaVinculableOrdenable<TicketGridDto>(listaMapeada);
            dgvTickets.AutoajustarAnchosMinimos();
            _controladorPaginacion.Actualizar(totalCount);
        }

        public void CargarCombosFiltros(List<UsuarioDto> admins, List<UsuarioDto> tecnicos)
        {
            var listaTecnicos = new List<object> { new { Id = (int?)0, Nombre = "Todos" } };

            foreach (var a in admins)
            {
                listaTecnicos.Add(new { Id = (int?)a.IdUsuario, Nombre = $"Admin - {a.Nombre}" });
            }
            foreach (var t in tecnicos)
            {
                listaTecnicos.Add(new { Id = (int?)t.IdUsuario, Nombre = $"Técnico - {t.Nombre}" });
            }

            filtroGenerico.ActualizarCombo("Tecnico", listaTecnicos, "Nombre", "Id");
        }

        public void MostrarCargando(bool cargando)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => MostrarCargando(cargando)));
                return;
            }
            _estaCargando = cargando;
            Cursor = cargando ? Cursors.WaitCursor : Cursors.Default;
        }

        public void MostrarError(string mensaje)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => MostrarError(mensaje)));
                return;
            }
            MessageBox.Show(mensaje, "Error en Dashboard de Administración", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void MostrarInformacion(string mensaje, string titulo)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => MostrarInformacion(mensaje, titulo)));
                return;
            }
            MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion
    }
}

