#nullable enable
using System.Data;
using System.Runtime.Versioning;
using HSis.Contracts.Constants;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.UI.Services;
using HSis.UI.Controls;
using HSis.UI.Factories;
using HSis.UI.Forms.Otros;
using HSis.UI.Forms.Tickets;
using HSis.UI.Helpers;

namespace HSis.UI.Forms.Dashboards
{
    [SupportedOSPlatform("windows")]
    public partial class DashboardAdminForm : Form
    {
        private readonly ITicketService _ticketService;
        private readonly IUsuarioService _usuarioService;
        private readonly ICatalogoService _catalogoService;
        private readonly IAdministradorSesionUsuario _contextoSesion;
        private readonly IAlmacenamientoCredencialesLocal _sessionCache;
        private readonly IFabricaFormularios _fabricaFormularios;
        private readonly IClienteSignalRNotificaciones _notificationClient;

        private bool _estaCargando = true;
        private IndicadorControl? _ucCalificacion;

        public DashboardAdminForm(
            ITicketService ticketService,
            IUsuarioService usuarioService,
            ICatalogoService catalogoService,
            IAdministradorSesionUsuario contextoSesion,
            IAlmacenamientoCredencialesLocal sessionCache,
            IFabricaFormularios fabricaFormularios,
            IClienteSignalRNotificaciones notificationClient)
        {
            InitializeComponent();
            _ticketService = ticketService;
            _usuarioService = usuarioService;
            _catalogoService = catalogoService;
            _contextoSesion = contextoSesion;
            _sessionCache = sessionCache;
            _fabricaFormularios = fabricaFormularios;
            _notificationClient = notificationClient;
        }

        private async void DashboardAdmin_Load(object sender, EventArgs e)
        {
            // Crear indicador de calificación dinámico
            _ucCalificacion = new IndicadorControl();
            _ucCalificacion.IndicadorClic += UcCalificacion_Click;

            // ── Bloque 3: configuración centralizada de VistaTicketsDashboardControl ──
            vistaTickets.ConfigurarComportamiento(new ConfiguracionVistaDashboard
            {
                Indicadores     = [ucNuevos, ucUrgentes, ucEnProceso, ucCerrados, ucReabiertos, _ucCalificacion, btnNuevoTicket, btnAbrirReportes],
                Filtros         = ConfiguracionFiltrosTickets.ObtenerCamposAdmin(),
                AlMostrarPagina = async () => { if (!_estaCargando) await FiltrarTicketsAsync(); },
                AlRecargar      = async () => { await CargarGridCompletoAsync(); await CargarKPIsAsync(_contextoSesion.IdUsuario); },
                AlLimpiar       = async () =>
                {
                    _estaCargando = true;
                    vistaTickets.Filtro.LimpiarFiltros(ConfiguracionFiltrosTickets.ObtenerValoresDefecto());
                    vistaTickets.ReiniciarAPrimeraPagina();
                    _estaCargando = false;
                    await CargarGridCompletoAsync();
                },
                AlDobleClicFila = async rowIndex =>
                    await vistaTickets.Grid.ManejarDetalleTicketAsync(
                        rowIndex, _fabricaFormularios,
                        () => Task.WhenAll(CargarKPIsAsync(_contextoSesion.IdUsuario), CargarGridCompletoAsync()),
                        "Folio")
            });

            // FiltroCambiado con guard de _estaCargando (Admin usa paginación servidor)
            vistaTickets.FiltroCambiado += async (_, _) =>
            {
                if (!_estaCargando)
                {
                    vistaTickets.ReiniciarAPrimeraPagina();
                    await FiltrarTicketsAsync();
                }
            };

            // ── Bloque 1: sidebar + topBar ───────────────────────────────────────────
            var items = new[]
            {
                new ItemSidebar { Clave = "tickets",       Titulo = "Tickets",       Icono = FontAwesome.Sharp.IconChar.TicketAlt },
                new ItemSidebar { Clave = "inventario",    Titulo = "Inventario",    Icono = FontAwesome.Sharp.IconChar.BoxesStacked },
                new ItemSidebar { Clave = "usuarios",      Titulo = "Usuarios",      Icono = FontAwesome.Sharp.IconChar.Users },
                new ItemSidebar { Clave = "departamentos", Titulo = "Departamentos", Icono = FontAwesome.Sharp.IconChar.Building },
                new ItemSidebar { Clave = "sucursales",    Titulo = "Sucursales",    Icono = FontAwesome.Sharp.IconChar.MapMarkerAlt },
                new ItemSidebar { Clave = "empresas",      Titulo = "Empresas",      Icono = FontAwesome.Sharp.IconChar.Landmark },
                new ItemSidebar { Clave = "puestos",       Titulo = "Puestos",       Icono = FontAwesome.Sharp.IconChar.Briefcase },
                new ItemSidebar { Clave = "roles",         Titulo = "Roles",         Icono = FontAwesome.Sharp.IconChar.Key },
                new ItemSidebar { Clave = "reportes",      Titulo = "Reportes",      Icono = FontAwesome.Sharp.IconChar.ChartBar }
            };

            ConfiguradorSidebarDashboard.Configurar(
                sidebar:        sidebarAdmin,
                topBar:         topBarAdmin,
                sessionCache:   _sessionCache,
                contextoSesion: _contextoSesion,
                items:          items,
                claveDefault:   "tickets",
                alSeleccionar:  SeleccionarVista
            );

            this.IntegrarNotificacionesModerno(
                topBarAdmin, _fabricaFormularios, _contextoSesion, _notificationClient, null,
                () => Task.WhenAll(CargarKPIsAsync(_contextoSesion.IdUsuario), CargarGridCompletoAsync())
            );

            await CargarCombosFiltrosAsync();
            _estaCargando = false;

            await Task.WhenAll(
                CargarKPIsAsync(_contextoSesion.IdUsuario),
                CargarGridCompletoAsync()
            );

            await ConfigurarTabsCatalogosAsync();
        }

