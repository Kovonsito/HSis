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
    public partial class frmNuevoReporte : Form
    {
        private readonly TicketService _ticketService;

        public frmNuevoReporte()
        {
            InitializeComponent();
            _ticketService = new TicketService();
        }

        private async void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(rtbDescripcion.Text))
            {
                MessageBox.Show("Por favor, describe el problema", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var nuevoTicket = new Ticket
                {
                    IdUsuario = SesionSistema.IdUsuario,
                    Descripción = rtbDescripcion.Text,
                    Status = ConstantesEstatus.ABIERTO,
                    Alta = DateTime.Now
                };

                await _ticketService.CrearTicketAsync(nuevoTicket);
                MessageBox.Show("Reporte creado exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear el reporte: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
