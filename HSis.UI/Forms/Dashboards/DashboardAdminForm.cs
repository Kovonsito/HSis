#nullable enable
using System.Data;
using System.Runtime.Versioning;
using HSis.Logic.Constants;
using HSis.Logic.DTOs;
using HSis.Logic.Services;
using HSis.UI.Controls;
using HSis.UI.Factories;
using HSis.UI.Forms.Otros;
using HSis.UI.Forms.Tickets;
using HSis.UI.Helpers;

using HSis.Contracts.Coordinators;
using HSis.UI.Services.Coordinators;

namespace HSis.UI.Forms.Dashboards
{
    [SupportedOSPlatform("windows")]
    public partial class DashboardAdminForm : Form
    {
        private readonly IAdminDashboardCoordinator _coordinator;
        private readonly IUiSessionCoordinator _sessionCoordinator;
        private readonly IFabricaFormularios _fabricaFormularios;

        private bool _estaCargando = true;
        private PaginacionControl PaginacionControl = null!;
        private ControladorPaginacionGrid _controladorPaginacion = null!;
        private IndicadorControl? _ucCalificacion;

        public DashboardAdminForm(
            IAdminDashboardCoordinator coordinator,
            IUiSessionCoordinator sessionCoordinator)
        {
            InitializeComponent();
            _coordinator = coordinator;
            _sessionCoordinator = sessionCoordinator;
            _fabricaFormularios = sessionCoordinator.FabricaFormularios;
        }

        private async void DashboardAdmin_Load(object sender, EventArgs e)
        {
            dgvTickets.AplicarTemaModerno();
            InicializarLayoutDashboard();
            _controladorPaginacion = new ControladorPaginacionGrid(PaginacionControl);
            _controladorPaginacion.Vincular(async () => { if (!_estaCargando) await FiltrarTicketsAsync(); });

            ConfigurarSidebar();
            ConfigurarFechasYFiltros();

            this.IntegrarNotificacionesModerno(
                topBarAdmin,
                _sessionCoordinator,
                () => Task.WhenAll(CargarKPIsAsync(SesionSistema.IdUsuario), CargarGridCompletoAsync())
            );

            // Cargar los combos de filtros antes del grid
            await CargarCombosFiltrosAsync();

            // Desactivar bandera de carga para permitir consultas
            _estaCargando = false;

            // Cargamos KPIs y Grid de tickets en paralelo
            await Task.WhenAll(
                CargarKPIsAsync(SesionSistema.IdUsuario),
                CargarGridCompletoAsync()
            );

            await ConfigurarTabsCatalogosAsync();
        }

        private void ConfigurarSidebar()
        {
            var items = new[]
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
            };

            sidebarAdmin.ConfigurarSesion(_sessionCoordinator.SessionCache);
            sidebarAdmin.ConfigurarItems(items, "tickets");

            void SeleccionarVista(string clave)
            {
                if (clave == "reportes")
                {
                    btnAbrirReportes_Click(this, EventArgs.Empty);
                    sidebarAdmin.SeleccionarItem("tickets");
                    topBarAdmin.ActualizarItemActivo("tickets");
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

                sidebarAdmin.SeleccionarItem(clave);
                topBarAdmin.ActualizarItemActivo(clave);
            }

            sidebarAdmin.ItemSeleccionado += (s, clave) => SeleccionarVista(clave);

            topBarAdmin.ConfigurarSesion(_sessionCoordinator.SessionCache);
            topBarAdmin.ConfigurarMenuHamburguesa(
                items,
                "tickets",
                SeleccionarVista,
                () => sidebarAdmin.Colapsado = !sidebarAdmin.Colapsado,
                () => !sidebarAdmin.Colapsado
            );
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

            await this.EjecutarOperacionAsync(async () =>
            {
                var filtros = ConfiguracionFiltrosTickets.MapearFiltrosAdmin(filtroGenerico.ObtenerValoresFiltros());
                var resultado = await _coordinator.FiltrarTicketsPaginadosAsync(filtros, _controladorPaginacion.PaginaActual, _controladorPaginacion.TamanoPagina);
                MostrarTickets(resultado.Items, resultado.TotalCount);
            }, "Error al filtrar tickets");
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
            _controladorPaginacion.ReiniciarAPrimeraPagina();
            await FiltrarTicketsAsync();
        }

