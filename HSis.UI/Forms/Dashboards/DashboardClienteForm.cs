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
            dgvMisTickets.AplicarTemaModerno();
            InicializarLayoutDashboard();
            _controladorPaginacion = new ControladorPaginacionGrid(PaginacionControl);
            _controladorPaginacion.Vincular(MostrarPaginaActual);

            ConfigurarSidebar();
            ConfigurarFiltros();

            this.IntegrarNotificacionesModerno(topBarCliente, _notificacionesPresenter, _formFactory, _contextoSesion, CargarDatosDashboardAsync);

            await CargarDatosDashboardAsync();
        }

        private void ConfigurarFiltros()
        {
            filtroCliente.InicializarFiltros(ConfiguracionFiltrosTickets.ObtenerCamposCliente());
            filtroCliente.FiltroCambiado += (s, e) =>
            {
                _controladorPaginacion.ReiniciarAPrimeraPagina();
                MostrarPaginaActual();
            };
        }

        private void ConfigurarSidebar()
        {
            sidebarCliente.ConfigurarSesion(_sessionCache);
            sidebarCliente.ConfigurarItems(new[]
            {
                new ItemSidebar { Clave = "activos", Titulo = "Mis Activos", Icono = FontAwesome.Sharp.IconChar.Ticket },
                new ItemSidebar { Clave = "cerrados", Titulo = "Historial Cerrados", Icono = FontAwesome.Sharp.IconChar.ClockRotateLeft }
            }, "activos");

            sidebarCliente.ItemSeleccionado += (s, clave) =>
            {
                if (clave == "activos")
                {
                    _vistaActual = VistaCliente.Activos;
                    topBarCliente.Titulo = "Mis Tickets Activos";
                    topBarCliente.Subtitulo = "Solicitudes en proceso y pendientes de atención";
                }
                else if (clave == "cerrados")
                {
                    _vistaActual = VistaCliente.Cerrados;
                    topBarCliente.Titulo = "Historial de Tickets Cerrados";
                    topBarCliente.Subtitulo = "Solicitudes resueltas y cerradas";
                }

                _controladorPaginacion.ReiniciarAPrimeraPagina();
                MostrarPaginaActual();
            };
        }

        private async Task CargarDatosDashboardAsync()
        {
            await _presenter.CargarTicketsClienteAsync(SesionSistema.IdUsuario);
        }

        private void MostrarPaginaActual()
        {
            // 1. Filtrar en memoria según la vista del Sidebar
            var query = _todosLosTickets.AsEnumerable();
            if (_vistaActual == VistaCliente.Activos)
            {
                query = query.Where(t => t.Status != ConstantesEstatus.CERRADO);
            }
            else if (_vistaActual == VistaCliente.Cerrados)
            {
                query = query.Where(t => t.Status == ConstantesEstatus.CERRADO);
            }

            // 2. Aplicar filtros dinámicos (Texto de búsqueda y Rango de Fechas)
            var vals = filtroCliente.ObtenerValoresFiltros();
            if (vals.TryGetValue("Texto", out var txtVal) && txtVal != null)
            {
                var txt = txtVal.ToString()?.Trim().ToLowerInvariant();
                if (!string.IsNullOrEmpty(txt))
                {
                    query = query.Where(t =>
                        (t.Folio?.ToLowerInvariant().Contains(txt) ?? false) ||
                        (t.Descripcion?.ToLowerInvariant().Contains(txt) ?? false) ||
                        (t.TecnicoAsignado?.ToLowerInvariant().Contains(txt) ?? false));
                }
            }

            if (vals.TryGetValue("FechaInicio", out var fiVal) && fiVal is DateTime dtInicio)
            {
                query = query.Where(t => t.FechaAlta >= dtInicio.Date);
            }

            if (vals.TryGetValue("FechaFin", out var ffVal) && fiVal is DateTime && ffVal is DateTime dtFin)
            {
                query = query.Where(t => t.FechaAlta <= dtFin.Date.AddDays(1).AddTicks(-1));
            }

            var ticketsFiltrados = query.ToList();

            // 3. Segmentar la página actual
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
                dgvMisTickets.ConfigurarOcultarColumnas("IdTicket", "Estatus", "Evaluacion");

                var colFolio = dgvMisTickets.Columns["Folio"];
                if (colFolio != null)
                {
                    colFolio.HeaderText = "Folio";
                    colFolio.FillWeight = 45;
                    colFolio.MinimumWidth = 80;
                }

                var colFecha = dgvMisTickets.Columns["FechaAlta"];
                if (colFecha != null)
                {
                    colFecha.HeaderText = "Fecha de Solicitud";
                    colFecha.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                    colFecha.FillWeight = 70;
                    colFecha.MinimumWidth = 120;
                }

                var colStatus = dgvMisTickets.Columns["Status"];
                if (colStatus != null)
                {
                    colStatus.HeaderText = "Estatus";
                    colStatus.FillWeight = 55;
                    colStatus.MinimumWidth = 85;
                }

                var colTecnico = dgvMisTickets.Columns["TecnicoAsignado"];
                if (colTecnico != null)
                {
                    colTecnico.HeaderText = "Técnico Asignado";
                    colTecnico.FillWeight = 75;
                    colTecnico.MinimumWidth = 120;
                }

                var colDesc = dgvMisTickets.Columns["Descripcion"];
                if (colDesc != null)
                {
                    colDesc.HeaderText = "Descripción del Problema";
                    colDesc.FillWeight = 160;
                    colDesc.MinimumWidth = 150;
                }

                var colFeedback = dgvMisTickets.Columns["Feedback"];
                if (colFeedback != null)
                {
                    colFeedback.HeaderText = "Calificación / Feedback";
                    colFeedback.FillWeight = 75;
                    colFeedback.MinimumWidth = 110;
                }
            }
        }

        private void UcMisActivos_Click(object? sender, EventArgs e)
        {
            _vistaActual = _vistaActual == VistaCliente.Activos ? VistaCliente.Todos : VistaCliente.Activos;
            sidebarCliente.SeleccionarItem(_vistaActual == VistaCliente.Activos ? "activos" : "");
            topBarCliente.Titulo = _vistaActual == VistaCliente.Activos ? "Mis Tickets Activos" : "Todos Mis Tickets";
            _controladorPaginacion.ReiniciarAPrimeraPagina();
            MostrarPaginaActual();
        }

        private void UcMisCerrados_Click(object? sender, EventArgs e)
        {
            _vistaActual = _vistaActual == VistaCliente.Cerrados ? VistaCliente.Todos : VistaCliente.Cerrados;
            sidebarCliente.SeleccionarItem(_vistaActual == VistaCliente.Cerrados ? "cerrados" : "");
            topBarCliente.Titulo = _vistaActual == VistaCliente.Cerrados ? "Historial de Tickets Cerrados" : "Todos Mis Tickets";
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
                ucMisActivos.ColorFondo = TemaVisual.TicketNuevo;
            }

            if (ucMisCerrados != null)
            {
                ucMisCerrados.Cantidad = cerrados.ToString();
                ucMisCerrados.Titulo = "Mis Tickets Cerrados";
                ucMisCerrados.ColorFondo = TemaVisual.TicketCerrado;
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
            MessageBox.Show(mensaje, "Error en Dashboard de Cliente", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion
    }
}

