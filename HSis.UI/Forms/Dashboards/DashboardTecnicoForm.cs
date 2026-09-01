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
            InicializarLayoutDashboard();
            _controladorPaginacion = new ControladorPaginacionGrid(PaginacionControl);
            _controladorPaginacion.Vincular(MostrarPaginaActual);

            ConfigurarFiltros();
            SesionSistema.ConfigurarMenuSesion(this, _sessionCache);

            this.IntegrarNotificaciones(_notificacionesPresenter, _formFactory, _contextoSesion, CargarDatosInicialesAsync);

            // Cargamos indicadores y grid en paralelo
            await CargarDatosInicialesAsync();
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
            await CargarTicketsSegunVistaAsync();
        }

        private async void UcDisponibles_Click(object? sender, EventArgs e)
        {
            _vistaActual = VistaDashboard.Disponibles;
            await CargarTicketsSegunVistaAsync();
        }

        private async void UcCerrados_Click(object? sender, EventArgs e)
        {
            _vistaActual = VistaDashboard.Cerrados;
            await CargarTicketsSegunVistaAsync();
        }

        private async void UcCalificacion_Click(object? sender, EventArgs e)
        {
            _vistaActual = VistaDashboard.Calificaciones;
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

            string? estatus = null;
            if (vals.TryGetValue("Estatus", out var estVal) && estVal != null)
            {
                var estStr = estVal.ToString();
                if (estStr != "Todos") estatus = estStr;
            }

            string? prioridad = null;
            if (vals.TryGetValue("Prioridad", out var priVal) && priVal != null)
            {
                var priStr = priVal.ToString();
                if (priStr != "Todos") prioridad = priStr;
            }

            string? usuario = null;
            if (vals.TryGetValue("Usuario", out var usrVal) && usrVal != null)
            {
                var usrStr = usrVal.ToString();
                if (!string.IsNullOrWhiteSpace(usrStr)) usuario = usrStr.ToLower();
            }

            DateTime fechaInicio = DateTime.MinValue;
            if (vals.TryGetValue("FechaInicio", out var fiVal) && fiVal is DateTime)
            {
                fechaInicio = ((DateTime)fiVal).Date;
            }

            DateTime fechaFin = DateTime.MaxValue;
            if (vals.TryGetValue("FechaFin", out var ffVal) && ffVal is DateTime)
            {
                fechaFin = ((DateTime)ffVal).Date.AddDays(1).AddTicks(-1);
            }

            _ticketsFiltrados = _todosLosTickets.Where(t =>
            {
                if (estatus != null && !string.Equals(t.Status, estatus, StringComparison.OrdinalIgnoreCase)) return false;
                if (prioridad != null && !string.Equals(t.Prioridad, prioridad, StringComparison.OrdinalIgnoreCase)) return false;
                if (usuario != null && !(t.Usuario?.ToLower().Contains(usuario) ?? false)) return false;
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
                ucMisAsignados.ColorFondo = Color.FromArgb(41, 128, 185);
            }

            if (ucDisponibles != null)
            {
                ucDisponibles.Cantidad = disponibles.ToString();
                ucDisponibles.Titulo = "Disponibles";
                ucDisponibles.ColorFondo = Color.FromArgb(241, 196, 15);
            }

            if (ucCerrados != null)
            {
                ucCerrados.Cantidad = cerrados.ToString();
                ucCerrados.Titulo = "Mis Cerrados";
                ucCerrados.ColorFondo = Color.FromArgb(46, 204, 113);
            }

            if (ucCalificacion != null)
            {
                ucCalificacion.Cantidad = promedioCalificacion > 0 ? $"⭐ {promedioCalificacion:F1}" : "⭐ N/A";
                ucCalificacion.Titulo = "Mi Calificación";
                ucCalificacion.ColorFondo = Color.FromArgb(155, 89, 182);
            }
        }

        public void MostrarCargando(bool cargando)
        {
            Cursor = cargando ? Cursors.WaitCursor : Cursors.Default;
        }

        public void MostrarError(string mensaje)
        {
            MessageBox.Show(mensaje, "Error en Dashboard Técnico", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion
    }
}

