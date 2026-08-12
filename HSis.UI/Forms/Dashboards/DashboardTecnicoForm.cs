#nullable enable
using System.Runtime.Versioning;
using HSis.Logic.Constants;
using HSis.Logic.DTOs;
using HSis.Logic.Services;
using HSis.UI.Controls;
using HSis.UI.Factories;
using HSis.UI.Forms.Tickets;
using HSis.UI.Helpers;
using HSis.UI.Services;

using HSis.UI.Presenters;

namespace HSis.UI.Forms.Dashboards
{
    [SupportedOSPlatform("windows")]
    public partial class DashboardTecnicoForm : Form, IDashboardTecnicoView
    {
        private readonly ITicketService _ticketService;
        private readonly DashboardTecnicoPresenter? _presenter;
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
        private List<TicketOperativoDto> _todosLosTickets = [];
        private List<TicketOperativoDto> _ticketsFiltrados = [];
        private List<FeedbackTecnicoDto> _todosLosFeedbacks = [];
        private bool _estaCargando = false;

        private readonly IFabricaFormularios _formFactory;
        private readonly ISessionCacheService _sessionCache;

        public DashboardTecnicoForm(
            ITicketService ticketService,
            NotificacionesPresenter notificacionesPresenter,
            IContextoSesion contextoSesion,
            IFabricaFormularios formFactory,
            ISessionCacheService sessionCache,
            DashboardTecnicoPresenter? presenter = null)
        {
            InitializeComponent();
            _ticketService = ticketService;
            _notificacionesPresenter = notificacionesPresenter;
            _contextoSesion = contextoSesion;
            _formFactory = formFactory;
            _sessionCache = sessionCache;
            _presenter = presenter;
            _presenter?.SetView(this);
        }

        private async void frmDashboardTecnico_Load(object? sender, EventArgs e)
        {
            InicializarLayoutDashboard();
            ConfigurarFiltros();
            SesionSistema.ConfigurarMenuSesion(this, _sessionCache);

            var notifControl = new NotificacionesControl();
            notifControl.Configurar(_notificacionesPresenter, _formFactory, _contextoSesion, CargarDatosInicialesAsync);
            this.Controls.Add(notifControl);
            notifControl.BringToFront();

            // Cargamos indicadores y grid en paralelo
            await CargarDatosInicialesAsync();
        }

        private async Task CargarDatosInicialesAsync()
        {
            // Ejecutamos las dos cargas principales en paralelo
            await Task.WhenAll(CargarIndicadoresAsync(), CargarTicketsSegunVistaAsync());
        }

        private async Task CargarTicketsSegunVistaAsync()
        {
            var task = _vistaActual switch
            {
                VistaDashboard.MisAsignados => CargarTicketsMisAsignadosAsync(),
                VistaDashboard.Disponibles => CargarTicketsDisponiblesAsync(),
                VistaDashboard.Cerrados => CargarTicketsCerradosAsync(),
                VistaDashboard.Calificaciones => CargarCalificacionesAsync(),
                _ => Task.CompletedTask
            };
            await task;
        }

