#nullable enable
using System.Runtime.Versioning;
using HSis.Contracts.Constants;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.UI.Controls;
using HSis.UI.Factories;
using HSis.UI.Forms.Otros;
using HSis.UI.Forms.Tickets;
using HSis.UI.Helpers;

namespace HSis.UI.Coordinators
{
    /// <summary>
    /// Coordinador unificado para el Dashboard del Administrador.
    /// Maneja tickets globales (paginación de servidor), KPIs, catálogo de pestañas y modales.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class CoordinadorDashboardAdmin(
        Form formulario,
        SidebarControl sidebar,
        TopBarControl topBar,
        VistaTicketsDashboardControl vistaTickets,
        TabControl tabMain,
        Control btnNuevoTicket,
        Control btnAbrirReportes,
        ITicketService ticketService,
        IUsuarioService usuarioService,
        ICatalogoService catalogoService,
        IAdministradorSesionUsuario contextoSesion,
        IAlmacenamientoCredencialesLocal sessionCache,
        IFabricaFormularios formFactory,
        IClienteSignalRNotificaciones notificationClient) : CoordinadorDashboardBase(formulario, sidebar, topBar, vistaTickets, contextoSesion, sessionCache, formFactory, notificationClient)
    {
        private readonly TabControl _tabMain = tabMain;
        private readonly Control _btnNuevoTicket = btnNuevoTicket;
        private readonly Control _btnAbrirReportes = btnAbrirReportes;
        private readonly ITicketService _ticketService = ticketService;
        private readonly IUsuarioService _usuarioService = usuarioService;
        private readonly ICatalogoService _catalogoService = catalogoService;
        private bool _estaCargando = false;

        public override async Task IniciarAsync()
        {
            ConfigurarNavegacion();
            ConfigurarVistaTickets();

            Formulario.IntegrarNotificacionesModerno(
                TopBar, FormFactory, ContextoSesion, NotificationClient, null,
                () => Task.WhenAll(CargarKPIsAsync(), CargarGridCompletoAsync())
            );

            await CargarCombosFiltrosAsync();
            _estaCargando = false;

            await Task.WhenAll(CargarKPIsAsync(), CargarGridCompletoAsync());
            await ConfigurarTabsCatalogosAsync();
        }

        protected override void ConfigurarNavegacion()
        {
            ConfiguradorSidebarDashboard.Configurar(
                sidebar: Sidebar,
                topBar: TopBar,
                sessionCache: SessionCache,
                contextoSesion: ContextoSesion,
                items: FabricaMenusSidebar.ParaAdmin(),
                claveDefault: "tickets",
                alSeleccionar: SeleccionarVista
            );
        }

        protected override void ConfigurarVistaTickets()
        {
            VistaTickets.ConfigurarKpis([
                TipoKpiDashboard.Disponibles,
                TipoKpiDashboard.Urgentes,
                TipoKpiDashboard.EnProceso,
                TipoKpiDashboard.Cerrados,
                TipoKpiDashboard.Reabiertos,
                TipoKpiDashboard.Calificacion
            ], [_btnNuevoTicket, _btnAbrirReportes]);

            VistaTickets.Filtro.InicializarFiltros(ConfiguracionFiltrosTickets.ObtenerCamposAdmin());
            VistaTickets.ControladorPaginacion.Vincular(async () => { if (!_estaCargando) await FiltrarTicketsAsync(); });

            VistaTickets.RecargarClic += async (_, _) => { await CargarGridCompletoAsync(); await CargarKPIsAsync(); };
            VistaTickets.LimpiarClic += async (_, _) =>
            {
                _estaCargando = true;
                VistaTickets.Filtro.LimpiarFiltros(ConfiguracionFiltrosTickets.ObtenerValoresDefecto());
                VistaTickets.ControladorPaginacion.ReiniciarAPrimeraPagina();
                _estaCargando = false;
                await CargarGridCompletoAsync();
            };
            VistaTickets.Grid.CellDoubleClick += async (_, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    await VistaTickets.Grid.ManejarDetalleTicketAsync(
                        e.RowIndex, FormFactory,
                        () => Task.WhenAll(CargarKPIsAsync(), CargarGridCompletoAsync()),
                        "Folio");
                }
            };
            VistaTickets.KpiClic += tipo =>
            {
                switch (tipo)
                {
                    case TipoKpiDashboard.Disponibles:
                        VistaTickets.Filtro.EstablecerValorFiltro("Estatus", ConstantesEstatus.DISPONIBLE);
                        break;
                    case TipoKpiDashboard.Urgentes:
                        VistaTickets.Filtro.EstablecerValorFiltro("Estatus", ConstantesEstatus.URGENTE);
                        break;
                    case TipoKpiDashboard.EnProceso:
                        VistaTickets.Filtro.EstablecerValorFiltro("Estatus", ConstantesEstatus.EN_PROCESO);
                        break;
                    case TipoKpiDashboard.Cerrados:
                        VistaTickets.Filtro.EstablecerValorFiltro("Estatus", ConstantesEstatus.CERRADO);
                        break;
                    case TipoKpiDashboard.Reabiertos:
                        VistaTickets.Filtro.EstablecerValorFiltro("Estatus", ConstantesEstatus.REABIERTO);
                        break;
                    case TipoKpiDashboard.Calificacion:
                        AbrirModalCalificaciones();
                        break;
                }
            };

            VistaTickets.FiltroCambiado += async (_, _) =>
            {
                if (!_estaCargando)
                {
                    VistaTickets.ControladorPaginacion.ReiniciarAPrimeraPagina();
                    await FiltrarTicketsAsync();
                }
            };

            _btnNuevoTicket.Click += async (s, e) =>
            {
                try
                {
                    using var frm = FormFactory.Crear<NuevoTicketForm>();
                    if (frm.ShowDialog(Formulario) == DialogResult.OK)
                    {
                        await Task.WhenAll(CargarKPIsAsync(), CargarGridCompletoAsync());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al crear ticket: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            _btnAbrirReportes.Click += (s, e) => AbrirReportes();
        }

        public override async Task RecargarDatosAsync()
        {
            await Task.WhenAll(CargarKPIsAsync(), CargarGridCompletoAsync());
        }

        public void SeleccionarVista(string clave)
        {
            if (clave == "reportes")
            {
                AbrirReportes();
                Sidebar.SeleccionarItem("tickets");
                TopBar.ActualizarItemActivo("tickets");
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

            TopBar.Titulo = nombreTab == "Tickets" ? "Panel de Control" : $"Catálogo: {nombreTab}";
            TopBar.Subtitulo = nombreTab == "Tickets" ? "Mesa de Servicio y Gestión Global" : $"Administración de registros de {nombreTab}";

            foreach (TabPage tab in _tabMain.TabPages)
            {
                if (tab.Text.Equals(nombreTab, StringComparison.OrdinalIgnoreCase) ||
                    tab.Name.Equals("tab" + nombreTab, StringComparison.OrdinalIgnoreCase))
                {
                    _tabMain.SelectedTab = tab;
                    break;
                }
            }

            Sidebar.SeleccionarItem(clave);
            TopBar.ActualizarItemActivo(clave);
        }

        private void AbrirReportes()
        {
            var modal = FormFactory.Crear<GeneradorReportesForm>();
            modal.ShowDialog();
        }

        private void AbrirModalCalificaciones()
        {
            var modal = FormFactory.Crear<GeneradorReportesForm>();
            modal.ShowDialog();
        }

        public async Task FiltrarTicketsAsync()
        {
            if (_estaCargando) return;

            await Formulario.EjecutarOperacionAsync(async () =>
            {
                var filtros = ConfiguracionFiltrosTickets.MapearFiltrosAdmin(VistaTickets.Filtro.ObtenerValoresFiltros());
                var ctrl = VistaTickets.ControladorPaginacion;
                var resultado = await _ticketService.ObtenerTicketsFiltradosPaginadosAsync(filtros, ctrl.PaginaActual, ctrl.TamanoPagina);
                MostrarTickets(resultado.Items, resultado.TotalCount);
            }, "Error al filtrar tickets");
        }

        public async Task CargarGridCompletoAsync()
        {
            VistaTickets.ControladorPaginacion.ReiniciarAPrimeraPagina();
            await FiltrarTicketsAsync();
        }

        public async Task CargarKPIsAsync()
        {
            await Formulario.EjecutarOperacionAsync(async () =>
            {
                var resumen = await _ticketService.ObtenerResumenDashboardAsync();
                VistaTickets.ActualizarValorKpi(TipoKpiDashboard.Disponibles, resumen.TicketsNuevos);
                VistaTickets.ActualizarValorKpi(TipoKpiDashboard.Urgentes, resumen.TicketsUrgentes);
                VistaTickets.ActualizarValorKpi(TipoKpiDashboard.EnProceso, resumen.TicketsEnProceso);
                VistaTickets.ActualizarValorKpi(TipoKpiDashboard.Cerrados, resumen.TicketsCerrados);
                VistaTickets.ActualizarValorKpi(TipoKpiDashboard.Reabiertos, resumen.TicketsReabiertos);
                VistaTickets.ActualizarValorKpi(TipoKpiDashboard.Calificacion, resumen.PromedioCalificacion, esCalificacionEstrellas: true);
            }, "Error al cargar KPIs de administración");
        }

        public async Task CargarCombosFiltrosAsync()
        {
            await Formulario.EjecutarOperacionAsync(async () =>
            {
                var tecnicosYAdmins = await ObtenerTecnicosYAdminsAsync();
                VistaTickets.Filtro.ConfigurarOpcionesCombo("Tecnico", tecnicosYAdmins);
                VistaTickets.Filtro.ConfigurarOpcionesCombo("Estatus", new List<string> { "Todos", "Nuevos", "Urgentes", ConstantesEstatus.ABIERTO, ConstantesEstatus.EN_PROCESO, ConstantesEstatus.CERRADO, ConstantesEstatus.REABIERTO });
                VistaTickets.Filtro.ConfigurarOpcionesCombo("Prioridad", new List<string> { "Todos", ConstantesPrioridad.ALTA, ConstantesPrioridad.MEDIA, ConstantesPrioridad.BAJA });
            }, "Error al cargar filtros de técnicos y administradores");
        }

        private async Task<List<ElementoOpcionCombo>> ObtenerTecnicosYAdminsAsync()
        {
            var resultado = new List<ElementoOpcionCombo> { new(0, "Todos"), new(-1, "Sin Asignar") };
            var tareaAdmins = _usuarioService.ObtenerUsuariosPorRolAsync(RolUsuarioEnum.Administrador);
            var tareaTecnicos = _usuarioService.ObtenerUsuariosPorRolAsync(RolUsuarioEnum.Tecnico);
            await Task.WhenAll(tareaAdmins, tareaTecnicos);

            var admins = await tareaAdmins ?? [];
            var tecnicos = await tareaTecnicos ?? [];

            foreach (var a in admins) resultado.Add(new(a.IdUsuario, $"Admin - {a.Nombre}"));
            foreach (var t in tecnicos) resultado.Add(new(t.IdUsuario, $"Técnico - {t.Nombre}"));

            return resultado;
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
                    Action<Panel, DataGridView>? onExtra = tipo == typeof(MaterialDto)
                        ? AgregarControlesInventario
                        : null;

                    var tab = AyudanteCatalogoTab.CrearTabCatalogo(nombre, tipo, _catalogoService, onExtra);
                    _tabMain.TabPages.Add(tab);

                    if (tab.Controls["dgv" + nombre] is DataGridView dgv)
                    {
                        await AyudanteCatalogoTab.CargarDatosAsync(dgv, tipo, _catalogoService);
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Error(ex, "Error al configurar la pestaña de catálogo '{Nombre}'.", nombre);
                }
            }
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
                var frmK = FormFactory.Crear<KardexForm>();
                frmK.ShowDialog();
            };

            panelTop.Controls.Add(btnKardex);
        }

        private void MostrarTickets(List<TicketDto> tickets, int totalCount)
        {
            var grid = VistaTickets.Grid;
            grid.DataSource = new ListaVinculableOrdenable<TicketDto>(tickets);
            ConfiguracionColumnasDashboard.AplicarPerfilAdmin(grid);
            VistaTickets.ControladorPaginacion.Actualizar(totalCount);
        }
    }
}
