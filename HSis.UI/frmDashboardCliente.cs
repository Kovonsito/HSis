using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HSis.Logic.DTOs;
using HSis.Logic.Services;

namespace HSis.UI
{
    public partial class frmDashboardCliente : Form
    {
        private readonly TicketService _ticketService;

        public frmDashboardCliente()
        {
            InitializeComponent();
            _ticketService = new TicketService();
        }

        private async void frmDashboardCliente_Load(object sender, EventArgs e)
        {
            await CargarTicketsAsync();
            await ActualizarIndicadorAsync();
        }

        private async Task CargarTicketsAsync()
        {
            try
            {
                var tickets = await _ticketService.ObtenerTicketsPorUsuarioAsync(SesionSistema.IdUsuario);
                var ticketsDto = tickets.ConvertAll(t => new TicketClienteDto
                {
                    IdTicket = t.IdTicket,
                    FechaAlta = t.Alta,
                    Status = t.Status,
                    TecnicoAsignado = t.IdTecnicoNavigation?.Nombre ?? "Sin asignar",
                    Descripcion = t.Descripción?.Length > 50 ? t.Descripción.Substring(0, 50) + "..." : t.Descripción
                });

                dgvMisTickets.DataSource = ticketsDto;
                PersonalizarColumnas();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar tickets: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PersonalizarColumnas()
        {
            if (dgvMisTickets.Columns.Count > 0)
            {
                dgvMisTickets.Columns["IdTicket"].Visible = false;
                dgvMisTickets.Columns["Folio"].HeaderText = "Folio";
                dgvMisTickets.Columns["FechaAlta"].HeaderText = "Fecha de Alta";
                dgvMisTickets.Columns["Status"].HeaderText = "Estatus";
                dgvMisTickets.Columns["TecnicoAsignado"].HeaderText = "Técnico Asignado";
                dgvMisTickets.Columns["Descripcion"].HeaderText = "Descripción";

                dgvMisTickets.Columns["FechaAlta"].Width = 100;
                dgvMisTickets.Columns["Status"].Width = 100;
                dgvMisTickets.Columns["TecnicoAsignado"].Width = 120;
            }
        }

        private async Task ActualizarIndicadorAsync()
        {
            try
            {
                var tickets = await _ticketService.ObtenerTicketsPorUsuarioAsync(SesionSistema.IdUsuario);
                var activos = tickets.FindAll(t => t.Status != ConstantesEstatus.CERRADO);

                ucMisActivos.Cantidad = activos.Count.ToString();
                ucMisActivos.Titulo = "Mis Tickets Activos";
                ucMisActivos.ColorFondo = Color.FromArgb(41, 128, 185);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar indicador: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNuevoReporte_Click(object sender, EventArgs e)
        {
            using (var frmNuevo = new frmNuevoReporte())
            {
                if (frmNuevo.ShowDialog() == DialogResult.OK)
                {
                    CargarTicketsAsync().ConfigureAwait(false);
                }
            }
        }

        private void dgvMisTickets_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvMisTickets.Rows[e.RowIndex];
                if (int.TryParse(row.Cells["IdTicket"].Value?.ToString(), out int idTicket))
                {
                    using (var frmTicket = new frmTicket(idTicket))
                    {
                        frmTicket.ShowDialog();
                        CargarTicketsAsync().ConfigureAwait(false);
                    }
                }
            }
        }
    }
}
