using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using HSis.Data.Models;
using HSis.Logic.DTOs;
using HSis.Logic.Services;

namespace HSis.UI
{
    public partial class frmDashboardAdmin : Form
    {
        private readonly TicketService _ticketService;

        public frmDashboardAdmin()
        {
            InitializeComponent();
            _ticketService = new TicketService();
        }

        private async void DashboardAdmin_Load(object sender, EventArgs e)
        {
            await CargarKPIsAsync();
            await CargarGridCompletoAsync();
        }

        private async Task CargarKPIsAsync()
        {
            ucNuevos.Titulo = "Nuevos";
            ucNuevos.Cantidad = (await _ticketService.ObtenerCountTicketsPorSLAAsync(false)).ToString();
            ucNuevos.ColorFondo = Color.DodgerBlue;
            ucNuevos.ImagenFondo = Properties.Resources.Nuevo;

            ucUrgentes.Titulo = "Urgentes";
            ucUrgentes.Cantidad = (await _ticketService.ObtenerCountTicketsPorSLAAsync(true)).ToString();
            ucUrgentes.ColorFondo = Color.Red;
            ucUrgentes.ImagenFondo = Properties.Resources.Urgente;

            ucEnProceso.Titulo = "En proceso";
            ucEnProceso.Cantidad = (await _ticketService.ObtenerCountTicketsPorEstatusAsync(ConstantesEstatus.EN_PROCESO)).ToString();
            ucEnProceso.ColorFondo = Color.Yellow;
            ucEnProceso.ImagenFondo = Properties.Resources.En_proceso;

            ucCerrados.Titulo = "Cerrados";
            ucCerrados.Cantidad = (await _ticketService.ObtenerCountTicketsPorEstatusAsync(ConstantesEstatus.CERRADO)).ToString();
            ucCerrados.ColorFondo = Color.LawnGreen;
            ucCerrados.ImagenFondo = Properties.Resources.Cerrado;
        }

        private async Task CargarGridCompletoAsync()
        {
            var todos = await _ticketService.ObtenerTicketsAsync();
            ActualizarGrid(todos);
        }

        private void ActualizarGrid(List<Ticket> listaTickets)
        {
            var listaMapeada = listaTickets.Select(t => new TicketGridDto
            {
                Folio = t.IdTicket,
                NombreUsuario = t.IdUsuarioNavigation?.Nombre ?? "N/A",
                Status = t.Status ?? "N/A",
                Alta = t.Alta,
                Atención = t.Atención,
                Cierre = t.Cierre ?? DateTime.Now,
                AtendidoPor = t.IdTecnicoNavigation != null ? t.IdTecnicoNavigation.Nombre : "N/A",
                Descripción = t.Descripción ?? "N/A",
                Solución = t.Solución ?? "N/A"
            }).ToList();

            dgvTickets.DataSource = listaMapeada;
        }

        public async void ucNuevos_ucIndicadorEvent(object sender, EventArgs e)
        {
            var filtrados = await _ticketService.ObtenerTicketsPorSLAAsync(false);
            ActualizarGrid(filtrados);
        }

        private async void ucUrgentes_ucIndicadorEvent(object sender, EventArgs e)
        {
            var filtrados = await _ticketService.ObtenerTicketsPorSLAAsync(true);
            ActualizarGrid(filtrados);
        }

        private async void ucEnProceso_ucIndicadorEvent(object sender, EventArgs e)
        {
            var filtrados = await _ticketService.ObtenerTicketsPorEstatusAsync(ConstantesEstatus.EN_PROCESO);
            ActualizarGrid(filtrados);
        }

        private async void ucCerrados_ucIndicadorEvent(object sender, EventArgs e)
        {
            var filtrados = await _ticketService.ObtenerTicketsPorEstatusAsync(ConstantesEstatus.CERRADO);
            ActualizarGrid(filtrados);
        }

        private async void btnRecargar_Click(object sender, EventArgs e)
        {
            await CargarGridCompletoAsync();
            await CargarKPIsAsync();
        }

        private void dgvTickets_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Obtenemos el valor de la celda "Folio"
                int idSeleccionado = (int)dgvTickets.Rows[e.RowIndex].Cells["Folio"].Value;

                // Pasamos el ID al constructor del formulario
                frmTicket formulario = new frmTicket(idSeleccionado);
                formulario.ShowDialog();

                // Al cerrar el detalle, recargamos el Dashboard por si hubo cambios
                _ = CargarKPIsAsync();
                _ = CargarGridCompletoAsync();
            }
        }
    }
}