        private async void btnRecargar_Click(object sender, EventArgs e)
        {
            await CargarGridCompletoAsync();
            await CargarKPIsAsync(SesionSistema.IdUsuario);
        }

        private async void dgvTickets_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            await dgvTickets.ManejarDetalleTicketAsync(e.RowIndex, _fabricaFormularios, () => Task.WhenAll(CargarKPIsAsync(SesionSistema.IdUsuario), CargarGridCompletoAsync()), "Folio");
        }

        private async Task ConfigurarTabsCatalogosAsync()
        {
            var catalogos = new (string Nombre, Type Tipo)[] {
                ("Usuarios", typeof(UsuarioDto)),
                ("Departamentos", typeof(DepartamentoDto)),
                ("Empresas", typeof(EmpresaDto)),
                ("Materiales", typeof(MaterialDto)),
                ("Puestos", typeof(PuestoDto)),
                ("RolesUsuario", typeof(RolUsuarioDto)),
                ("Sucursales", typeof(SucursalDto))
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
            TabPage tab = new(nombre) { BackColor = Color.FromArgb(248, 250, 252) };

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
            panelTop.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(226, 232, 240), 1f);
                e.Graphics.DrawLine(pen, 0, panelTop.Height - 1, panelTop.Width, panelTop.Height - 1);
            };

            BotonModerno btnActualizar = new()
            {
                Text = "Actualizar",
                Icono = FontAwesome.Sharp.IconChar.Rotate,
                IconoTamano = 14,
                Location = new Point(12, 10),
                Width = 130,
                Height = 36,
                Estilo = EstiloBotonModerno.Secundario
            };
            btnActualizar.Click += async (s, e) => await CargarDatosCatalogo(tipo, dgv);
            panelTop.Controls.Add(btnActualizar);

            if (tipo == typeof(MaterialDto))
            {
                AgregarControlesInventario(panelTop, dgv);
            }

            tab.Controls.Add(dgv);
            tab.Controls.Add(panelTop);
            tabMain.TabPages.Add(tab);

