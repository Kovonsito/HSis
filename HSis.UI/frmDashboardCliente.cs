#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;
using HSis.Data.Models;
using HSis.Logic.DTOs;
using HSis.Logic.Services;

namespace HSis.UI
{
    [SupportedOSPlatform("windows")]
    public partial class frmDashboardCliente : Form
    {
        private readonly ITicketService _ticketService;
        private readonly NotificationUIManager _uiManager;
        private ucPaginacion ucPaginacion = null!;
        private List<TicketClienteDto> _todosLosTickets = [];

        private ucIndicador ucMisCerrados = null!;
        private readonly IFormFactory _formFactory;

        private enum VistaCliente
        {
            Todos,
            Activos,
            Cerrados
        }
        private VistaCliente _vistaActual = VistaCliente.Todos;

        private readonly ISessionCacheService _sessionCache;

        public frmDashboardCliente(
            ITicketService ticketService,
            NotificationUIManager uiManager,
            IFormFactory formFactory,
            ISessionCacheService sessionCache)
        {
            InitializeComponent();
            _ticketService = ticketService;
            _uiManager = uiManager;
            _formFactory = formFactory;
            _sessionCache = sessionCache;
        }

        private async void frmDashboardCliente_Load(object? sender, EventArgs e)
        {
            InicializarLayoutDashboard();
            SesionSistema.ConfigurarMenuSesion(this, _sessionCache);

            var tblPrincipal = this.Controls["tblPrincipal"];
            if (tblPrincipal != null)
            {
                _uiManager.Attach(this, tblPrincipal, CargarDatosDashboardAsync);
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
                FechaAlta = t.Alta,
                Status = t.Status,
                TecnicoAsignado = t.NombreTecnico ?? "Sin asignar",
                Descripcion = !string.IsNullOrEmpty(t.Descripcion) && t.Descripcion.Length > 50 ? string.Concat(t.Descripcion.AsSpan(0, 50), "...") : (t.Descripcion ?? ""),
                Feedback = t.Status == ConstantesEstatus.CERRADO
                    ? (t.Calificacion.HasValue ? $"Enviada ({new string('★', t.Calificacion.Value)}{new string('☆', 5 - t.Calificacion.Value)})" : "Pendiente")
                    : "N/A"
            });

            ucPaginacion.CurrentPage = 1;
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

            ucPaginacion.TotalRecords = ticketsFiltrados.Count;

            // 2. Segmentar la página actual
            var pageTickets = ticketsFiltrados
                .Skip((ucPaginacion.CurrentPage - 1) * ucPaginacion.PageSize)
                .Take(ucPaginacion.PageSize)
                .ToList();

            dgvMisTickets.DataSource = new SortableBindingList<TicketClienteDto>(pageTickets);
            PersonalizarColumnas();
            ucPaginacion.ActualizarInterfaz();
        }

        private void PersonalizarColumnas()
        {
            if (dgvMisTickets.Columns.Count > 0)
            {
                var colIdTicket = dgvMisTickets.Columns["IdTicket"];
                if (colIdTicket != null) colIdTicket.Visible = false;

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
            }
        }

        private void ActualizarIndicador(List<TicketDto> tickets)
        {
            var activos = tickets.FindAll(t => t.Status != ConstantesEstatus.CERRADO);
            var cerrados = tickets.FindAll(t => t.Status == ConstantesEstatus.CERRADO);

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
            ucPaginacion.CurrentPage = 1;
            MostrarPaginaActual();
        }

        private void UcMisCerrados_Click(object? sender, EventArgs e)
        {
            _vistaActual = _vistaActual == VistaCliente.Cerrados ? VistaCliente.Todos : VistaCliente.Cerrados;
            ucPaginacion.CurrentPage = 1;
            MostrarPaginaActual();
        }

        private void btnNuevoReporte_Click(object? sender, EventArgs e)
        {
            using var frmNuevo = _formFactory.Create<frmNuevoTicket>();
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
                    using var frmDetalle = _formFactory.CreateDetalleCliente(idTicket);
                    frmDetalle.ShowDialog();
                    await CargarDatosDashboardAsync();
                }
            }
        }

        private void InicializarLayoutDashboard()
        {
            // Instanciar control de paginación
            ucPaginacion = new ucPaginacion();
            ucPaginacion.PageChanged += (s, e) => MostrarPaginaActual();

            // Instanciar indicador de cerrados
            ucMisCerrados = new ucIndicador();

            // Suscribir eventos de filtrado
            ucMisActivos.ucIndicadorEvent += UcMisActivos_Click;
            ucMisCerrados.ucIndicadorEvent += UcMisCerrados_Click;

            // 1. Crear el TableLayoutPanel principal que ocupará todo el formulario
            var tblPrincipal = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Name = "tblPrincipal",
                RowCount = 4,
                ColumnCount = 1,
                Size = this.ClientSize
            };

            // Definir filas del grid principal
            tblPrincipal.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Fila 0: Título (lblTitulo)
            tblPrincipal.RowStyles.Add(new RowStyle(SizeType.Absolute, 110F)); // Fila 1: Indicador y Botón
            tblPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Fila 2: Grid de Tickets (dgvMisTickets)
            tblPrincipal.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F)); // Fila 3: Paginación

            // 2. Crear un TableLayoutPanel interno para el indicador y el botón de nuevo ticket
            var tblCabecera = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Name = "tblCabecera",
                RowCount = 1,
                ColumnCount = 3,
                Margin = new Padding(0)
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
            btnNuevoReporte.Location = new Point(10, 20); // Posicionarlo de forma prolija en el panel
            pnlBoton.Controls.Add(btnNuevoReporte);

            ucMisActivos.Dock = DockStyle.Fill;
            ucMisActivos.Margin = new Padding(12, 5, 5, 5);

            ucMisCerrados.Dock = DockStyle.Fill;
            ucMisCerrados.Margin = new Padding(5);

            tblCabecera.Controls.Add(ucMisActivos, 0, 0);
            tblCabecera.Controls.Add(ucMisCerrados, 1, 0);
            tblCabecera.Controls.Add(pnlBoton, 2, 0);

            // Configurar el título y el grid principal para que se estiren
            lblTitulo.Dock = DockStyle.Fill;
            lblTitulo.Margin = new Padding(12, 10, 12, 10);

            // Configurar el grid principal para que se estire
            dgvMisTickets.Dock = DockStyle.Fill;
            dgvMisTickets.Margin = new Padding(12, 10, 12, 12);

            // Agregar componentes al TableLayoutPanel principal
            tblPrincipal.Controls.Add(lblTitulo, 0, 0);
            tblPrincipal.Controls.Add(tblCabecera, 0, 1);
            tblPrincipal.Controls.Add(dgvMisTickets, 0, 2);
            tblPrincipal.Controls.Add(ucPaginacion, 0, 3);

            // Remover controles del formulario para agregarlos al grid principal
            this.Controls.Remove(lblTitulo);
            this.Controls.Remove(ucMisActivos);
            this.Controls.Remove(btnNuevoReporte);
            this.Controls.Remove(dgvMisTickets);

            // Agregar el panel principal al formulario
            this.Controls.Add(tblPrincipal);
        }


    }
}
