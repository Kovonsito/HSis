#nullable enable
using System.Runtime.Versioning;
using HSis.Data.Models;
using HSis.Logic.DTOs;
using HSis.Logic.Services;

namespace HSis.UI
{
    [SupportedOSPlatform("windows")]
    public partial class frmDashboardTecnico : Form
    {
        private readonly TicketService _ticketService;
        private enum VistaDashboard
        {
            MisAsignados,
            Disponibles,
            Cerrados
        }
        private VistaDashboard _vistaActual = VistaDashboard.MisAsignados;

        public frmDashboardTecnico(TicketService ticketService)
        {
            InitializeComponent();
            _ticketService = ticketService;
        }

        private async void frmDashboardTecnico_Load(object? sender, EventArgs e)
        {
            SesionSistema.ConfigurarMenuSesion(this);
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

                await Task.WhenAll(taskMisAsignados, taskDisponibles, taskCerrados);

                var misAsignados = taskMisAsignados.Result;
                var disponibles = taskDisponibles.Result;
                var cerrados = taskCerrados.Result;

                ucMisAsignados.Cantidad = misAsignados.Count.ToString();
                ucMisAsignados.Titulo = "Mis Asignados";
                ucMisAsignados.ColorFondo = Color.FromArgb(41, 128, 185);
                // Suscribir solo una vez
                ucMisAsignados.ucIndicadorEvent -= UcMisAsignados_Click;
                ucMisAsignados.ucIndicadorEvent += UcMisAsignados_Click;

                ucDisponibles.Cantidad = disponibles.Count.ToString();
                ucDisponibles.Titulo = "Disponibles";
                ucDisponibles.ColorFondo = Color.FromArgb(241, 196, 15);
                ucDisponibles.ucIndicadorEvent -= UcDisponibles_Click;
                ucDisponibles.ucIndicadorEvent += UcDisponibles_Click;

                ucCerrados.Cantidad = cerrados.Count.ToString();
                ucCerrados.Titulo = "Mis Cerrados";
                ucCerrados.ColorFondo = Color.FromArgb(46, 204, 113);
                ucCerrados.ucIndicadorEvent -= UcCerrados_Click;
                ucCerrados.ucIndicadorEvent += UcCerrados_Click;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar indicadores: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            var ticketsDto = tickets.Select(t => new TicketOperativoDto
            {
                IdTicket = t.IdTicket,
                FechaAlta = t.Alta,
                Status = t.Status ?? "N/A",
                Usuario = t.NombreUsuario ?? "N/A",
                Descripcion = !string.IsNullOrEmpty(t.Descripcion) && t.Descripcion.Length > 50 ? t.Descripcion.Substring(0, 50) + "..." : (t.Descripcion ?? ""),
                Prioridad = t.Prioridad
            }).ToList();

            dgvTicketsOperativos.DataSource = new SortableBindingList<TicketOperativoDto>(ticketsDto);
            PersonalizarColumnas();
        }

        private void PersonalizarColumnas()
        {
            if (dgvTicketsOperativos.Columns.Count > 0)
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

        private async void dgvTicketsOperativos_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvTicketsOperativos.Rows[e.RowIndex];
                if (int.TryParse(row.Cells["IdTicket"].Value?.ToString(), out int idTicket))
                {
                    if (Program.ServiceProvider is null) return;
                    using var frmTicket = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance<frmTicketDetalle>(Program.ServiceProvider, idTicket);

                    frmTicket.ShowDialog();
                    await CargarIndicadoresAsync();
                    await CargarTicketsSegunVistaAsync();
                }
            }
        }
    }
}