        // ── Selección de vista (lógica propia del rol Admin) ──────────────────────────
        private void SeleccionarVista(string clave)
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
                "tickets"       => "Tickets",
                "inventario"    => "Materiales",
                "usuarios"      => "Usuarios",
                "departamentos" => "Departamentos",
                "sucursales"    => "Sucursales",
                "empresas"      => "Empresas",
                "puestos"       => "Puestos",
                "roles"         => "RolesUsuario",
                _ => "Tickets"
            };

            topBarAdmin.Titulo    = nombreTab == "Tickets" ? "Panel de Control"              : $"Catálogo: {nombreTab}";
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


        public void UcNuevosUcIndicadorEvent(object sender, EventArgs e)
        {
            vistaTickets.EstablecerValorFiltro("Estatus", "Nuevos");
        }

        private void UcUrgentes_ucIndicadorEvent(object sender, EventArgs e)
        {
            vistaTickets.EstablecerValorFiltro("Estatus", "Urgentes");
        }

        private void UcEnProceso_ucIndicadorEvent(object sender, EventArgs e)
        {
            vistaTickets.EstablecerValorFiltro("Estatus", ConstantesEstatus.EN_PROCESO);
        }

        private void UcCerrados_ucIndicadorEvent(object sender, EventArgs e)
        {
            vistaTickets.EstablecerValorFiltro("Estatus", ConstantesEstatus.CERRADO);
        }

        private void UcReabiertos_ucIndicadorEvent(object sender, EventArgs e)
        {
            vistaTickets.EstablecerValorFiltro("Estatus", ConstantesEstatus.REABIERTO);
        }

        private async Task FiltrarTicketsAsync()
        {
            if (_estaCargando) return;

            await this.EjecutarOperacionAsync(async () =>
            {
                var filtros = ConfiguracionFiltrosTickets.MapearFiltrosAdmin(vistaTickets.ObtenerValoresFiltros());
                var ctrl = vistaTickets.ControladorPaginacion;
                var resultado = await _ticketService.ObtenerTicketsFiltradosPaginadosAsync(filtros, ctrl.PaginaActual, ctrl.TamanoPagina);
                MostrarTickets(resultado.Items, resultado.TotalCount);
            }, "Error al filtrar tickets");
        }

