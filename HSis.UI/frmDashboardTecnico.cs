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
    public partial class frmDashboardTecnico : Form
    {
        private readonly ITicketService _ticketService;
        private enum VistaDashboard
        {
            MisAsignados,
            Disponibles,
            Cerrados,
            Calificaciones
        }
        private VistaDashboard _vistaActual = VistaDashboard.MisAsignados;
        private readonly NotificationUIManager _uiManager;
        private ucPaginacion ucPaginacion = null!;
        private List<TicketOperativoDto> _todosLosTickets = [];
        private List<FeedbackTecnicoDto> _todosLosFeedbacks = [];

        private readonly IFormFactory _formFactory;
        private readonly ISessionCacheService _sessionCache;

        public frmDashboardTecnico(
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

        private async void frmDashboardTecnico_Load(object? sender, EventArgs e)
        {
            InicializarLayoutDashboard();
            SesionSistema.ConfigurarMenuSesion(this, _sessionCache);

            var tblPrincipal = this.Controls["tblPrincipal"];
            if (tblPrincipal != null)
            {
                _uiManager.Attach(this, tblPrincipal, CargarDatosInicialesAsync);
            }

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
                    Comentario = t.ComentarioFeedback ?? "Sin comentarios",
                    Fecha = t.FechaFeedback
                })];

                ucPaginacion.CurrentPage = 1;
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
                FechaAlta = t.Alta,
                Status = t.Status ?? "N/A",
                Usuario = t.NombreUsuario ?? "N/A",
                Descripcion = !string.IsNullOrEmpty(t.Descripcion) && t.Descripcion.Length > 50 ? t.Descripcion[..50] + "..." : (t.Descripcion ?? ""),
                Prioridad = t.Prioridad
            })];

            ucPaginacion.CurrentPage = 1;
            MostrarPaginaActual();
        }

        private void MostrarPaginaActual()
        {
            if (_vistaActual == VistaDashboard.Calificaciones)
            {
                ucPaginacion.TotalRecords = _todosLosFeedbacks.Count;
                var pageFeedbacks = _todosLosFeedbacks
                    .Skip((ucPaginacion.CurrentPage - 1) * ucPaginacion.PageSize)
                    .Take(ucPaginacion.PageSize)
                    .ToList();
                dgvTicketsOperativos.DataSource = new SortableBindingList<FeedbackTecnicoDto>(pageFeedbacks);
            }
            else
            {
                ucPaginacion.TotalRecords = _todosLosTickets.Count;
                var pageTickets = _todosLosTickets
                    .Skip((ucPaginacion.CurrentPage - 1) * ucPaginacion.PageSize)
                    .Take(ucPaginacion.PageSize)
                    .ToList();
                dgvTicketsOperativos.DataSource = new SortableBindingList<TicketOperativoDto>(pageTickets);
            }
            PersonalizarColumnas();
            ucPaginacion.ActualizarInterfaz();
        }

        private void PersonalizarColumnas()
        {
            if (dgvTicketsOperativos.Columns.Count > 0)
            {
                if (dgvTicketsOperativos.DataSource is SortableBindingList<FeedbackTecnicoDto>)
                {
                    if (dgvTicketsOperativos.Columns["IdTicket"] is DataGridViewColumn colId) colId.Visible = false;
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
                    if (dgvTicketsOperativos.Columns["IdTicket"] is DataGridViewColumn colId) colId.Visible = false;
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
            }
        }

        private async void dgvTicketsOperativos_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvTicketsOperativos.Rows[e.RowIndex];
                if (int.TryParse(row.Cells["IdTicket"].Value?.ToString(), out int idTicket))
                {
                    using var frmTicket = _formFactory.CreateTicketDetalle(idTicket);

                    frmTicket.ShowDialog();
                    await CargarIndicadoresAsync();
                    await CargarTicketsSegunVistaAsync();
                }
            }
        }

        private void InicializarLayoutDashboard()
        {
            // Instanciar control de paginación
            ucPaginacion = new ucPaginacion();
            ucPaginacion.PageChanged += (s, e) => MostrarPaginaActual();

            // Suscribir eventos de filtrado una sola vez aquí (hilo de UI garantizado)
            ucMisAsignados.ucIndicadorEvent += UcMisAsignados_Click;
            ucDisponibles.ucIndicadorEvent   += UcDisponibles_Click;
            ucCerrados.ucIndicadorEvent      += UcCerrados_Click;
            ucCalificacion.ucIndicadorEvent  += UcCalificacion_Click;

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
            tblPrincipal.RowStyles.Add(new RowStyle(SizeType.Absolute, 130F)); // Fila 1: Indicadores (ucMisAsignados, etc.)
            tblPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Fila 2: Grid de Tickets (dgvTicketsOperativos)
            tblPrincipal.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F)); // Fila 3: Paginación

            // 2. Crear un TableLayoutPanel interno para los indicadores (4 columnas, 25% c/u)
            var tblIndicadores = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Name = "tblIndicadores",
                RowCount = 1,
                ColumnCount = 4,
                Margin = new Padding(0)
            };
            tblIndicadores.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tblIndicadores.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tblIndicadores.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tblIndicadores.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

            // Configurar los indicadores para que ocupen todo su espacio asignado
            ucMisAsignados.Dock = DockStyle.Fill;
            ucDisponibles.Dock = DockStyle.Fill;
            ucCerrados.Dock = DockStyle.Fill;
            ucCalificacion.Dock = DockStyle.Fill;

            ucMisAsignados.Margin = new Padding(5);
            ucDisponibles.Margin = new Padding(5);
            ucCerrados.Margin = new Padding(5);
            ucCalificacion.Margin = new Padding(5);

            // Agregar los indicadores al sub-grid
            tblIndicadores.Controls.Add(ucMisAsignados, 0, 0);
            tblIndicadores.Controls.Add(ucDisponibles, 1, 0);
            tblIndicadores.Controls.Add(ucCerrados, 2, 0);
            tblIndicadores.Controls.Add(ucCalificacion, 3, 0);

            // Configurar el título y el grid principal para que se estiren
            lblTitulo.Dock = DockStyle.Fill;
            lblTitulo.Margin = new Padding(12, 10, 12, 10);

            // Agregar componentes al TableLayoutPanel principal
            tblPrincipal.Controls.Add(lblTitulo, 0, 0);
            tblPrincipal.Controls.Add(tblIndicadores, 0, 1);
            tblPrincipal.Controls.Add(dgvTicketsOperativos, 0, 2);
            tblPrincipal.Controls.Add(ucPaginacion, 0, 3);

            // Remover controles del formulario para agregarlos al grid principal
            this.Controls.Remove(lblTitulo);
            this.Controls.Remove(ucMisAsignados);
            this.Controls.Remove(ucDisponibles);
            this.Controls.Remove(ucCerrados);
            this.Controls.Remove(ucCalificacion);
            this.Controls.Remove(dgvTicketsOperativos);

            // Agregar el panel principal al formulario
            this.Controls.Add(tblPrincipal);
        }
    }
}
