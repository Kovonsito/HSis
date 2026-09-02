#nullable enable
using System.Runtime.Versioning;
using HSis.Logic.Constants;
using HSis.Logic.DTOs;
using HSis.Logic.Services;
using HSis.UI.Controls;
using HSis.UI.Factories;
using HSis.UI.Forms.Tickets;
using HSis.UI.Helpers;
using HSis.UI.Presenters;

namespace HSis.UI.Forms.Dashboards
{
    [SupportedOSPlatform("windows")]
    public partial class DashboardTecnicoForm : Form, IDashboardTecnicoView
    {
        private readonly DashboardTecnicoPresenter _presenter;
        private enum VistaDashboard
        {
            MisAsignados,
            Disponibles,
            Cerrados,
            Calificaciones
        }
        private VistaDashboard _vistaActual = VistaDashboard.MisAsignados;
        private readonly NotificacionesPresenter _notificacionesPresenter;
        private readonly IContextoSesion _contextoSesion;
        private PaginacionControl PaginacionControl = null!;
        private ControladorPaginacionGrid _controladorPaginacion = null!;
        private List<TicketOperativoDto> _todosLosTickets = [];
        private List<TicketOperativoDto> _ticketsFiltrados = [];
        private List<FeedbackTecnicoDto> _todosLosFeedbacks = [];
        private bool _estaCargando = false;

        private readonly IFabricaFormularios _formFactory;
        private readonly ISessionCacheService _sessionCache;

        public DashboardTecnicoForm(
            DashboardTecnicoPresenter presenter,
            NotificacionesPresenter notificacionesPresenter,
            IContextoSesion contextoSesion,
            IFabricaFormularios formFactory,
            ISessionCacheService sessionCache)
        {
            InitializeComponent();
            _presenter = presenter;
            _presenter.SetView(this);
            _notificacionesPresenter = notificacionesPresenter;
            _contextoSesion = contextoSesion;
            _formFactory = formFactory;
            _sessionCache = sessionCache;
        }

        private async void frmDashboardTecnico_Load(object? sender, EventArgs e)
        {
            dgvTicketsOperativos.AplicarTemaModerno();
            InicializarLayoutDashboard();
            _controladorPaginacion = new ControladorPaginacionGrid(PaginacionControl);
            _controladorPaginacion.Vincular(MostrarPaginaActual);

            ConfigurarSidebar();
            ConfigurarFiltros();

            this.IntegrarNotificacionesModerno(topBarTecnico, _notificacionesPresenter, _formFactory, _contextoSesion, CargarDatosInicialesAsync);

            // Cargamos indicadores y grid en paralelo
            await CargarDatosInicialesAsync();
        }

        private void ConfigurarSidebar()
        {
            sidebarTecnico.ConfigurarSesion(_sessionCache);
            sidebarTecnico.ConfigurarItems(new[]
            {
                new ItemSidebar { Clave = "asignados", Titulo = "Mis Asignados", Icono = FontAwesome.Sharp.IconChar.ClipboardCheck },
                new ItemSidebar { Clave = "disponibles", Titulo = "Disponibles", Icono = FontAwesome.Sharp.IconChar.Inbox },
                new ItemSidebar { Clave = "cerrados", Titulo = "Mis Cerrados", Icono = FontAwesome.Sharp.IconChar.CheckCircle },
                new ItemSidebar { Clave = "calificaciones", Titulo = "Calificaciones", Icono = FontAwesome.Sharp.IconChar.Star },
                new ItemSidebar { Clave = "kardex", Titulo = "Almacén / Kardex", Icono = FontAwesome.Sharp.IconChar.BoxesStacked }
            }, "asignados");

            sidebarTecnico.ItemSeleccionado += async (s, clave) =>
            {
                if (clave == "kardex")
                {
                    var frmK = _formFactory.Crear<Forms.Otros.KardexForm>();
                    frmK.ShowDialog();
                    sidebarTecnico.SeleccionarItem(_vistaActual switch
                    {
                        VistaDashboard.MisAsignados => "asignados",
                        VistaDashboard.Disponibles => "disponibles",
                        VistaDashboard.Cerrados => "cerrados",
                        VistaDashboard.Calificaciones => "calificaciones",
                        _ => "asignados"
                    });
                    return;
                }

                switch (clave)
                {
                    case "asignados":
                        _vistaActual = VistaDashboard.MisAsignados;
                        topBarTecnico.Titulo = "Mis Tickets Asignados";
                        topBarTecnico.Subtitulo = "Tickets que tienes actualmente en proceso o abiertos";
                        break;
                    case "disponibles":
                        _vistaActual = VistaDashboard.Disponibles;
                        topBarTecnico.Titulo = "Tickets Disponibles en Cola";
                        topBarTecnico.Subtitulo = "Tickets sin asignar listos para ser atendidos";
                        break;
                    case "cerrados":
                        _vistaActual = VistaDashboard.Cerrados;
                        topBarTecnico.Titulo = "Historial de Tickets Cerrados";
                        topBarTecnico.Subtitulo = "Tickets resueltos y finalizados exitosamente";
                        break;
                    case "calificaciones":
                        _vistaActual = VistaDashboard.Calificaciones;
                        topBarTecnico.Titulo = "Mis Calificaciones";
                        topBarTecnico.Subtitulo = "Evaluaciones y comentarios de los clientes";
                        break;
                }

                await CargarTicketsSegunVistaAsync();
            };
        }