            ConfigurarFormateoDeCeldas(dgv, tipo);
            await CargarDatosCatalogo(tipo, dgv);
        }

        private void AgregarControlesInventario(Panel panelTop, DataGridView dgv)
        {
            BotonModerno btnKardex = new()
            {
                Text = "Ver Kardex",
                Icono = FontAwesome.Sharp.IconChar.ClipboardList,
                IconoTamano = 14,
                Location = new Point(155, 10),
                Width = 140,
                Height = 36,
                Estilo = EstiloBotonModerno.Primario
            };

            btnKardex.Click += (s, ev) =>
            {
                var frmK = _fabricaFormularios.Crear<KardexForm>();
                frmK.ShowDialog();
            };

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

        private async Task CargarDatosCatalogo(Type tipoEntidad, DataGridView dgv)
        {
            try
            {
                var resultList = await _coordinator.CargarDatosCatalogoAsync(tipoEntidad);

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
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Error al cargar datos del catálogo '{Tipo}'.", tipoEntidad.Name);
            }
        }

        private async void btnNuevoTicket_Click(object sender, EventArgs e)
        {
            try
            {
                using var frm = _fabricaFormularios.Crear<NuevoTicketForm>();
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    await Task.WhenAll(CargarKPIsAsync(SesionSistema.IdUsuario), CargarGridCompletoAsync());
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
                var promedio = await _coordinator.ObtenerCalificacionPromedioAsync(SesionSistema.IdUsuario);
                MessageBox.Show($"Tu calificación promedio como Administrador resolviendo tickets es: {promedio:F1} de 5.0 ⭐", "Mi Calificación", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener la calificación: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async Task CargarKPIsAsync(int? idUsuario = null)
        {
            try
            {
                int targetUser = idUsuario ?? _sessionCoordinator.ContextoSesion.IdUsuario;
                var resumen = await _coordinator.ObtenerResumenKPIsAsync(targetUser);
                MostrarKPIs(
                    resumen.TicketsNuevos,
                    resumen.TicketsUrgentes,
                    resumen.TicketsEnProceso,
                    resumen.TicketsCerrados,
                    resumen.TicketsReabiertos,
                    resumen.PromedioCalificacion
                );
            }
            catch (Exception ex)
            {
                MostrarError($"Error al cargar KPIs de administración: {ex.Message}");
            }
        }

        public async Task CargarCombosFiltrosAsync()
        {
            try
            {
                var tecnicosYAdmins = await _coordinator.ObtenerTecnicosYAdminsAsync();
                filtroGenerico.ConfigurarOpcionesCombo("Tecnico", tecnicosYAdmins);
                filtroGenerico.ConfigurarOpcionesCombo("Estatus", new List<string> { "Todos", "Nuevos", "Urgentes", ConstantesEstatus.ABIERTO, ConstantesEstatus.EN_PROCESO, ConstantesEstatus.CERRADO, ConstantesEstatus.REABIERTO });
                filtroGenerico.ConfigurarOpcionesCombo("Prioridad", new List<string> { "Todos", ConstantesPrioridad.ALTA, ConstantesPrioridad.MEDIA, ConstantesPrioridad.BAJA });
            }
            catch (Exception ex)
            {
                MostrarError($"Error al cargar filtros de técnicos y administradores: {ex.Message}");
            }
        }

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
            dgvTickets.DataSource = new ListaVinculableOrdenable<TicketDto>(tickets);
            dgvTickets.ConfigurarOcultarColumnas(
                "IdTicket", "IdUsuario", "DepartamentoUsuario", "Calificacion",
                "ComentarioEvaluacion", "FechaEvaluacion", "Evaluacion", "Feedback", "FolioFormato");

            var colFolio = dgvTickets.Columns["Folio"];
            if (colFolio != null) { colFolio.HeaderText = "Folio"; colFolio.FillWeight = 50; }
            var colUsuario = dgvTickets.Columns["NombreUsuario"];
            if (colUsuario != null) { colUsuario.HeaderText = "Usuario"; colUsuario.FillWeight = 110; }
            var colStatus = dgvTickets.Columns["Estatus"];
            if (colStatus != null) { colStatus.HeaderText = "Estatus"; colStatus.FillWeight = 70; }
            var colPrioridad = dgvTickets.Columns["Prioridad"];
            if (colPrioridad != null) { colPrioridad.HeaderText = "Prioridad"; colPrioridad.FillWeight = 70; }
            var colFechaAlta = dgvTickets.Columns["FechaAlta"];
            if (colFechaAlta != null) { colFechaAlta.HeaderText = "Fecha Alta"; colFechaAlta.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm"; colFechaAlta.FillWeight = 85; }
            var colFechaAtencion = dgvTickets.Columns["FechaAtencion"];
            if (colFechaAtencion != null) { colFechaAtencion.HeaderText = "Fecha Atención"; colFechaAtencion.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm"; colFechaAtencion.FillWeight = 85; }
            var colFechaCierre = dgvTickets.Columns["FechaCierre"];
            if (colFechaCierre != null) { colFechaCierre.HeaderText = "Fecha Cierre"; colFechaCierre.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm"; colFechaCierre.FillWeight = 85; }
            var colTecnico = dgvTickets.Columns["TecnicoAsignado"];
            if (colTecnico != null) { colTecnico.HeaderText = "Técnico Asignado"; colTecnico.FillWeight = 100; }
            var colDesc = dgvTickets.Columns["Descripcion"];
            if (colDesc != null) { colDesc.HeaderText = "Descripción"; colDesc.FillWeight = 150; }
            var colSol = dgvTickets.Columns["Solucion"];
            if (colSol != null) { colSol.HeaderText = "Solución"; colSol.FillWeight = 150; }

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

            filtroGenerico.ConfigurarOpcionesCombo("Tecnico", listaTecnicos);
            filtroGenerico.ConfigurarOpcionesCombo("Estatus", new List<string> { "Todos", "Nuevos", "Urgentes", ConstantesEstatus.ABIERTO, ConstantesEstatus.EN_PROCESO, ConstantesEstatus.CERRADO, ConstantesEstatus.REABIERTO });
            filtroGenerico.ConfigurarOpcionesCombo("Prioridad", new List<string> { "Todos", ConstantesPrioridad.ALTA, ConstantesPrioridad.MEDIA, ConstantesPrioridad.BAJA });
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

        public void MostrarCargando(bool cargando)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => MostrarCargando(cargando)));
                return;
            }
            Cursor = cargando ? Cursors.WaitCursor : Cursors.Default;
        }
    }
}
