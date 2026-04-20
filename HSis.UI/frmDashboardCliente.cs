using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HSis.Logic.DTOs;
using HSis.Logic.Services;
using HSis.Data.Models;

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
            SesionSistema.ConfigurarMenuSesion(this);
            // Cargamos la información una sola vez para evitar múltiples llamadas a la BD
            await CargarDatosDashboardAsync();
        }

        private async Task CargarDatosDashboardAsync()
        {
            try
            {
                var tickets = await _ticketService.ObtenerTicketsPorUsuarioAsync(SesionSistema.IdUsuario);
                
                // Actualizar Grid
                ActualizarGridTickets(tickets);
                
                // Actualizar Indicador
                ActualizarIndicador(tickets);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el dashboard: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarGridTickets(List<Ticket> tickets)
        {
            var ticketsDto = tickets.ConvertAll(t => new TicketClienteDto
            {
                IdTicket = t.IdTicket,
                FechaAlta = t.Alta,
                Status = t.Status,
                TecnicoAsignado = t.IdTecnicoNavigation?.Nombre ?? "Sin asignar",
                Descripcion = !string.IsNullOrEmpty(t.Descripción) && t.Descripción.Length > 50 ? t.Descripción.Substring(0, 50) + "..." : (t.Descripción ?? "")
            });

            dgvMisTickets.DataSource = ticketsDto;
            PersonalizarColumnas();
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

        private void ActualizarIndicador(List<Ticket> tickets)
        {
            var activos = tickets.FindAll(t => t.Status != ConstantesEstatus.CERRADO);

            ucMisActivos.Cantidad = activos.Count.ToString();
            ucMisActivos.Titulo = "Mis Tickets Activos";
            ucMisActivos.ColorFondo = Color.FromArgb(41, 128, 185);
        }

        private void btnNuevoReporte_Click(object sender, EventArgs e)
        {
            using (var frmNuevo = new frmNuevoTicket())
            {
                if (frmNuevo.ShowDialog() == DialogResult.OK)
                {
                    _ = CargarDatosDashboardAsync();
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
                    using (var frmDetalle = new frmDetalleCliente(idTicket))
                    {
                        frmDetalle.ShowDialog();
                        _ = CargarDatosDashboardAsync();
                    }
                }
            }
        }
    }
}
