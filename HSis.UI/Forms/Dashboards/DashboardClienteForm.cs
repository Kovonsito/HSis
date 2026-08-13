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
        private ControladorPaginacionGrid _controladorPaginacion = null!;
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
            _controladorPaginacion = new ControladorPaginacionGrid(PaginacionControl);
            _controladorPaginacion.Vincular(MostrarPaginaActual);

            SesionSistema.ConfigurarMenuSesion(this, _sessionCache);

            this.IntegrarNotificaciones(_notificacionesPresenter, _formFactory, _contextoSesion, CargarDatosDashboardAsync);

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
                Descripcion = FormatoVisualHelper.TruncarTexto(t.Descripcion, 50),
                Feedback = t.Estatus == ConstantesEstatus.CERRADO
                    ? (t.Calificacion.HasValue ? $"Enviada ({FormatoVisualHelper.FormatearEstrellas(t.Calificacion.Value)})" : "Pendiente")
                    : "N/A"
            });

            _controladorPaginacion.ReiniciarAPrimeraPagina();
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

            // 2. Segmentar la página actual
            var pageTickets = ticketsFiltrados
                .Skip((_controladorPaginacion.PaginaActual - 1) * _controladorPaginacion.TamanoPagina)
                .Take(_controladorPaginacion.TamanoPagina)
                .ToList();

            dgvMisTickets.DataSource = new ListaVinculableOrdenable<TicketClienteDto>(pageTickets);
            PersonalizarColumnas();
            _controladorPaginacion.Actualizar(ticketsFiltrados.Count);
        }

        private void PersonalizarColumnas()
        {
            if (dgvMisTickets.Columns.Count > 0)
            {
                dgvMisTickets.ConfigurarOcultarColumnas("IdTicket");

                var colFolio = dgvMisTickets.Columns["Folio"];
                if (colFolio != null)
                {
                    colFolio.HeaderText = "Folio";
                    colFolio.Width = 80;
                }

                var colFecha = dgvMisTickets.Columns["FechaAlta"];
                if (colFecha != null)
                {
                    colFecha.HeaderText = "Fecha de Creación";
                    colFecha.Width = 140;
                }

                var colStatus = dgvMisTickets.Columns["Status"];
                if (colStatus != null)
                {
                    colStatus.HeaderText = "Estatus";
                    colStatus.Width = 110;
                }

                var colTecnico = dgvMisTickets.Columns["TecnicoAsignado"];
                if (colTecnico != null)
                {
                    colTecnico.HeaderText = "Técnico Asignado";
                    colTecnico.Width = 160;
                }

                var colDesc = dgvMisTickets.Columns["Descripcion"];
                if (colDesc != null)
                {
                    colDesc.HeaderText = "Descripción";
                    colDesc.Width = 250;
                }

                var colFeedback = dgvMisTickets.Columns["Feedback"];
                if (colFeedback != null)
                {
                    colFeedback.HeaderText = "Calificación / Feedback";
                    colFeedback.Width = 160;
                }

                dgvMisTickets.AutoajustarAnchosMinimos();
            }
        }

        private void ActualizarIndicador(List<TicketDto> tickets)
        {
            var activos = tickets.Count(t => t.Estatus != ConstantesEstatus.CERRADO);
            var cerrados = tickets.Count(t => t.Estatus == ConstantesEstatus.CERRADO);

            ucMisActivos.Cantidad = activos.ToString();
            ucMisActivos.Titulo = "Mis Tickets Activos";
            ucMisActivos.ColorFondo = Color.FromArgb(52, 152, 219); // Azul

            if (ucMisCerrados != null)
            {
                ucMisCerrados.Cantidad = cerrados.ToString();
                ucMisCerrados.Titulo = "Mis Tickets Cerrados";
                ucMisCerrados.ColorFondo = Color.FromArgb(46, 204, 113); // Verde
            }
        }

        private void UcMisActivos_Click(object? sender, EventArgs e)
        {
            _vistaActual = _vistaActual == VistaCliente.Activos ? VistaCliente.Todos : VistaCliente.Activos;
            _controladorPaginacion.ReiniciarAPrimeraPagina();
            MostrarPaginaActual();
        }

        private void UcMisCerrados_Click(object? sender, EventArgs e)
        {
            _vistaActual = _vistaActual == VistaCliente.Cerrados ? VistaCliente.Todos : VistaCliente.Cerrados;
            _controladorPaginacion.ReiniciarAPrimeraPagina();
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

