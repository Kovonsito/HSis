using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HSis.Logic.DTOs;
using HSis.Data.Models;
using HSis.Logic.Services;

namespace HSis.UI
{
    public partial class frmDashboardTecnico : Form
    {
        private readonly TicketService _ticketService;
        private bool _mostrandoMisAsignados = true;

        public frmDashboardTecnico()
        {
            InitializeComponent();
            _ticketService = new TicketService();
        }

        private async void frmDashboardTecnico_Load(object sender, EventArgs e)
        {
            await CargarIndicadoresAsync();
            await CargarTicketsMisAsignadosAsync();
        }

        private async Task CargarIndicadoresAsync()
        {
            try
            {
                var misAsignados = await _ticketService.ObtenerTicketsAsignadosATecnicoAsync(SesionSistema.IdUsuario);
                var disponibles = await _ticketService.ObtenerTicketsDisponiblesAsync();

                ucMisAsignados.Cantidad = misAsignados.Count.ToString();
                ucMisAsignados.Titulo = "Mis Asignados";
                ucMisAsignados.ColorFondo = Color.FromArgb(41, 128, 185);
                ucMisAsignados.ucIndicadorEvent += UcMisAsignados_Click;

                ucDisponibles.Cantidad = disponibles.Count.ToString();
                ucDisponibles.Titulo = "Disponibles";
                ucDisponibles.ColorFondo = Color.FromArgb(241, 196, 15);
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
                Descripcion = t.Descripción?.Length > 50 ? t.Descripción.Substring(0, 50) + "..." : t.Descripción
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
                    using (var frmTicket = new frmTicket(idTicket))
                    {
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
}
