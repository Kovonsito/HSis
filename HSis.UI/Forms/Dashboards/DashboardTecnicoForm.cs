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
        private PaginacionControl PaginacionControl = null!;
        private ControladorPaginacionGrid _controladorPaginacion = null!;
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
            dgvTicketsOperativos.AplicarTemaModerno();
            InicializarLayoutDashboard();
            _controladorPaginacion = new ControladorPaginacionGrid(PaginacionControl);
            _controladorPaginacion.Vincular(MostrarPaginaActual);

            ConfigurarSidebar();
            ConfigurarFiltros();

            this.IntegrarNotificacionesModerno(topBarTecnico, _formFactory, _contextoSesion, _notificationClient, null, CargarDatosInicialesAsync);

            await CargarDatosInicialesAsync();
        }

        private void ConfigurarSidebar()
        {
            var items = new[]
            {
                new ItemSidebar { Clave = "asignados", Titulo = "Mis Asignados", Icono = FontAwesome.Sharp.IconChar.ClipboardCheck },
                new ItemSidebar { Clave = "disponibles", Titulo = "Disponibles", Icono = FontAwesome.Sharp.IconChar.Inbox },
                new ItemSidebar { Clave = "cerrados", Titulo = "Mis Cerrados", Icono = FontAwesome.Sharp.IconChar.CheckCircle },
                new ItemSidebar { Clave = "calificaciones", Titulo = "Calificaciones", Icono = FontAwesome.Sharp.IconChar.Star },
                new ItemSidebar { Clave = "kardex", Titulo = "Almacén / Kardex", Icono = FontAwesome.Sharp.IconChar.BoxesStacked }
            };

            sidebarTecnico.ConfigurarSesion(_sessionCache);
            sidebarTecnico.ConfigurarItems(items, "asignados");

            async void SeleccionarVista(string clave)
            {
                if (clave == "kardex")
                {
                    var frmK = _formFactory.Crear<Forms.Otros.KardexForm>();
                    frmK.ShowDialog();
                    string claveActual = _vistaActual switch
                    {
                        VistaDashboard.MisAsignados => "asignados",
                        VistaDashboard.Disponibles => "disponibles",
                        VistaDashboard.Cerrados => "cerrados",
                        VistaDashboard.Calificaciones => "calificaciones",
                        _ => "asignados"
                    };
                    sidebarTecnico.SeleccionarItem(claveActual);
                    topBarTecnico.ActualizarItemActivo(claveActual);
                    return;
                }

                _vistaActual = clave switch
                {
                    "asignados" => VistaDashboard.MisAsignados,
                    "disponibles" => VistaDashboard.Disponibles,
                    "cerrados" => VistaDashboard.Cerrados,
                    "calificaciones" => VistaDashboard.Calificaciones,
                    _ => VistaDashboard.MisAsignados
                };

                sidebarTecnico.SeleccionarItem(clave);
                topBarTecnico.ActualizarItemActivo(clave);

                topBarTecnico.Titulo = _vistaActual switch
                {
                    VistaDashboard.MisAsignados => "Mis Tickets Asignados",
                    VistaDashboard.Disponibles => "Tickets Disponibles en Cola",
                    VistaDashboard.Cerrados => "Historial de Tickets Cerrados",
                    VistaDashboard.Calificaciones => "Mis Calificaciones",
                    _ => "Panel Técnico"
                };

                topBarTecnico.Subtitulo = _vistaActual switch
                {
                    VistaDashboard.MisAsignados => "Tickets activos bajo mi responsabilidad",
                    VistaDashboard.Disponibles => "Tickets abiertos listos para ser tomados",
                    VistaDashboard.Cerrados => "Tickets completados satisfactoriamente",
                    VistaDashboard.Calificaciones => "Evaluaciones y comentarios de usuarios",
                    _ => string.Empty
                };

                await CargarTicketsSegunVistaAsync();
            }

            sidebarTecnico.ItemSeleccionado += (s, clave) => SeleccionarVista(clave);

            topBarTecnico.ConfigurarSesion(_sessionCache, _contextoSesion);
            topBarTecnico.ConfigurarMenuHamburguesa(
                items,
                "asignados",
                SeleccionarVista,
                () => sidebarTecnico.Colapsado = !sidebarTecnico.Colapsado,
                () => !sidebarTecnico.Colapsado
            );
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

        private void MostrarPaginaActual()
        {
            if (_vistaActual == VistaDashboard.Calificaciones)
            {
                var pageFeedbacks = _todosLosFeedbacks
                    .Skip((_controladorPaginacion.PaginaActual - 1) * _controladorPaginacion.TamanoPagina)
                    .Take(_controladorPaginacion.TamanoPagina)
                    .ToList();
                dgvTicketsOperativos.DataSource = new ListaVinculableOrdenable<FeedbackTecnicoDto>(pageFeedbacks);
                _controladorPaginacion.Actualizar(_todosLosFeedbacks.Count);
            }
            else
            {
                var pageTickets = _ticketsFiltrados
                    .Skip((_controladorPaginacion.PaginaActual - 1) * _controladorPaginacion.TamanoPagina)
                    .Take(_controladorPaginacion.TamanoPagina)
                    .ToList();
                dgvTicketsOperativos.DataSource = new ListaVinculableOrdenable<TicketDto>(pageTickets);
                _controladorPaginacion.Actualizar(_ticketsFiltrados.Count);
            }
            PersonalizarColumnas();
        }

        private void PersonalizarColumnas()
        {
            if (dgvTicketsOperativos.Columns.Count > 0)
            {
                dgvTicketsOperativos.ConfigurarOcultarColumnas(
                    "IdTicket", "IdUsuario", "NombreUsuario", "DepartamentoUsuario",
                    "FechaAtencion", "FechaCierre", "Estatus", "IdTecnico", "NombreTecnico",
                    "TecnicoAsignado", "Calificacion", "ComentarioEvaluacion", "FechaEvaluacion",
                    "Evaluacion", "Feedback", "FolioFormato");

                if (dgvTicketsOperativos.DataSource is ListaVinculableOrdenable<FeedbackTecnicoDto>)
                {
                    dgvTicketsOperativos.ConfigurarColumnas(
                        ("NombreUsuario", "Usuario Calificador", 180),
                        ("Comentario", "Comentario de Retroalimentación", 320),
                        ("FechaRegistro", "Fecha Calificación", 140),
                        ("Puntuacion", "Calificación ⭐", 130)
                    );
                }
                else
                {
                    dgvTicketsOperativos.ConfigurarColumnas(
                        ("Folio", "Folio", 80),
                        ("Usuario", "Usuario Solicitante", 160),
                        ("Status", "Estatus", 100),
                        ("Prioridad", "Prioridad", 100),
                        ("FechaAlta", "Fecha Alta", 130),
                        ("Descripcion", "Descripción del Problema", 260),
                        ("Solucion", "Solución Aplicada", 260)
                    );
                }
            }
            dgvTicketsOperativos.AutoajustarAnchosMinimos();
        }

        private async void dgvTicketsOperativos_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            await dgvTicketsOperativos.ManejarDetalleTicketAsync(e.RowIndex, _formFactory, () => CargarDatosInicialesAsync());
        }

        private void ConfigurarFiltros()
        {
            filtroGenerico.InicializarFiltros(ConfiguracionFiltrosTickets.ObtenerCamposTecnico());
            filtroGenerico.FiltroCambiado += (s, e) => { if (!_estaCargando) AplicarFiltrosMemoria(); };
        }

        private void AplicarFiltrosMemoria()
        {
            var vals = filtroGenerico.ObtenerValoresFiltros();

            string? texto = null;
            if (vals.TryGetValue("Texto", out var txtVal) && txtVal != null)
            {
                var txtStr = txtVal.ToString()?.Trim().ToLowerInvariant();
                if (!string.IsNullOrWhiteSpace(txtStr)) texto = txtStr;
            }

            string? prioridad = null;
            if (vals.TryGetValue("Prioridad", out var priVal) && priVal != null)
            {
                var priStr = priVal.ToString();
                if (!string.IsNullOrEmpty(priStr) && priStr != "Todos") prioridad = priStr;
            }

            string? usuario = null;
            if (vals.TryGetValue("Usuario", out var usrVal) && usrVal != null)
            {
                var usrStr = usrVal.ToString()?.Trim().ToLowerInvariant();
                if (!string.IsNullOrWhiteSpace(usrStr)) usuario = usrStr;
            }

            DateTime fechaInicio = DateTime.MinValue;
            if (vals.TryGetValue("FechaInicio", out var fiVal) && fiVal is DateTime dtInicio)
            {
                fechaInicio = dtInicio.Date;
            }

            DateTime fechaFin = DateTime.MaxValue;
            if (vals.TryGetValue("FechaFin", out var ffVal) && ffVal is DateTime dtFin)
            {
                fechaFin = dtFin.Date.AddDays(1).AddTicks(-1);
            }

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

            _controladorPaginacion.ReiniciarAPrimeraPagina();
            MostrarPaginaActual();
        }

        private void btnLimpiarFiltros_Click(object? sender, EventArgs e)
        {
            _estaCargando = true;
            filtroGenerico.LimpiarFiltros(ConfiguracionFiltrosTickets.ObtenerValoresDefecto());
            _estaCargando = false;
            AplicarFiltrosMemoria();
        }

        private async void btnRecargar_Click(object? sender, EventArgs e)
        {
            await CargarDatosInicialesAsync();
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

        public void MostrarIndicadores(int asignados, int disponibles, int cerrados, double promedioCalificacion)
        {
            if (ucMisAsignados != null)
            {
                ucMisAsignados.Cantidad = asignados.ToString();
                ucMisAsignados.Titulo = "Mis Asignados";
                ucMisAsignados.ColorFondo = TemaVisual.TicketNuevo;
            }

            if (ucDisponibles != null)
            {
                ucDisponibles.Cantidad = disponibles.ToString();
                ucDisponibles.Titulo = "Disponibles";
                ucDisponibles.ColorFondo = TemaVisual.TicketEnProceso;
            }

            if (ucCerrados != null)
            {
                ucCerrados.Cantidad = cerrados.ToString();
                ucCerrados.Titulo = "Mis Cerrados";
                ucCerrados.ColorFondo = TemaVisual.TicketCerrado;
            }

            if (ucCalificacion != null)
            {
                ucCalificacion.Cantidad = promedioCalificacion > 0 ? $"⭐ {promedioCalificacion:F1}" : "⭐ N/A";
                ucCalificacion.Titulo = "Mi Calificación";
                ucCalificacion.ColorFondo = TemaVisual.TicketReabierto;
            }
        }
    }
}
