#nullable enable
using System.Runtime.Versioning;
using HSis.Contracts.Constants;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.UI.Controls;
using HSis.UI.Factories;
using HSis.UI.Forms.Tickets;
using HSis.UI.Helpers;
using HSis.UI.Services;

namespace HSis.UI.Forms.Dashboards
{
    [SupportedOSPlatform("windows")]
    public partial class DashboardClienteForm : Form
    {
        private readonly ITicketService _ticketService;
        private readonly IAdministradorSesionUsuario _contextoSesion;
        private readonly IAlmacenamientoCredencialesLocal _sessionCache;
        private readonly IFabricaFormularios _formFactory;
        private readonly IClienteSignalRNotificaciones _notificationClient;

        private List<TicketDto> _todosLosTickets = [];
        private IndicadorControl ucMisCerrados = null!;

        private enum VistaCliente { Todos, Activos, Cerrados }
        private VistaCliente _vistaActual = VistaCliente.Todos;

        public DashboardClienteForm(
            ITicketService ticketService,
            IAdministradorSesionUsuario contextoSesion,
            IAlmacenamientoCredencialesLocal sessionCache,
            IFabricaFormularios formFactory,
            IClienteSignalRNotificaciones notificationClient)
        {
            InitializeComponent();
            _ticketService = ticketService;
            _contextoSesion = contextoSesion;
            _sessionCache = sessionCache;
            _formFactory = formFactory;
            _notificationClient = notificationClient;
        }

        private async void frmDashboardCliente_Load(object? sender, EventArgs e)
        {
            ucMisCerrados = new IndicadorControl();
            ucMisActivos.IndicadorClic += UcMisActivos_Click;
            ucMisCerrados.IndicadorClic += UcMisCerrados_Click;

            // ── Bloque 3: configuración centralizada de VistaTicketsDashboardControl ──
            vistaTickets.ConfigurarComportamiento(new ConfiguracionVistaDashboard
            {
                Indicadores     = [ucMisActivos, ucMisCerrados, btnNuevoReporte],
                Filtros         = ConfiguracionFiltrosTickets.ObtenerCamposCliente(),
                AlMostrarPagina = () => { MostrarPaginaActual(); return Task.CompletedTask; },
                AlRecargar      = CargarDatosDashboardAsync,
                AlDobleClicFila = async rowIndex =>
                {
                    var row = vistaTickets.Grid.Rows[rowIndex];
                    if (int.TryParse(row.Cells["IdTicket"].Value?.ToString(), out int idTicket))
                    {
                        using var frmDetalle = _formFactory.CrearDetalleCliente(idTicket);
                        frmDetalle.ShowDialog();
                        await CargarDatosDashboardAsync();
                    }
                }
            });

            // ── Bloque 1: sidebar + topBar ────────────────────────────────────────────
            var items = new[]
            {
                new ItemSidebar { Clave = "activos",  Titulo = "Mis Activos",        Icono = FontAwesome.Sharp.IconChar.Ticket },
                new ItemSidebar { Clave = "cerrados", Titulo = "Historial Cerrados",  Icono = FontAwesome.Sharp.IconChar.ClockRotateLeft }
            };

            ConfiguradorSidebarDashboard.Configurar(
                sidebar:        sidebarCliente,
                topBar:         topBarCliente,
                sessionCache:   _sessionCache,
                contextoSesion: _contextoSesion,
                items:          items,
                claveDefault:   "activos",
                alSeleccionar:  SeleccionarVista
            );

            this.IntegrarNotificacionesModerno(topBarCliente, _formFactory, _contextoSesion,
                _notificationClient, null, CargarDatosDashboardAsync);

            await CargarDatosDashboardAsync();
        }

        // ── Selección de vista (lógica propia del rol Cliente) ────────────────────────
        private void SeleccionarVista(string clave)
        {
            if (clave == "activos")
            {
                _vistaActual = VistaCliente.Activos;
                topBarCliente.Titulo    = "Mis Tickets Activos";
                topBarCliente.Subtitulo = "Solicitudes en proceso y pendientes de atención";
            }
            else if (clave == "cerrados")
            {
                _vistaActual = VistaCliente.Cerrados;
                topBarCliente.Titulo    = "Historial de Tickets Cerrados";
                topBarCliente.Subtitulo = "Solicitudes resueltas y cerradas";
            }

            sidebarCliente.SeleccionarItem(clave);
            topBarCliente.ActualizarItemActivo(clave);
            vistaTickets.ReiniciarAPrimeraPagina();
            MostrarPaginaActual();
        }

