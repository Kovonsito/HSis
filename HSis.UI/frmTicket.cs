using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using HSis.Data.Models;
using HSis.Logic.Services;

namespace HSis.UI
{
    public partial class frmTicket : Form
    {
        private int _idTicket;
        private readonly TicketService _ticketService;
        private readonly UsuarioService _usuarioService;
        private Ticket? _ticketActual;

        public frmTicket(int idTicket)
        {
            InitializeComponent();
            _idTicket = idTicket;
            _ticketService = new TicketService();
            _usuarioService = new UsuarioService();
        }

        private async void FormularioTicket_Load(object sender, EventArgs e)
        {
            try
            {
                // Cargar el ticket
                _ticketActual = await _ticketService.ObtenerTicketPorIdAsync(_idTicket);

                if (_ticketActual == null)
                {
                    MessageBox.Show("Ticket no encontrado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                // Mostrar datos del ticket
                lblFolio.Text = $"Folio: {_ticketActual.IdTicket}";
                rtbDescripcion.Text = _ticketActual.Descripción;
                txtUsuario.Text = _ticketActual.IdUsuarioNavigation?.Nombre ?? "Desconocido";
                cmbEstatus.SelectedItem = _ticketActual.Status ?? ConstantesEstatus.ABIERTO;
                dtpAlta.Value = _ticketActual.Alta ?? DateTime.Now;
                rtbSolucion.Text = _ticketActual.Solución ?? string.Empty;

                // Cargar combo de técnicos (usuarios con rol de Técnico o Administrador)
                // Roles: 1 = Admin, 2 = Técnico, 3 = Usuario
                await CargarTecnicosAsync();
                await CargarHistorialAsync();

                // Seleccionar el técnico asignado actual (si existe)
                if (_ticketActual.IdTecnico.HasValue)
                {
                    cmbAtendido.SelectedValue = _ticketActual.IdTecnico.Value;
                }

                // Aplicar RBAC: si el usuario no es Admin (IdRol != 1), deshabilitar el combo de técnicos
                if (SesionSistema.IdRolUsuario != 1) // 1 = Admin
                {
                    cmbAtendido.Enabled = false;
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el ticket: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private async Task CargarTecnicosAsync()
        {
            try
            {
                // Obtener técnicos (IdRol = 1 para Admin o IdRol = 2 para Técnico)
                var admins = await _usuarioService.ObtenerUsuariosPorRolAsync(1);
                var tecnicos = await _usuarioService.ObtenerUsuariosPorRolAsync(2);

                var todosTecnicos = admins.Concat(tecnicos).ToList();

                cmbAtendido.DisplayMember = "Nombre";
                cmbAtendido.ValueMember = "IdUsuario";
                cmbAtendido.DataSource = todosTecnicos;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar técnicos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_ticketActual == null)
                {
                    MessageBox.Show("Error: El ticket no está disponible.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Actualizar los campos editables del ticket
                _ticketActual.Status = cmbEstatus.SelectedItem?.ToString() ?? ConstantesEstatus.ABIERTO;
                _ticketActual.IdTecnico = cmbAtendido.SelectedValue != null ? (int)cmbAtendido.SelectedValue : null;
                _ticketActual.Solución = rtbSolucion.Text;

                // Lógica automática de fechas para KPIs
                if (_ticketActual.Status == ConstantesEstatus.EN_PROCESO && _ticketActual.Atención == null)
                {
                    _ticketActual.Atención = DateTime.Now;
                }
                else if (_ticketActual.Status == ConstantesEstatus.CERRADO && _ticketActual.Cierre == null)
                {
                    _ticketActual.Cierre = DateTime.Now;
                }

                // Llamar al método de auditoría que registra los cambios en el historial
                await _ticketService.ActualizarTicketConHistorialAsync(_ticketActual, SesionSistema.IdUsuario);

                MessageBox.Show("Ticket actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Retornar DialogResult.OK para que el Dashboard recargue los datos
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el ticket: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task CargarHistorialAsync()
        {
            try
            {
                // Llamamos a la lógica asíncrona
                var historial = await _ticketService.ObtenerHistorialPorTicketAsync(_idTicket);

                // Asignamos al Grid
                dgvHistorial.DataSource = historial;

                // Limpieza visual: Asegurarnos de que no se auto-seleccione la primera fila
                dgvHistorial.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el historial: {ex.Message}", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }


    }
}
