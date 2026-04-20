using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HSis.Data.Models;
using HSis.Logic.Services;

namespace HSis.UI
{
    /// <summary>
    /// Formulario de solo lectura para que clientes (Rol 3) vean el progreso de sus tickets.
    /// Responsabilidad única: Mostrar información de ticket en modo consulta.
    /// Cumple SRP: No permite ediciones, solo visualización.
    /// </summary>
    public partial class frmDetalleCliente : Form
    {
        private int _idTicket;
        private readonly TicketService _ticketService;
        private Ticket? _ticketActual;

        public frmDetalleCliente(int idTicket)
        {
            InitializeComponent();
            _idTicket = idTicket;
            _ticketService = new TicketService();
        }

        private async void frmDetalleCliente_Load(object sender, EventArgs e)
        {
            try
            {
                // Cargar el ticket desde la base de datos
                _ticketActual = await _ticketService.ObtenerTicketPorIdAsync(_idTicket);

                if (_ticketActual == null)
                {
                    MessageBox.Show("Ticket no encontrado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                // Mostrar datos del ticket (solo lectura)
                lblFolioValor.Text = $"TK{_ticketActual.IdTicket:D6}";
                lblFechaAltaValor.Text = _ticketActual.Alta?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";
                lblEstatusValor.Text = _ticketActual.Status ?? "Desconocido";
                lblTecnicoValor.Text = _ticketActual.IdTecnicoNavigation?.Nombre ?? "Sin asignar";
                txtDescripcion.Text = _ticketActual.Descripción ?? string.Empty;
                txtSolucion.Text = _ticketActual.Solución ?? string.Empty;

                // Aplicar estilo de color según el estatus
                AplicarEstiloEstatus(_ticketActual.Status);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el ticket: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        /// <summary>
        /// Aplica colores visuales según el estatus del ticket para mejor UX.
        /// </summary>
        private void AplicarEstiloEstatus(string? estatus)
        {
            if (string.IsNullOrEmpty(estatus))
                return;

            if (estatus == ConstantesEstatus.ABIERTO)
            {
                lblEstatusValor.BackColor = Color.LightBlue;
                lblEstatusValor.ForeColor = Color.DarkBlue;
            }
            else if (estatus == ConstantesEstatus.EN_PROCESO)
            {
                lblEstatusValor.BackColor = Color.LightYellow;
                lblEstatusValor.ForeColor = Color.DarkGoldenrod;
            }
            else if (estatus == ConstantesEstatus.CERRADO)
            {
                lblEstatusValor.BackColor = Color.LightGreen;
                lblEstatusValor.ForeColor = Color.DarkGreen;
            }
            else if (estatus == ConstantesEstatus.REABIERTO)
            {
                lblEstatusValor.BackColor = Color.FromArgb(153, 102, 204);
                lblEstatusValor.ForeColor = Color.LightPink;
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
