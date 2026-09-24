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
    /// Coordinador unificado para el Dashboard del Técnico.
    /// Maneja tickets asignados, cola disponible, historial, calificaciones y kardex.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class CoordinadorDashboardTecnico(
        Form formulario,
        SidebarControl sidebar,
        TopBarControl topBar,
        VistaTicketsDashboardControl vistaTickets,
        Control btnNuevoTicket,
        ITicketService ticketService,
        IAdministradorSesionUsuario contextoSesion,
        IAlmacenamientoCredencialesLocal sessionCache,
        IFabricaFormularios formFactory,
        IClienteSignalRNotificaciones notificationClient) : CoordinadorDashboardBase(formulario, sidebar, topBar, vistaTickets, contextoSesion, sessionCache, formFactory, notificationClient)
    {
        public enum VistaTecnico
        {
            MisAsignados,
            Disponibles,
            Cerrados,
            Calificaciones
        }

        private readonly ITicketService _ticketService = ticketService;
        private readonly Control _btnNuevoTicket = btnNuevoTicket;

        private VistaTecnico _vistaActual = VistaTecnico.MisAsignados;
        private List<TicketDto> _todosLosTickets = [];
        private List<TicketDto> _ticketsFiltrados = [];
        private List<FeedbackTecnicoDto> _todosLosFeedbacks = [];
        private bool _estaCargando = false;

        protected override void ConfigurarNavegacion()
        {
            ConfiguradorSidebarDashboard.Configurar(
                sidebar: Sidebar,
                topBar: TopBar,
                sessionCache: SessionCache,
                contextoSesion: ContextoSesion,
                items: FabricaMenusSidebar.ParaTecnico(),
                claveDefault: "asignados",
                alSeleccionar: SeleccionarVista
            );
        }

        protected override void ConfigurarVistaTickets()
        {
            VistaTickets.ConfigurarKpis([
                TipoKpiDashboard.MisAsignados,
                TipoKpiDashboard.Disponibles,
                TipoKpiDashboard.MisCerrados,
                TipoKpiDashboard.Calificacion
            ], [_btnNuevoTicket]);

            VistaTickets.Filtro.InicializarFiltros(ConfiguracionFiltrosTickets.ObtenerCamposTecnico());
            VistaTickets.ControladorPaginacion.Vincular(() => { MostrarPaginaActual(); return Task.CompletedTask; });

            VistaTickets.RecargarClic += (_, _) => _ = RecargarDatosAsync();
            VistaTickets.FiltroCambiado += (_, _) => { if (!_estaCargando) AplicarFiltrosMemoria(); };
            VistaTickets.LimpiarClic += (_, _) =>
            {
                _estaCargando = true;
                VistaTickets.Filtro.LimpiarFiltros(ConfiguracionFiltrosTickets.ObtenerValoresDefecto());
                _estaCargando = false;
                AplicarFiltrosMemoria();
            };
            VistaTickets.Grid.CellDoubleClick += async (_, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    await VistaTickets.Grid.ManejarDetalleTicketAsync(e.RowIndex, FormFactory, RecargarDatosAsync);
                }
            };
            VistaTickets.KpiClic += tipo =>
            {
                switch (tipo)
                {
                    case TipoKpiDashboard.MisAsignados:
                        CambiarVista(VistaTecnico.MisAsignados, "asignados", "Mis Tickets Asignados", "Tickets activos bajo mi responsabilidad");
                        break;
                    case TipoKpiDashboard.Disponibles:
                        CambiarVista(VistaTecnico.Disponibles, "disponibles", "Tickets Disponibles en Cola", "Tickets abiertos listos para ser tomados");
                        break;
                    case TipoKpiDashboard.MisCerrados:
                        CambiarVista(VistaTecnico.Cerrados, "cerrados", "Historial de Tickets Cerrados", "Tickets completados satisfactoriamente");
                        break;
                    case TipoKpiDashboard.Calificacion:
                        CambiarVista(VistaTecnico.Calificaciones, "calificaciones", "Mis Calificaciones", "Evaluaciones y comentarios de usuarios");
                        break;
                }
            };

            _btnNuevoTicket.Click += async (s, e) =>
            {
                try
                {
                    using var frm = FormFactory.Crear<NuevoTicketForm>();
                    if (frm.ShowDialog(Formulario) == DialogResult.OK)
                    {
                        await RecargarDatosAsync();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al abrir el formulario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
        }

        public override async Task RecargarDatosAsync()
        {
            await Task.WhenAll(CargarIndicadoresAsync(ContextoSesion.IdUsuario), CargarTicketsSegunVistaAsync());
        }

        public async void SeleccionarVista(string clave)
        {
            if (clave == "kardex")
            {
                var frmK = FormFactory.Crear<KardexForm>();
                frmK.ShowDialog();
                string claveActual = _vistaActual switch
                {
                    VistaTecnico.MisAsignados => "asignados",
                    VistaTecnico.Disponibles => "disponibles",
                    VistaTecnico.Cerrados => "cerrados",
                    VistaTecnico.Calificaciones => "calificaciones",
                    _ => "asignados"
                };
                Sidebar.SeleccionarItem(claveActual);
                TopBar.ActualizarItemActivo(claveActual);
                return;
            }

            _vistaActual = clave switch
            {
                "asignados" => VistaTecnico.MisAsignados,
                "disponibles" => VistaTecnico.Disponibles,
                "cerrados" => VistaTecnico.Cerrados,
                "calificaciones" => VistaTecnico.Calificaciones,
                _ => VistaTecnico.MisAsignados
            };

            Sidebar.SeleccionarItem(clave);
            TopBar.ActualizarItemActivo(clave);

            TopBar.Titulo = _vistaActual switch
            {
                VistaTecnico.MisAsignados => "Mis Tickets Asignados",
                VistaTecnico.Disponibles => "Tickets Disponibles en Cola",
                VistaTecnico.Cerrados => "Historial de Tickets Cerrados",
                VistaTecnico.Calificaciones => "Mis Calificaciones",
                _ => "Panel Técnico"
            };

            TopBar.Subtitulo = _vistaActual switch
            {
                VistaTecnico.MisAsignados => "Tickets activos bajo mi responsabilidad",
                VistaTecnico.Disponibles => "Tickets abiertos listos para ser tomados",
                VistaTecnico.Cerrados => "Tickets completados satisfactoriamente",
                VistaTecnico.Calificaciones => "Evaluaciones y comentarios de usuarios",
                _ => string.Empty
            };

            await CargarTicketsSegunVistaAsync();
        }

        private async void CambiarVista(VistaTecnico vista, string clave, string titulo, string subtitulo)
        {
            _vistaActual = vista;
            Sidebar.SeleccionarItem(clave);
            TopBar.ActualizarItemActivo(clave);
            TopBar.Titulo = titulo;
            TopBar.Subtitulo = subtitulo;
            await CargarTicketsSegunVistaAsync();
        }

        private async Task CargarIndicadoresAsync(int idTecnico)
        {
            await Formulario.EjecutarOperacionAsync(async () =>
            {
                var ind = await _ticketService.ObtenerIndicadoresTecnicoAsync(idTecnico);
                VistaTickets.ActualizarValorKpi(TipoKpiDashboard.MisAsignados, ind.Asignados);
                VistaTickets.ActualizarValorKpi(TipoKpiDashboard.Disponibles, ind.Disponibles);
                VistaTickets.ActualizarValorKpi(TipoKpiDashboard.MisCerrados, ind.Cerrados);
                VistaTickets.ActualizarValorKpi(TipoKpiDashboard.Calificacion, ind.PromedioCalificacion, esCalificacionEstrellas: true);
            }, "Error al cargar indicadores técnicos");
        }

        private async Task CargarTicketsSegunVistaAsync()
        {
            await Formulario.EjecutarOperacionAsync(async () =>
            {
                int idTecnico = ContextoSesion.IdUsuario;
                if (_vistaActual == VistaTecnico.MisAsignados)
                {
                    _todosLosTickets = await _ticketService.ObtenerTicketsAsignadosATecnicoAsync(idTecnico);
                    AplicarFiltrosMemoria();
                }
                else if (_vistaActual == VistaTecnico.Disponibles)
                {
                    _todosLosTickets = await _ticketService.ObtenerTicketsDisponiblesAsync();
                    AplicarFiltrosMemoria();
                }
                else if (_vistaActual == VistaTecnico.Cerrados)
                {
                    _todosLosTickets = await _ticketService.ObtenerTicketsCerradosPorTecnicoAsync(idTecnico);
                    AplicarFiltrosMemoria();
                }
                else if (_vistaActual == VistaTecnico.Calificaciones)
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

        private void MostrarPaginaActual()
        {
            var grid = VistaTickets.Grid;
            var ctrl = VistaTickets.ControladorPaginacion;

            if (_vistaActual == VistaTecnico.Calificaciones)
            {
                var pageFeedbacks = ctrl.ObtenerPagina(_todosLosFeedbacks).ToList();
                grid.DataSource = new ListaVinculableOrdenable<FeedbackTecnicoDto>(pageFeedbacks);
                ctrl.Actualizar(_todosLosFeedbacks.Count);
                ConfiguracionColumnasDashboard.AplicarPerfilCalificaciones(grid);
            }
            else
            {
                var pageTickets = ctrl.ObtenerPagina(_ticketsFiltrados).ToList();
                grid.DataSource = new ListaVinculableOrdenable<TicketDto>(pageTickets);
                ctrl.Actualizar(_ticketsFiltrados.Count);
                ConfiguracionColumnasDashboard.AplicarPerfilTecnico(grid);
            }
        }

        private void AplicarFiltrosMemoria()
        {
            var vals = VistaTickets.Filtro.ObtenerValoresFiltros();
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

            VistaTickets.ControladorPaginacion.ReiniciarAPrimeraPagina();
            MostrarPaginaActual();
        }
    }
}
