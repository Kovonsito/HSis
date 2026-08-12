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
    public partial class DashboardClienteForm : Form, IDashboardClienteView
    {
        private readonly ITicketService _ticketService;
        private readonly DashboardClientePresenter? _presenter;

        private PaginacionControl PaginacionControl = null!;
        private List<TicketClienteDto> _todosLosTickets = [];

        private IndicadorControl ucMisCerrados = null!;
        private readonly IFabricaFormularios _formFactory;

        private enum VistaCliente
        {
            Todos,
            Activos,
            Cerrados
        }
        private VistaCliente _vistaActual = VistaCliente.Todos;

        private readonly NotificacionesPresenter _notificacionesPresenter;
        private readonly IContextoSesion _contextoSesion;
        private readonly ISessionCacheService _sessionCache;

        public DashboardClienteForm(
            ITicketService ticketService,
            NotificacionesPresenter notificacionesPresenter,
            IContextoSesion contextoSesion,
            IFabricaFormularios formFactory,
            ISessionCacheService sessionCache,
            DashboardClientePresenter? presenter = null)
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

        private async void frmDashboardCliente_Load(object? sender, EventArgs e)
        {
            InicializarLayoutDashboard();
            SesionSistema.ConfigurarMenuSesion(this, _sessionCache);

            var notifControl = new NotificacionesControl();
            notifControl.Configurar(_notificacionesPresenter, _formFactory, _contextoSesion, CargarDatosDashboardAsync);
            this.Controls.Add(notifControl);
            notifControl.BringToFront();

            // Cargamos la información una sola vez para evitar múltiples llamadas a la BD
            await CargarDatosDashboardAsync();
        }

        private async Task CargarDatosDashboardAsync()
        {
            try
            {
                var tickets = await _ticketService.ObtenerTicketsPorUsuarioAsync(SesionSistema.IdUsuario);

                // Actualizar Indicador
                ActualizarIndicador(tickets);

                // Actualizar Grid
                ActualizarGridTickets(tickets);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el dashboard: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarGridTickets(List<TicketDto> tickets)
        {
            _todosLosTickets = tickets.ConvertAll(t => new TicketClienteDto
            {
                IdTicket = t.IdTicket,
                FechaAlta = t.FechaAlta,
                Status = t.Estatus,
                TecnicoAsignado = t.NombreTecnico ?? "Sin asignar",
                Descripcion = !string.IsNullOrEmpty(t.Descripcion) && t.Descripcion.Length > 50 ? string.Concat(t.Descripcion.AsSpan(0, 50), "...") : (t.Descripcion ?? ""),
                Feedback = t.Estatus == ConstantesEstatus.CERRADO
                    ? (t.Calificacion.HasValue ? $"Enviada ({new string('★', t.Calificacion.Value)}{new string('☆', 5 - t.Calificacion.Value)})" : "Pendiente")
                    : "N/A"
            });

            PaginacionControl.PaginaActual = 1;
            MostrarPaginaActual();
        }

        private void MostrarPaginaActual()
        {
            // 1. Filtrar en memoria según la vista actual
            var ticketsFiltrados = _todosLosTickets;
            if (_vistaActual == VistaCliente.Activos)
            {
                ticketsFiltrados = [.. _todosLosTickets.Where(t => t.Status != ConstantesEstatus.CERRADO)];
            }
            else if (_vistaActual == VistaCliente.Cerrados)
            {
                ticketsFiltrados = [.. _todosLosTickets.Where(t => t.Status == ConstantesEstatus.CERRADO)];
            }

            PaginacionControl.TotalRegistros = ticketsFiltrados.Count;

            // 2. Segmentar la página actual
            var pageTickets = ticketsFiltrados
                .Skip((PaginacionControl.PaginaActual - 1) * PaginacionControl.TamanoPagina)
                .Take(PaginacionControl.TamanoPagina)
                .ToList();

            dgvMisTickets.DataSource = new ListaVinculableOrdenable<TicketClienteDto>(pageTickets);
            PersonalizarColumnas();
            PaginacionControl.ActualizarInterfaz();
        }

        private void PersonalizarColumnas()
        {
            if (dgvMisTickets.Columns.Count > 0)
            {
                dgvMisTickets.ConfigurarOcultarColumnas("IdTicket");

                var colFolio = dgvMisTickets.Columns["Folio"];
                if (colFolio != null) colFolio.HeaderText = "Folio";

                var colFechaAlta = dgvMisTickets.Columns["FechaAlta"];
                if (colFechaAlta != null)
                {
                    colFechaAlta.HeaderText = "Fecha de Alta";
                    colFechaAlta.Width = 100;
                }

                var colStatus = dgvMisTickets.Columns["Status"];
                if (colStatus != null)
                {
                    colStatus.HeaderText = "Estatus";
                    colStatus.Width = 100;
                }

                var colTecnicoAsignado = dgvMisTickets.Columns["TecnicoAsignado"];
                if (colTecnicoAsignado != null)
                {
                    colTecnicoAsignado.HeaderText = "Técnico Asignado";
                    colTecnicoAsignado.Width = 120;
                }

                var colDescripcion = dgvMisTickets.Columns["Descripcion"];
                if (colDescripcion != null) colDescripcion.HeaderText = "Descripción";

                var colFeedback = dgvMisTickets.Columns["Feedback"];
                if (colFeedback != null)
                {
                    colFeedback.HeaderText = "Retroalimentación";
                    colFeedback.Width = 140;
                }

                dgvMisTickets.AutoajustarAnchosMinimos();
            }
        }

        private void ActualizarIndicador(List<TicketDto> tickets)
        {
            var activos = tickets.FindAll(t => t.Estatus != ConstantesEstatus.CERRADO);
            var cerrados = tickets.FindAll(t => t.Estatus == ConstantesEstatus.CERRADO);

            ucMisActivos.Cantidad = activos.Count.ToString();
            ucMisActivos.Titulo = "Mis Tickets Activos";
            ucMisActivos.ColorFondo = Color.FromArgb(41, 128, 185); // Azul

            if (ucMisCerrados != null)
            {
                ucMisCerrados.Cantidad = cerrados.Count.ToString();
                ucMisCerrados.Titulo = "Mis Tickets Cerrados";
                ucMisCerrados.ColorFondo = Color.FromArgb(46, 204, 113); // Verde
            }
        }

        private void UcMisActivos_Click(object? sender, EventArgs e)
        {
            _vistaActual = _vistaActual == VistaCliente.Activos ? VistaCliente.Todos : VistaCliente.Activos;
            PaginacionControl.PaginaActual = 1;
            MostrarPaginaActual();
        }

        private void UcMisCerrados_Click(object? sender, EventArgs e)
        {
            _vistaActual = _vistaActual == VistaCliente.Cerrados ? VistaCliente.Todos : VistaCliente.Cerrados;
            PaginacionControl.PaginaActual = 1;
            MostrarPaginaActual();
        }

        private void btnNuevoReporte_Click(object? sender, EventArgs e)
        {
            using var frmNuevo = _formFactory.Crear<NuevoTicketForm>();
            if (frmNuevo.ShowDialog() == DialogResult.OK)
            {
                _ = CargarDatosDashboardAsync();
            }
        }

        private async void dgvMisTickets_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvMisTickets.Rows[e.RowIndex];
                if (int.TryParse(row.Cells["IdTicket"].Value?.ToString(), out int idTicket))
                {
                    using var frmDetalle = _formFactory.CrearDetalleCliente(idTicket);
                    frmDetalle.ShowDialog();
                    await CargarDatosDashboardAsync();
                }
            }
        }



        #region Implementación IDashboardClienteView (MVP)

        public void MostrarTickets(List<TicketClienteDto> tickets)
        {
            _todosLosTickets = tickets;
            MostrarPaginaActual();
        }

        public void MostrarIndicadores(int activos, int cerrados)
        {
            if (ucMisActivos != null) ucMisActivos.Cantidad = activos.ToString();
            if (ucMisCerrados != null) ucMisCerrados.Cantidad = cerrados.ToString();
        }

        public void MostrarCargando(bool cargando)
        {
            Cursor = cargando ? Cursors.WaitCursor : Cursors.Default;
        }

        public void MostrarError(string mensaje)
        {
            MessageBox.Show(mensaje, "Error en Dashboard de Cliente", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion
    }
}