        private async Task CargarDatosInicialesAsync()
        {
            await Task.WhenAll(_presenter.CargarIndicadoresAsync(SesionSistema.IdUsuario), CargarTicketsSegunVistaAsync());
        }

        private async Task CargarTicketsSegunVistaAsync()
        {
            var task = _vistaActual switch
            {
                VistaDashboard.MisAsignados => _presenter.CargarTicketsAsignadosAsync(SesionSistema.IdUsuario),
                VistaDashboard.Disponibles => _presenter.CargarTicketsDisponiblesAsync(),
                VistaDashboard.Cerrados => _presenter.CargarTicketsCerradosAsync(SesionSistema.IdUsuario),
                VistaDashboard.Calificaciones => _presenter.CargarFeedbacksAsync(SesionSistema.IdUsuario),
                _ => Task.CompletedTask
            };
            await task;
        }

        private async void UcMisAsignados_Click(object? sender, EventArgs e)
        {
            _vistaActual = VistaDashboard.MisAsignados;
            sidebarTecnico.SeleccionarItem("asignados");
            topBarTecnico.Titulo = "Mis Tickets Asignados";
            await CargarTicketsSegunVistaAsync();
        }

        private async void UcDisponibles_Click(object? sender, EventArgs e)
        {
            _vistaActual = VistaDashboard.Disponibles;
            sidebarTecnico.SeleccionarItem("disponibles");
            topBarTecnico.Titulo = "Tickets Disponibles en Cola";
            await CargarTicketsSegunVistaAsync();
        }

        private async void UcCerrados_Click(object? sender, EventArgs e)
        {
            _vistaActual = VistaDashboard.Cerrados;
            sidebarTecnico.SeleccionarItem("cerrados");
            topBarTecnico.Titulo = "Historial de Tickets Cerrados";
            await CargarTicketsSegunVistaAsync();
        }

        private async void UcCalificacion_Click(object? sender, EventArgs e)
        {
            _vistaActual = VistaDashboard.Calificaciones;
            sidebarTecnico.SeleccionarItem("calificaciones");
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
                dgvTicketsOperativos.DataSource = new ListaVinculableOrdenable<TicketOperativoDto>(pageTickets);
                _controladorPaginacion.Actualizar(_ticketsFiltrados.Count);
            }
            PersonalizarColumnas();
        }

        private void PersonalizarColumnas()
        {
            if (dgvTicketsOperativos.Columns.Count > 0)
            {
                dgvTicketsOperativos.ConfigurarOcultarColumnas("IdTicket");

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
                    bool matchTexto = (t.Folio?.ToLowerInvariant().Contains(texto) ?? false) ||
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

        #region Implementación IDashboardTecnicoView (MVP)

        public void MostrarTickets(List<TicketOperativoDto> tickets)
        {
            _todosLosTickets = tickets;
            AplicarFiltrosMemoria();
        }

        public void MostrarFeedbacks(List<FeedbackTecnicoDto> feedbacks)
        {
            _todosLosFeedbacks = feedbacks;
            AplicarFiltrosMemoria();
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

        public void MostrarCargando(bool cargando)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => MostrarCargando(cargando)));
                return;
            }
            Cursor = cargando ? Cursors.WaitCursor : Cursors.Default;
        }

        public void MostrarError(string mensaje)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => MostrarError(mensaje)));
                return;
            }
            MessageBox.Show(mensaje, "Error en Dashboard Técnico", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion
    }
}

