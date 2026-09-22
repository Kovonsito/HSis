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
    public partial class DashboardTecnicoForm : Form
    {
        private readonly ITicketService _ticketService;
        private readonly IAdministradorSesionUsuario _contextoSesion;
        private readonly IAlmacenamientoCredencialesLocal _sessionCache;
        private readonly IFabricaFormularios _formFactory;
        private readonly IClienteSignalRNotificaciones _notificationClient;

        private enum VistaDashboard
        {
            MisAsignados,
            Disponibles,
            Cerrados,
            Calificaciones
        }
        private VistaDashboard _vistaActual = VistaDashboard.MisAsignados;
        private List<TicketDto> _todosLosTickets = [];
        private List<TicketDto> _ticketsFiltrados = [];
        private List<FeedbackTecnicoDto> _todosLosFeedbacks = [];
        private bool _estaCargando = false;

        public DashboardTecnicoForm(
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

        private async void frmDashboardTecnico_Load(object? sender, EventArgs e)
        {
            // Conectar indicadores KPI
            ucMisAsignados.IndicadorClic += UcMisAsignados_Click;
            ucDisponibles.IndicadorClic  += UcDisponibles_Click;
            ucCerrados.IndicadorClic     += UcCerrados_Click;
            ucCalificacion.IndicadorClic += UcCalificacion_Click;

            // ── Bloque 3: configuración centralizada de VistaTicketsDashboardControl ──
            vistaTickets.ConfigurarComportamiento(new ConfiguracionVistaDashboard
            {
                Indicadores     = [ucMisAsignados, ucDisponibles, ucCerrados, ucCalificacion, btnNuevoTicket],
                Filtros         = ConfiguracionFiltrosTickets.ObtenerCamposTecnico(),
                AlMostrarPagina = () => { MostrarPaginaActual(); return Task.CompletedTask; },
                AlRecargar      = CargarDatosInicialesAsync,
                AlLimpiar       = () =>
                {
                    _estaCargando = true;
                    vistaTickets.Filtro.LimpiarFiltros(ConfiguracionFiltrosTickets.ObtenerValoresDefecto());
                    _estaCargando = false;
                    AplicarFiltrosMemoria();
                    return Task.CompletedTask;
                },
                AlDobleClicFila = async rowIndex =>
                    await vistaTickets.Grid.ManejarDetalleTicketAsync(rowIndex, _formFactory, () => CargarDatosInicialesAsync())
            });

            // El evento FiltroCambiado del Bloque3 llama a MostrarPaginaActual en lugar de AplicarFiltrosMemoria;
            // sobreescribimos para respetar la lógica de filtrado en memoria del técnico.
            vistaTickets.FiltroCambiado += (_, _) => { if (!_estaCargando) AplicarFiltrosMemoria(); };

            // ── Bloque 1: sidebar + topBar ────────────────────────────────────────────
            var items = new[]
            {
                new ItemSidebar { Clave = "asignados",    Titulo = "Mis Asignados",   Icono = FontAwesome.Sharp.IconChar.ClipboardCheck },
                new ItemSidebar { Clave = "disponibles",  Titulo = "Disponibles",      Icono = FontAwesome.Sharp.IconChar.Inbox },
                new ItemSidebar { Clave = "cerrados",     Titulo = "Mis Cerrados",     Icono = FontAwesome.Sharp.IconChar.CheckCircle },
                new ItemSidebar { Clave = "calificaciones",Titulo = "Calificaciones",  Icono = FontAwesome.Sharp.IconChar.Star },
                new ItemSidebar { Clave = "kardex",       Titulo = "Almécn / Kardex",  Icono = FontAwesome.Sharp.IconChar.BoxesStacked }
            };

            ConfiguradorSidebarDashboard.Configurar(
                sidebar:        sidebarTecnico,
                topBar:         topBarTecnico,
                sessionCache:   _sessionCache,
                contextoSesion: _contextoSesion,
                items:          items,
                claveDefault:   "asignados",
                alSeleccionar:  SeleccionarVista
            );

            this.IntegrarNotificacionesModerno(topBarTecnico, _formFactory, _contextoSesion, _notificationClient, null, CargarDatosInicialesAsync);

            await CargarDatosInicialesAsync();
        }

        // ── Selección de vista (lógica propia del rol Técnico) ──────────────────────────
        private async void SeleccionarVista(string clave)
        {
            if (clave == "kardex")
            {
                var frmK = _formFactory.Crear<Forms.Otros.KardexForm>();
                frmK.ShowDialog();
                string claveActual = _vistaActual switch
                {
                    VistaDashboard.MisAsignados  => "asignados",
                    VistaDashboard.Disponibles   => "disponibles",
                    VistaDashboard.Cerrados      => "cerrados",
                    VistaDashboard.Calificaciones=> "calificaciones",
                    _ => "asignados"
                };
                sidebarTecnico.SeleccionarItem(claveActual);
                topBarTecnico.ActualizarItemActivo(claveActual);
                return;
            }

            _vistaActual = clave switch
            {
                "asignados"     => VistaDashboard.MisAsignados,
                "disponibles"   => VistaDashboard.Disponibles,
                "cerrados"      => VistaDashboard.Cerrados,
                "calificaciones"=> VistaDashboard.Calificaciones,
                _ => VistaDashboard.MisAsignados
            };

            sidebarTecnico.SeleccionarItem(clave);
            topBarTecnico.ActualizarItemActivo(clave);

            topBarTecnico.Titulo = _vistaActual switch
            {
                VistaDashboard.MisAsignados   => "Mis Tickets Asignados",
                VistaDashboard.Disponibles    => "Tickets Disponibles en Cola",
                VistaDashboard.Cerrados       => "Historial de Tickets Cerrados",
                VistaDashboard.Calificaciones => "Mis Calificaciones",
                _ => "Panel Técnico"
            };

            topBarTecnico.Subtitulo = _vistaActual switch
            {
                VistaDashboard.MisAsignados   => "Tickets activos bajo mi responsabilidad",
                VistaDashboard.Disponibles    => "Tickets abiertos listos para ser tomados",
                VistaDashboard.Cerrados       => "Tickets completados satisfactoriamente",
                VistaDashboard.Calificaciones => "Evaluaciones y comentarios de usuarios",
                _ => string.Empty
            };

            await CargarTicketsSegunVistaAsync();
        }

        private async Task CargarDatosInicialesAsync()
        {
            await Task.WhenAll(CargarIndicadoresAsync(_contextoSesion.IdUsuario), CargarTicketsSegunVistaAsync());
        }

        private async Task CargarIndicadoresAsync(int idTecnico)
        {
            await this.EjecutarOperacionAsync(async () =>
            {
                var ind = await _ticketService.ObtenerIndicadoresTecnicoAsync(idTecnico);
                MostrarIndicadores(
                    ind.Asignados,
                    ind.Disponibles,
                    ind.Cerrados,
                    ind.PromedioCalificacion
                );
            }, "Error al cargar indicadores técnicos");
        }

        private async Task CargarTicketsSegunVistaAsync()
        {
            await this.EjecutarOperacionAsync(async () =>
            {
                int idTecnico = _contextoSesion.IdUsuario;
                if (_vistaActual == VistaDashboard.MisAsignados)
                {
                    _todosLosTickets = await _ticketService.ObtenerTicketsAsignadosATecnicoAsync(idTecnico);
                    AplicarFiltrosMemoria();
                }
                else if (_vistaActual == VistaDashboard.Disponibles)
                {
                    _todosLosTickets = await _ticketService.ObtenerTicketsDisponiblesAsync();
                    AplicarFiltrosMemoria();
                }
                else if (_vistaActual == VistaDashboard.Cerrados)
                {
                    _todosLosTickets = await _ticketService.ObtenerTicketsCerradosPorTecnicoAsync(idTecnico);
                    AplicarFiltrosMemoria();
                }
                else if (_vistaActual == VistaDashboard.Calificaciones)
                {
                    var feedbacks = await _ticketService.ObtenerFeedbackTecnicoAsync(idTecnico);
                    _todosLosFeedbacks = feedbacks.Select(f => new FeedbackTecnicoDto
                    {
                        IdTicket = f.IdTicket,
                        NombreUsuario = f.Usuario,
                        Calificacion = f.Calificacion.HasValue ? $"{f.Calificacion.Value} ⭐" : "N/A",
                        Comentario = f.ComentarioEvaluacion,
                        Fecha = f.FechaEvaluacion
                    }).ToList();
                    AplicarFiltrosMemoria();
                }
            }, "Error al cargar tickets");
        }

        private async void UcMisAsignados_Click(object? sender, EventArgs e)
        {
            _vistaActual = VistaDashboard.MisAsignados;
            sidebarTecnico.SeleccionarItem("asignados");
            topBarTecnico.ActualizarItemActivo("asignados");
            topBarTecnico.Titulo = "Mis Tickets Asignados";
            await CargarTicketsSegunVistaAsync();
        }

        private async void UcDisponibles_Click(object? sender, EventArgs e)
        {
            _vistaActual = VistaDashboard.Disponibles;
            sidebarTecnico.SeleccionarItem("disponibles");
            topBarTecnico.ActualizarItemActivo("disponibles");
            topBarTecnico.Titulo = "Tickets Disponibles en Cola";
            await CargarTicketsSegunVistaAsync();
        }

        private async void UcCerrados_Click(object? sender, EventArgs e)
        {
            _vistaActual = VistaDashboard.Cerrados;
            sidebarTecnico.SeleccionarItem("cerrados");
            topBarTecnico.ActualizarItemActivo("cerrados");
            topBarTecnico.Titulo = "Historial de Tickets Cerrados";
            await CargarTicketsSegunVistaAsync();
        }

        private async void UcCalificacion_Click(object? sender, EventArgs e)
        {
            _vistaActual = VistaDashboard.Calificaciones;
            sidebarTecnico.SeleccionarItem("calificaciones");
            topBarTecnico.ActualizarItemActivo("calificaciones");
            topBarTecnico.Titulo = "Mis Calificaciones";
            await CargarTicketsSegunVistaAsync();
        }

        private async void btnNuevoTicket_Click(object? sender, EventArgs e)
        {
            try
            {
                using var frm = _formFactory.Crear<NuevoTicketForm>();
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    await CargarDatosInicialesAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al abrir el formulario de registro de ticket: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MostrarPaginaActual()
        {
            var grid = vistaTickets.Grid;
            var ctrl = vistaTickets.ControladorPaginacion;

            if (_vistaActual == VistaDashboard.Calificaciones)
            {
                var pageFeedbacks = ctrl.ObtenerPagina(_todosLosFeedbacks).ToList();
                grid.DataSource = new ListaVinculableOrdenable<FeedbackTecnicoDto>(pageFeedbacks);
                ctrl.Actualizar(_todosLosFeedbacks.Count);
            }
            else
            {
                var pageTickets = ctrl.ObtenerPagina(_ticketsFiltrados).ToList();
                grid.DataSource = new ListaVinculableOrdenable<TicketDto>(pageTickets);
                ctrl.Actualizar(_ticketsFiltrados.Count);
            }
            PersonalizarColumnas();
        }

        // ── Bloque 2: perfiles de columnas centralizados ──────────────────────────────
        private void PersonalizarColumnas()
        {
            if (_vistaActual == VistaDashboard.Calificaciones)
                ConfiguracionColumnasDashboard.AplicarPerfilCalificaciones(vistaTickets.Grid);
            else
                ConfiguracionColumnasDashboard.AplicarPerfilTecnico(vistaTickets.Grid);
        }

        private void AplicarFiltrosMemoria()
        {
            var vals = vistaTickets.ObtenerValoresFiltros();

            var (texto, fi, ff, prioridad, usuario) = vals.ExtraerFiltrosComunes();
            DateTime fechaInicio = fi ?? DateTime.MinValue;
            DateTime fechaFin = ff ?? DateTime.MaxValue;

            _ticketsFiltrados = _todosLosTickets.Where(t =>
            {
                if (texto != null)
                {
                    bool matchTexto = (t.Folio.ToString().Contains(texto) || t.FolioFormato.ToLowerInvariant().Contains(texto)) ||
                                     (t.Descripcion?.ToLowerInvariant().Contains(texto) ?? false) ||
                                     (t.Usuario?.ToLowerInvariant().Contains(texto) ?? false);
                    if (!matchTexto) return false;
                }
                if (prioridad != null && !string.Equals(t.Prioridad, prioridad, StringComparison.OrdinalIgnoreCase)) return false;
                if (usuario != null && !(t.Usuario?.ToLowerInvariant().Contains(usuario) ?? false)) return false;
                if (t.FechaAlta < fechaInicio || t.FechaAlta > fechaFin) return false;
                return true;
            }).ToList();

            vistaTickets.ReiniciarAPrimeraPagina();
            MostrarPaginaActual();
        }

        public void MostrarIndicadores(int asignados, int disponibles, int cerrados, double promedioCalificacion)
        {
            ucMisAsignados.Cantidad = asignados.ToString();
            ucMisAsignados.Titulo = "Mis Asignados";
            ucMisAsignados.ColorFondo = TemaVisual.TicketNuevo;

            ucDisponibles.Cantidad = disponibles.ToString();
            ucDisponibles.Titulo = "Disponibles";
            ucDisponibles.ColorFondo = TemaVisual.TicketEnProceso;

            ucCerrados.Cantidad = cerrados.ToString();
            ucCerrados.Titulo = "Mis Cerrados";
            ucCerrados.ColorFondo = TemaVisual.TicketCerrado;

            ucCalificacion.Cantidad = promedioCalificacion > 0 ? $"⭐ {promedioCalificacion:F1}" : "⭐ N/A";
            ucCalificacion.Titulo = "Mi Calificación";
            ucCalificacion.ColorFondo = TemaVisual.TicketReabierto;
        }
    }
}
