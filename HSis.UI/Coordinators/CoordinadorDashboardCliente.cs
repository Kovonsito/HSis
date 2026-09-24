#nullable enable
using System.Runtime.Versioning;
using HSis.Contracts.Constants;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.UI.Controls;
using HSis.UI.Factories;
using HSis.UI.Forms.Tickets;
using HSis.UI.Helpers;

namespace HSis.UI.Coordinators
{
    /// <summary>
    /// Coordinador unificado para el Dashboard del Cliente.
    /// Maneja datos de resumen, vistas de Activos/Cerrados, KPIs y creación de nuevos reportes.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class CoordinadorDashboardCliente(
        Form formulario,
        SidebarControl sidebar,
        TopBarControl topBar,
        VistaTicketsDashboardControl vistaTickets,
        Control btnNuevoReporte,
        ITicketService ticketService,
        IAdministradorSesionUsuario contextoSesion,
        IAlmacenamientoCredencialesLocal sessionCache,
        IFabricaFormularios formFactory,
        IClienteSignalRNotificaciones notificationClient) : CoordinadorDashboardMemoria(formulario, sidebar, topBar, vistaTickets, contextoSesion, sessionCache, formFactory, notificationClient)
    {
        private readonly ITicketService _ticketService = ticketService;
        private readonly Control _btnNuevoReporte = btnNuevoReporte;

        public enum VistaCliente { Todos, Activos, Cerrados }
        public VistaCliente VistaActual { get; private set; } = VistaCliente.Todos;

        protected override void ConfigurarNavegacion()
        {
            ConfiguradorSidebarDashboard.Configurar(
                sidebar: Sidebar,
                topBar: TopBar,
                sessionCache: SessionCache,
                contextoSesion: ContextoSesion,
                items: FabricaMenusSidebar.ParaCliente(),
                claveDefault: "activos",
                alSeleccionar: SeleccionarVista
            );
        }

        protected override void ConfigurarVistaTickets()
        {
            VistaTickets.ConfigurarKpis([TipoKpiDashboard.MisActivos, TipoKpiDashboard.MisCerrados], [_btnNuevoReporte]);
            VistaTickets.Filtro.InicializarFiltros(ConfiguracionFiltrosTickets.ObtenerCamposCliente());
            VistaTickets.ControladorPaginacion.Vincular(() => { MostrarPaginaActual(); return Task.CompletedTask; });

            VistaTickets.RecargarClic += (_, _) => _ = RecargarDatosAsync();
            VistaTickets.FiltroCambiado += (_, _) =>
            {
                VistaTickets.ControladorPaginacion.ReiniciarAPrimeraPagina();
                MostrarPaginaActual();
            };
            VistaTickets.LimpiarClic += (_, _) =>
            {
                VistaTickets.ControladorPaginacion.ReiniciarAPrimeraPagina();
                MostrarPaginaActual();
            };
            VistaTickets.Grid.CellDoubleClick += async (_, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    await VistaTickets.Grid.ManejarDetalleTicketAsync(e.RowIndex, FormFactory, RecargarDatosAsync, "IdTicket", esCliente: true);
                }
            };
            VistaTickets.KpiClic += tipo =>
            {
                if (tipo == TipoKpiDashboard.MisActivos)
                    AlternarVista(VistaCliente.Activos, "activos", "Mis Tickets Activos", "Solicitudes en proceso y pendientes");
                else if (tipo == TipoKpiDashboard.MisCerrados)
                    AlternarVista(VistaCliente.Cerrados, "cerrados", "Historial de Tickets Cerrados", "Solicitudes resueltas");
            };

            _btnNuevoReporte.Click += (s, e) =>
            {
                using var frmNuevo = FormFactory.Crear<NuevoTicketForm>();
                if (frmNuevo.ShowDialog(Formulario) == DialogResult.OK)
                {
                    _ = RecargarDatosAsync();
                }
            };
        }

        public override async Task RecargarDatosAsync()
        {
            await Formulario.EjecutarOperacionAsync(async () =>
            {
                var resumen = await _ticketService.ObtenerResumenClienteAsync(ContextoSesion.IdUsuario);
                VistaTickets.ActualizarValorKpi(TipoKpiDashboard.MisActivos, resumen.Activos);
                VistaTickets.ActualizarValorKpi(TipoKpiDashboard.MisCerrados, resumen.Cerrados);
                TodosLosTickets = resumen.Tickets;
                VistaTickets.ControladorPaginacion.ReiniciarAPrimeraPagina();
                MostrarPaginaActual();
            }, "Error al cargar tickets del cliente");
        }

        public void SeleccionarVista(string clave)
        {
            if (clave == "activos")
            {
                VistaActual = VistaCliente.Activos;
                TopBar.Titulo = "Mis Tickets Activos";
                TopBar.Subtitulo = "Solicitudes en proceso y pendientes de atención";
            }
            else if (clave == "cerrados")
            {
                VistaActual = VistaCliente.Cerrados;
                TopBar.Titulo = "Historial de Tickets Cerrados";
                TopBar.Subtitulo = "Solicitudes resueltas y cerradas";
            }
            else
            {
                VistaActual = VistaCliente.Todos;
                TopBar.Titulo = "Todos Mis Tickets";
                TopBar.Subtitulo = "Historial completo de solicitudes";
            }

            VistaTickets.ControladorPaginacion.ReiniciarAPrimeraPagina();
            MostrarPaginaActual();
        }

        private void AlternarVista(VistaCliente vista, string clave, string titulo, string subtitulo)
        {
            VistaActual = VistaActual == vista ? VistaCliente.Todos : vista;
            string claveActiva = VistaActual == vista ? clave : "";
            Sidebar.SeleccionarItem(claveActiva);
            TopBar.ActualizarItemActivo(claveActiva);
            TopBar.Titulo = VistaActual == vista ? titulo : "Todos Mis Tickets";
            TopBar.Subtitulo = VistaActual == vista ? subtitulo : "Historial completo de solicitudes";
            VistaTickets.ControladorPaginacion.ReiniciarAPrimeraPagina();
            MostrarPaginaActual();
        }

        protected override IEnumerable<TicketDto> AplicarFiltroPorEstado(IEnumerable<TicketDto> tickets)
        {
            if (VistaActual == VistaCliente.Activos)
                return tickets.Where(t => t.Status != ConstantesEstatus.CERRADO);
            if (VistaActual == VistaCliente.Cerrados)
                return tickets.Where(t => t.Status == ConstantesEstatus.CERRADO);
            return tickets;
        }

        protected override void AplicarPerfilColumnas(DataGridView grid)
        {
            ConfiguracionColumnasDashboard.AplicarPerfilCliente(grid);
        }
    }
}