        private async Task CargarIndicadoresAsync()
        {
            try
            {
                // Consultas en paralelo para los indicadores
                var taskMisAsignados = _ticketService.ObtenerTicketsAsignadosATecnicoAsync(SesionSistema.IdUsuario);
                var taskDisponibles = _ticketService.ObtenerTicketsDisponiblesAsync();
                var taskCerrados = _ticketService.ObtenerTicketsCerradosPorTecnicoAsync(SesionSistema.IdUsuario);
                var taskCalificacion = _ticketService.ObtenerPromedioCalificacionTecnicoAsync(SesionSistema.IdUsuario);

                await Task.WhenAll(taskMisAsignados, taskDisponibles, taskCerrados, taskCalificacion);

                var misAsignados = taskMisAsignados.Result;
                var disponibles = taskDisponibles.Result;
                var cerrados = taskCerrados.Result;
                var promedio = taskCalificacion.Result;

                // Actualizar controles de UI siempre desde el hilo de UI
                this.Invoke(() =>
                {
                    ucMisAsignados.Cantidad = misAsignados.Count.ToString();
                    ucMisAsignados.Titulo = "Mis Asignados";
                    ucMisAsignados.ColorFondo = Color.FromArgb(41, 128, 185);

                    ucDisponibles.Cantidad = disponibles.Count.ToString();
                    ucDisponibles.Titulo = "Disponibles";
                    ucDisponibles.ColorFondo = Color.FromArgb(241, 196, 15);

                    ucCerrados.Cantidad = cerrados.Count.ToString();
                    ucCerrados.Titulo = "Mis Cerrados";
                    ucCerrados.ColorFondo = Color.FromArgb(46, 204, 113);

                    ucCalificacion.Cantidad = promedio > 0 ? $"⭐ {promedio:F1}" : "⭐ N/A";
                    ucCalificacion.Titulo = "Mi Calificación";
                    ucCalificacion.ColorFondo = Color.FromArgb(155, 89, 182);
                });
            }
            catch (Exception ex)
            {
                this.Invoke(() => MessageBox.Show($"Error al cargar indicadores: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error));
            }
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

        private async Task CargarCalificacionesAsync()
        {
            try
            {
                var feedback = await _ticketService.ObtenerFeedbackTecnicoAsync(SesionSistema.IdUsuario);
                _todosLosFeedbacks = [.. feedback.Select(t => new FeedbackTecnicoDto
                {
                    IdTicket = t.IdTicket,
                    Calificacion = new string('★', t.Calificacion ?? 0) + new string('☆', 5 - (t.Calificacion ?? 0)),
                    Comentario = t.ComentarioEvaluacion ?? "Sin comentarios",
                    Fecha = t.FechaEvaluacion
                })];

                PaginacionControl.PaginaActual = 1;
                MostrarPaginaActual();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar calificaciones: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task CargarTicketsMisAsignadosAsync()
        {
            try
            {
                var tickets = await _ticketService.ObtenerTicketsAsignadosATecnicoAsync(SesionSistema.IdUsuario);
                CargarGridTickets(tickets);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar tickets: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task CargarTicketsDisponiblesAsync()
        {
            try
            {
                var tickets = await _ticketService.ObtenerTicketsDisponiblesAsync();
                CargarGridTickets(tickets);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar tickets disponibles: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task CargarTicketsCerradosAsync()
        {
            try
            {
                var tickets = await _ticketService.ObtenerTicketsCerradosPorTecnicoAsync(SesionSistema.IdUsuario);
                CargarGridTickets(tickets);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar tickets cerrados: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarGridTickets(List<TicketDto> tickets)
        {
            _todosLosTickets = [.. tickets.Select(t => new TicketOperativoDto
            {
                IdTicket = t.IdTicket,
                FechaAlta = t.FechaAlta,
                Status = t.Estatus ?? "N/A",
                Usuario = t.NombreUsuario ?? "N/A",
                Descripcion = !string.IsNullOrEmpty(t.Descripcion) && t.Descripcion.Length > 50 ? t.Descripcion[..50] + "..." : (t.Descripcion ?? ""),
                Prioridad = t.Prioridad
            })];

            AplicarFiltrosMemoria();
        }

        private void MostrarPaginaActual()
        {
            if (_vistaActual == VistaDashboard.Calificaciones)
            {
                PaginacionControl.TotalRegistros = _todosLosFeedbacks.Count;
                var pageFeedbacks = _todosLosFeedbacks
                    .Skip((PaginacionControl.PaginaActual - 1) * PaginacionControl.TamanoPagina)
                    .Take(PaginacionControl.TamanoPagina)
                    .ToList();
                dgvTicketsOperativos.DataSource = new ListaVinculableOrdenable<FeedbackTecnicoDto>(pageFeedbacks);
            }
            else
            {
                PaginacionControl.TotalRegistros = _ticketsFiltrados.Count;
                var pageTickets = _ticketsFiltrados
                    .Skip((PaginacionControl.PaginaActual - 1) * PaginacionControl.TamanoPagina)
                    .Take(PaginacionControl.TamanoPagina)
                    .ToList();
                dgvTicketsOperativos.DataSource = new ListaVinculableOrdenable<TicketOperativoDto>(pageTickets);
            }
            PersonalizarColumnas();
            PaginacionControl.ActualizarInterfaz();
        }

        private void PersonalizarColumnas()
        {
            if (dgvTicketsOperativos.Columns.Count > 0)
            {
                dgvTicketsOperativos.ConfigurarOcultarColumnas("IdTicket");

                if (dgvTicketsOperativos.DataSource is ListaVinculableOrdenable<FeedbackTecnicoDto>)
                {
                    if (dgvTicketsOperativos.Columns["Folio"] is DataGridViewColumn colFolio)
                    {
                        colFolio.HeaderText = "Folio";
                        colFolio.Width = 80;
                    }
                    if (dgvTicketsOperativos.Columns["Calificacion"] is DataGridViewColumn colCal)
                    {
                        colCal.HeaderText = "Calificación";
                        colCal.Width = 100;
                    }
                    if (dgvTicketsOperativos.Columns["Comentario"] is DataGridViewColumn colCom)
                    {
                        colCom.HeaderText = "Comentario del Cliente";
                    }
                    if (dgvTicketsOperativos.Columns["Fecha"] is DataGridViewColumn colFec)
                    {
                        colFec.HeaderText = "Fecha de Calificación";
                        colFec.Width = 130;
                    }
                }
                else
                {
                    if (dgvTicketsOperativos.Columns["FechaAlta"] is DataGridViewColumn colFechaAlta)
                    {
                        colFechaAlta.HeaderText = "Fecha de Alta";
                        colFechaAlta.Width = 100;
                    }
                    if (dgvTicketsOperativos.Columns["Status"] is DataGridViewColumn colStatus)
                    {
                        colStatus.HeaderText = "Estatus";
                        colStatus.Width = 100;
                    }
                    if (dgvTicketsOperativos.Columns["Usuario"] is DataGridViewColumn colUsuario)
                    {
                        colUsuario.HeaderText = "Usuario Reportó";
                        colUsuario.Width = 120;
                    }
                    if (dgvTicketsOperativos.Columns["Descripcion"] is DataGridViewColumn colDesc)
                    {
                        colDesc.HeaderText = "Descripción";
                    }
                    if (dgvTicketsOperativos.Columns["Prioridad"] is DataGridViewColumn colPrioridad)
                    {
                        colPrioridad.HeaderText = "Prioridad";
                        colPrioridad.Width = 80;
                    }
                }

                dgvTicketsOperativos.AutoajustarAnchosMinimos();
            }
        }

        private async void dgvTicketsOperativos_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            await dgvTicketsOperativos.ManejarDetalleTicketAsync(e.RowIndex, _formFactory, () => Task.WhenAll(CargarIndicadoresAsync(), CargarTicketsSegunVistaAsync()));
        }



        private void ConfigurarFiltros()
        {
            var campos = new List<HSis.UI.Controls.FiltroCampo>
            {
                new() { NombrePropiedad = "Estatus", Etiqueta = "Estatus:", Tipo = HSis.UI.Controls.TipoFiltroControl.ComboSeleccion, ValoresCombo = ["Todos", "Abierto", "En Proceso", "Cerrado", "Reabierto"], Ancho = 130 },
                new() { NombrePropiedad = "Prioridad", Etiqueta = "Prioridad:", Tipo = HSis.UI.Controls.TipoFiltroControl.ComboSeleccion, ValoresCombo = ["Todos", ConstantesPrioridad.ALTA, ConstantesPrioridad.MEDIA, ConstantesPrioridad.BAJA, ConstantesPrioridad.URGENTE], Ancho = 130 },
                new() { NombrePropiedad = "Usuario", Etiqueta = "Usuario Emisor:", Tipo = HSis.UI.Controls.TipoFiltroControl.Texto, Ancho = 160 },
                new() { NombrePropiedad = "FechaInicio", Etiqueta = "Desde:", Tipo = HSis.UI.Controls.TipoFiltroControl.Fecha, Ancho = 130, ValorDefecto = DateTime.Today.AddDays(-30) },
                new() { NombrePropiedad = "FechaFin", Etiqueta = "Hasta:", Tipo = HSis.UI.Controls.TipoFiltroControl.Fecha, Ancho = 130, ValorDefecto = DateTime.Today.AddDays(1).AddTicks(-1) }
            };

            filtroGenerico.InicializarFiltros(campos);
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

            PaginacionControl.PaginaActual = 1;
            MostrarPaginaActual();
        }

        private void btnLimpiarFiltros_Click(object? sender, EventArgs e)
        {
            _estaCargando = true;
            var valoresDefecto = new Dictionary<string, object?>
            {
                { "FechaInicio", DateTime.Today.AddDays(-30) },
                { "FechaFin", DateTime.Today.AddDays(1).AddTicks(-1) }
            };
            filtroGenerico.LimpiarFiltros(valoresDefecto);
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
            if (ucMisAsignados != null) ucMisAsignados.Cantidad = asignados.ToString();
            if (ucDisponibles != null) ucDisponibles.Cantidad = disponibles.ToString();
            if (ucCerrados != null) ucCerrados.Cantidad = cerrados.ToString();
            if (ucCalificacion != null) ucCalificacion.Cantidad = $"{promedioCalificacion:F1} ⭐";
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