        // ── Carga de datos ────────────────────────────────────────────────────────────
        private async Task CargarDatosDashboardAsync()
        {
            await this.EjecutarOperacionAsync(async () =>
            {
                var resumen = await _ticketService.ObtenerResumenClienteAsync(_contextoSesion.IdUsuario);
                MostrarIndicadores(resumen.Activos, resumen.Cerrados);
                _todosLosTickets = resumen.Tickets;
                vistaTickets.ReiniciarAPrimeraPagina();
                MostrarPaginaActual();
            }, "Error al cargar tickets del cliente");
        }

        // ── Visualización de página actual ────────────────────────────────────────────
        private void MostrarPaginaActual()
        {
            // 1. Filtrar por vista del Sidebar
            var query = _todosLosTickets.AsEnumerable();
            if (_vistaActual == VistaCliente.Activos)
                query = query.Where(t => t.Status != ConstantesEstatus.CERRADO);
            else if (_vistaActual == VistaCliente.Cerrados)
                query = query.Where(t => t.Status == ConstantesEstatus.CERRADO);

            // 2. Filtros dinámicos (texto + fechas)
            var (txt, dtInicio, dtFin, _, _) = vistaTickets.ObtenerValoresFiltros().ExtraerFiltrosComunes();
            if (!string.IsNullOrEmpty(txt))
            {
                query = query.Where(t =>
                    (t.Folio.ToString().Contains(txt) || t.FolioFormato.ToLowerInvariant().Contains(txt)) ||
                    (t.Descripcion?.ToLowerInvariant().Contains(txt) ?? false) ||
                    (t.TecnicoAsignado?.ToLowerInvariant().Contains(txt) ?? false));
            }
            if (dtInicio.HasValue) query = query.Where(t => t.FechaAlta >= dtInicio.Value);
            if (dtFin.HasValue)    query = query.Where(t => t.FechaAlta <= dtFin.Value);

            var ticketsFiltrados = query.ToList();

            // 3. Paginar y mostrar
            var pageTickets = vistaTickets.ObtenerPagina(ticketsFiltrados).ToList();
            vistaTickets.Grid.DataSource = new ListaVinculableOrdenable<TicketDto>(pageTickets);

            // ── Bloque 2: perfil de columnas centralizado ─────────────────────────────
            ConfiguracionColumnasDashboard.AplicarPerfilCliente(vistaTickets.Grid);

            vistaTickets.ActualizarPaginacion(ticketsFiltrados.Count);
        }

        // ── Clics en KPIs ─────────────────────────────────────────────────────────────
        private void UcMisActivos_Click(object? sender, EventArgs e)
        {
            _vistaActual = _vistaActual == VistaCliente.Activos ? VistaCliente.Todos : VistaCliente.Activos;
            string clave = _vistaActual == VistaCliente.Activos ? "activos" : "";
            sidebarCliente.SeleccionarItem(clave);
            topBarCliente.ActualizarItemActivo(clave);
            topBarCliente.Titulo = _vistaActual == VistaCliente.Activos ? "Mis Tickets Activos" : "Todos Mis Tickets";
            vistaTickets.ReiniciarAPrimeraPagina();
            MostrarPaginaActual();
        }

        private void UcMisCerrados_Click(object? sender, EventArgs e)
        {
            _vistaActual = _vistaActual == VistaCliente.Cerrados ? VistaCliente.Todos : VistaCliente.Cerrados;
            string clave = _vistaActual == VistaCliente.Cerrados ? "cerrados" : "";
            sidebarCliente.SeleccionarItem(clave);
            topBarCliente.ActualizarItemActivo(clave);
            topBarCliente.Titulo = _vistaActual == VistaCliente.Cerrados ? "Historial de Tickets Cerrados" : "Todos Mis Tickets";
            vistaTickets.ReiniciarAPrimeraPagina();
            MostrarPaginaActual();
        }

        // ── Nuevo ticket ──────────────────────────────────────────────────────────────
        private void btnNuevoReporte_Click(object? sender, EventArgs e)
        {
            using var frmNuevo = _formFactory.Crear<NuevoTicketForm>();
            if (frmNuevo.ShowDialog() == DialogResult.OK)
                _ = CargarDatosDashboardAsync();
        }

        // ── Indicadores KPI ───────────────────────────────────────────────────────────
        public void MostrarIndicadores(int activos, int cerrados)
        {
            ucMisActivos.Cantidad  = activos.ToString();
            ucMisActivos.Titulo    = "Mis Tickets Activos";
            ucMisActivos.ColorFondo = TemaVisual.TicketNuevo;

            ucMisCerrados.Cantidad  = cerrados.ToString();
            ucMisCerrados.Titulo    = "Mis Tickets Cerrados";
            ucMisCerrados.ColorFondo = TemaVisual.TicketCerrado;
        }
    }
}
