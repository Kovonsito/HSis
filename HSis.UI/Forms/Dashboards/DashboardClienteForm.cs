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
    public partial class DashboardClienteForm : Form, IDashboardClienteView
    {
        private readonly DashboardClientePresenter _presenter;
        private readonly NotificacionesPresenter _notificacionesPresenter;
        private readonly IContextoSesion _contextoSesion;
        private readonly IFabricaFormularios _formFactory;
        private readonly ISessionCacheService _sessionCache;

        private PaginacionControl PaginacionControl = null!;
        private ControladorPaginacionGrid _controladorPaginacion = null!;
        private List<TicketClienteDto> _todosLosTickets = [];

        private IndicadorControl ucMisCerrados = null!;

        private enum VistaCliente
        {
            Todos,
            Activos,
            Cerrados
        }
        private VistaCliente _vistaActual = VistaCliente.Todos;

        public DashboardClienteForm(
            DashboardClientePresenter presenter,
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

        private async void frmDashboardCliente_Load(object? sender, EventArgs e)
        {
            InicializarLayoutDashboard();
            _controladorPaginacion = new ControladorPaginacionGrid(PaginacionControl);
            _controladorPaginacion.Vincular(MostrarPaginaActual);

            SesionSistema.ConfigurarMenuSesion(this, _sessionCache);

            this.IntegrarNotificaciones(_notificacionesPresenter, _formFactory, _contextoSesion, CargarDatosDashboardAsync);

            await CargarDatosDashboardAsync();
        }

        private async Task CargarDatosDashboardAsync()
        {
            await _presenter.CargarTicketsClienteAsync(SesionSistema.IdUsuario);
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
            _controladorPaginacion.ReiniciarAPrimeraPagina();
            MostrarPaginaActual();
        }

        public void MostrarIndicadores(int activos, int cerrados)
        {
            if (ucMisActivos != null)
            {
                ucMisActivos.Cantidad = activos.ToString();
                ucMisActivos.Titulo = "Mis Tickets Activos";
                ucMisActivos.ColorFondo = Color.FromArgb(52, 152, 219); // Azul
            }

            if (ucMisCerrados != null)
            {
                ucMisCerrados.Cantidad = cerrados.ToString();
                ucMisCerrados.Titulo = "Mis Tickets Cerrados";
                ucMisCerrados.ColorFondo = Color.FromArgb(46, 204, 113); // Verde
            }
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