        private void btnAbrirReportes_Click(object sender, EventArgs e)
        {
            var modal = _fabricaFormularios.Crear<GeneradorReportesForm>();
            modal.ShowDialog();
        }

        private async Task CargarGridCompletoAsync()
        {
            vistaTickets.ReiniciarAPrimeraPagina();
            await FiltrarTicketsAsync();
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
                var resultList = await _catalogoService.ObtenerTodosPorTipoAsync(tipoEntidad);

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
                    await Task.WhenAll(CargarKPIsAsync(_contextoSesion.IdUsuario), CargarGridCompletoAsync());
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
                var promedio = await _ticketService.ObtenerPromedioCalificacionTecnicoAsync(_contextoSesion.IdUsuario);
                MessageBox.Show($"Tu calificación promedio como Administrador resolviendo tickets es: {promedio:F1} de 5.0 ⭐", "Mi Calificación", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener la calificación: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async Task CargarKPIsAsync(int? idUsuario = null)
        {
            await this.EjecutarOperacionAsync(async () =>
            {
                int targetUser = idUsuario ?? _contextoSesion.IdUsuario;
                var resumen = await _ticketService.ObtenerResumenDashboardAsync(targetUser);
                MostrarKPIs(
                    resumen.TicketsNuevos,
                    resumen.TicketsUrgentes,
                    resumen.TicketsEnProceso,
                    resumen.TicketsCerrados,
                    resumen.TicketsReabiertos,
                    resumen.PromedioCalificacion
                );
            }, "Error al cargar KPIs de administración");
        }

        public async Task CargarCombosFiltrosAsync()
        {
            await this.EjecutarOperacionAsync(async () =>
            {
                var tecnicosYAdmins = await ObtenerTecnicosYAdminsAsync();
                vistaTickets.Filtro.ConfigurarOpcionesCombo("Tecnico", tecnicosYAdmins);
                vistaTickets.Filtro.ConfigurarOpcionesCombo("Estatus", new List<string> { "Todos", "Nuevos", "Urgentes", ConstantesEstatus.ABIERTO, ConstantesEstatus.EN_PROCESO, ConstantesEstatus.CERRADO, ConstantesEstatus.REABIERTO });
                vistaTickets.Filtro.ConfigurarOpcionesCombo("Prioridad", new List<string> { "Todos", ConstantesPrioridad.ALTA, ConstantesPrioridad.MEDIA, ConstantesPrioridad.BAJA });
            }, "Error al cargar filtros de técnicos y administradores");
        }

        private async Task<List<ElementoOpcionCombo>> ObtenerTecnicosYAdminsAsync()
        {
            var resultado = new List<ElementoOpcionCombo>
            {
                new(0, "Todos"),
                new(-1, "Sin Asignar")
            };

            var tareaAdmins = _usuarioService.ObtenerUsuariosPorRolAsync(RolUsuarioEnum.Administrador);
            var tareaTecnicos = _usuarioService.ObtenerUsuariosPorRolAsync(RolUsuarioEnum.Tecnico);

            await Task.WhenAll(tareaAdmins, tareaTecnicos);

            var admins = await tareaAdmins ?? [];
            var tecnicos = await tareaTecnicos ?? [];

            foreach (var a in admins)
            {
                resultado.Add(new(a.IdUsuario, $"Admin - {a.Nombre}"));
            }
            foreach (var t in tecnicos)
            {
                resultado.Add(new(t.IdUsuario, $"Técnico - {t.Nombre}"));
            }

            return resultado;
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
                _ucCalificacion.ColorFondo = TemaVisual.TicketReabierto;
            }
        }

        public void MostrarTickets(List<TicketDto> tickets, int totalCount)
        {
            var grid = vistaTickets.Grid;
            grid.DataSource = new ListaVinculableOrdenable<TicketDto>(tickets);

            // ── Bloque 2: perfil de columnas centralizado ─────────────────────────────
            ConfiguracionColumnasDashboard.AplicarPerfilAdmin(grid);

            vistaTickets.ActualizarPaginacion(totalCount);
        }
    }
}
