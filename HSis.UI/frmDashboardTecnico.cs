using HSis.Data.Models;
using HSis.Logic.DTOs;
using HSis.Logic.Services;

namespace HSis.UI
{
    public partial class frmDashboardTecnico : Form
    {
        private readonly TicketService _ticketService;
        private bool _mostrandoMisAsignados = true;

        public frmDashboardTecnico(TicketService ticketService)
        {
            InitializeComponent();
            _ticketService = ticketService;
        }

        private async void frmDashboardTecnico_Load(object sender, EventArgs e)
        {
            SesionSistema.ConfigurarMenuSesion(this);
            // Cargamos indicadores y grid en paralelo
            await CargarDatosInicialesAsync();
        }

        private async Task CargarDatosInicialesAsync()
        {
            // Ejecutamos las dos cargas principales en paralelo
            await Task.WhenAll(CargarIndicadoresAsync(), CargarTicketsMisAsignadosAsync());
        }

        private async Task CargarIndicadoresAsync()
        {
            try
            {
                // Consultas en paralelo para los indicadores
                var taskMisAsignados = _ticketService.ObtenerTicketsAsignadosATecnicoAsync(SesionSistema.IdUsuario);
                var taskDisponibles = _ticketService.ObtenerTicketsDisponiblesAsync();

                await Task.WhenAll(taskMisAsignados, taskDisponibles);

                var misAsignados = taskMisAsignados.Result;
                var disponibles = taskDisponibles.Result;

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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar indicadores: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void UcMisAsignados_Click(object sender, EventArgs e)
        {
            _mostrandoMisAsignados = true;
            await CargarTicketsMisAsignadosAsync();
        }

        private async void UcDisponibles_Click(object sender, EventArgs e)
        {
            _mostrandoMisAsignados = false;
            await CargarTicketsDisponiblesAsync();
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

        private void CargarGridTickets(List<Ticket> tickets)
        {
            var ticketsDto = tickets.ConvertAll(t => new TicketOperativoDto
            {
                IdTicket = t.IdTicket,
                FechaAlta = t.Alta,
                Status = t.Status,
                Usuario = t.IdUsuarioNavigation?.Nombre ?? "Desconocido",
                Descripcion = !string.IsNullOrEmpty(t.Descripción) && t.Descripción.Length > 50 ? t.Descripción.Substring(0, 50) + "..." : (t.Descripción ?? "")
            });

            dgvTicketsOperativos.DataSource = ticketsDto;
            PersonalizarColumnas();
        }

        private void PersonalizarColumnas()
        {
            if (dgvTicketsOperativos.Columns.Count > 0)
            {
                dgvTicketsOperativos.Columns["IdTicket"].Visible = false;
                dgvTicketsOperativos.Columns["FechaAlta"].HeaderText = "Fecha de Alta";
                dgvTicketsOperativos.Columns["Status"].HeaderText = "Estatus";
                dgvTicketsOperativos.Columns["Usuario"].HeaderText = "Usuario Reportó";
                dgvTicketsOperativos.Columns["Descripcion"].HeaderText = "Descripción";

                dgvTicketsOperativos.Columns["FechaAlta"].Width = 100;
                dgvTicketsOperativos.Columns["Status"].Width = 100;
                dgvTicketsOperativos.Columns["Usuario"].Width = 120;
            }
        }

        private async void dgvTicketsOperativos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvTicketsOperativos.Rows[e.RowIndex];
                if (int.TryParse(row.Cells["IdTicket"].Value?.ToString(), out int idTicket))
                {
                    using var frmTicket = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance<frmTicketDetalle>(Program.ServiceProvider, idTicket);

                    frmTicket.ShowDialog();
                    await CargarIndicadoresAsync();
                    if (_mostrandoMisAsignados)
                    {
                        await CargarTicketsMisAsignadosAsync();
                    }
                    else
                    {
                        await CargarTicketsDisponiblesAsync();
                    }

                }
            }
        }
    }
}
