#nullable enable
using System.Runtime.Versioning;
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
        private readonly AdministradorUINotificaciones _uiManager;
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

        private readonly ISessionCacheService _sessionCache;

        public DashboardClienteForm(
            ITicketService ticketService,
            AdministradorUINotificaciones uiManager,
            IFabricaFormularios formFactory,
            ISessionCacheService sessionCache,
            DashboardClientePresenter? presenter = null)
        {
            InitializeComponent();
            _ticketService = ticketService;
            _uiManager = uiManager;
            _formFactory = formFactory;
            _sessionCache = sessionCache;
            _presenter = presenter;
            _presenter?.SetView(this);
        }

        private async void frmDashboardCliente_Load(object? sender, EventArgs e)
        {
            InicializarLayoutDashboard();
            SesionSistema.ConfigurarMenuSesion(this, _sessionCache);

            var tblPrincipal = this.Controls["tblPrincipal"];
            if (tblPrincipal != null)
            {
                _uiManager.Adjuntar(this, tblPrincipal, CargarDatosDashboardAsync);
            }

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

        private void InicializarLayoutDashboard()
        {
            // Instanciar control de paginación
            PaginacionControl = new PaginacionControl();
            PaginacionControl.Dock = DockStyle.Fill;
            PaginacionControl.PaginaCambiada += (s, e) => MostrarPaginaActual();
            PaginacionControl.Margin = new Padding(12, 0, 12, 6);

            // Instanciar indicador de cerrados
            ucMisCerrados = new IndicadorControl();

            // Suscribir eventos de filtrado
            ucMisActivos.IndicadorClic += UcMisActivos_Click;
            ucMisCerrados.IndicadorClic += UcMisCerrados_Click;

            var tblPrincipal = CrearPanelPrincipal();
            var tblIndicadores = CrearPanelIndicadores();

            // Configurar el título y el grid principal para que se estiren
            lblTitulo.Dock = DockStyle.Fill;
            lblTitulo.Margin = new Padding(12, 10, 12, 10);

            // Configurar el grid principal para que se estire
            dgvMisTickets.Dock = DockStyle.Fill;
            dgvMisTickets.Margin = new Padding(12, 10, 12, 12);

            // Agregar componentes al TableLayoutPanel principal
            tblPrincipal.Controls.Add(lblTitulo, 0, 0);
            tblPrincipal.Controls.Add(tblIndicadores, 0, 1);
            tblPrincipal.Controls.Add(dgvMisTickets, 0, 2);
            tblPrincipal.Controls.Add(PaginacionControl, 0, 3);

            // Remover controles del formulario para agregarlos al grid principal
            ReubicarControlesAlPrincipal(tblPrincipal);
        }

        private TableLayoutPanel CrearPanelPrincipal()
        {
            var tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Name = "tblPrincipal",
                RowCount = 4,
                ColumnCount = 1,
                Size = this.ClientSize
            };

            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Fila 0: Título (lblTitulo)
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 120F)); // Fila 1: Indicador y Botón
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Fila 2: Grid de Tickets (dgvMisTickets)
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F)); // Fila 3: Paginación

            return tbl;
        }

        private TableLayoutPanel CrearPanelIndicadores()
        {
            var tblCabecera = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Name = "tblCabecera",
                RowCount = 1,
                ColumnCount = 3,
                Margin = new Padding(7, 0, 7, 0)
            };
            tblCabecera.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F)); // Columna 0: ucMisActivos
            tblCabecera.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F)); // Columna 1: ucMisCerrados
            tblCabecera.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); // Columna 2: btnNuevoReporte centrado

            // Panel contenedor para centrar verticalmente el botón
            var pnlBoton = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0)
            };

            // Configurar el botón
            btnNuevoReporte.Location = new Point(10, 25); // Posicionarlo de forma prolija en el panel (centrado vertical en altura 110px)
            pnlBoton.Controls.Add(btnNuevoReporte);

            ucMisActivos.Dock = DockStyle.Fill;
            ucMisActivos.Margin = new Padding(5);

            ucMisCerrados.Dock = DockStyle.Fill;
            ucMisCerrados.Margin = new Padding(5);

            tblCabecera.Controls.Add(ucMisActivos, 0, 0);
            tblCabecera.Controls.Add(ucMisCerrados, 1, 0);
            tblCabecera.Controls.Add(pnlBoton, 2, 0);

            return tblCabecera;
        }

        private void ReubicarControlesAlPrincipal(TableLayoutPanel tblPrincipal)
        {
            this.Controls.Remove(lblTitulo);
            this.Controls.Remove(ucMisActivos);
            this.Controls.Remove(btnNuevoReporte);
            this.Controls.Remove(dgvMisTickets);

            this.Controls.Add(tblPrincipal);
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

