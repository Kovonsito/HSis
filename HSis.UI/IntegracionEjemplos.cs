using System;
using System.Collections.Generic;
using System.Text;

namespace HSis.UI.Ejemplos
{
    /// <summary>
    /// Ejemplos de integración de los nuevos formularios en los dashboards.
    /// Proporciona código listo para copiar en frmDashboardAdmin, frmDashboardTecnico, frmDashboardCliente.
    /// </summary>
    public static class IntegracionEjemplos
    {
        // =====================================================
        // EJEMPLO 1: Cómo abrir frmNuevoTicket desde cualquier rol
        // =====================================================
        // Lugar: En cualquier dashboard (Admin, Técnico, Cliente)
        // Evento: Click del botón "Nuevo Ticket"

        public static void AbrirNuevoTicket()
        {
            /*
            private void btnNuevoTicket_Click(object sender, EventArgs e)
            {
                using (var form = new frmNuevoTicket())
                {
                    var result = form.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        // Recargar la grilla de tickets
                        CargarTicketsAsync();
                        MessageBox.Show("Ticket creado exitosamente. Será visible en tu listado.", 
                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            */
        }

        // =====================================================
        // EJEMPLO 2: Cómo abrir frmDetalleCliente desde dashboard Cliente
        // =====================================================
        // Lugar: frmDashboardCliente.cs
        // Evento: Double-click en DataGridView de tickets del cliente

        public static void AbrirDetalleTicketCliente(int idTicket)
        {
            /*
            private void dgvTickets_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
            {
                if (e.RowIndex < 0) return; // Header click

                // Obtener el ID del ticket de la fila
                int idTicket = (int)dgvTickets.Rows[e.RowIndex].Cells["IdTicket"].Value;

                using (var form = new frmDetalleCliente(idTicket))
                {
                    form.ShowDialog();
                    // No hay acciones post-cierre porque es solo lectura
                }
            }
            */
        }

        // =====================================================
        // EJEMPLO 3: Cómo abrir frmTicket desde dashboard Admin/Técnico
        // =====================================================
        // Lugar: frmDashboardAdmin.cs o frmDashboardTecnico.cs
        // Evento: Double-click en DataGridView de tickets

        public static void AbrirEdicionTicket(int idTicket)
        {
            /*
            private void dgvTickets_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
            {
                if (e.RowIndex < 0) return; // Header click

                // Obtener el ID del ticket de la fila
                int idTicket = (int)dgvTickets.Rows[e.RowIndex].Cells["IdTicket"].Value;

                using (var form = new frmTicket(idTicket))
                {
                    var result = form.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        // El ticket fue actualizado, recargar la grilla
                        CargarTicketsAsync();
                        MessageBox.Show("Cambios guardados correctamente.", 
                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            */
        }

        // =====================================================
        // EJEMPLO 4: Lógica RBAC para mostrar/ocultar botones
        // =====================================================
        // Lugar: En el Load() de cada dashboard

        public static void ConfigurarBotonesPorRol()
        {
            /*
            private void ConfigurarPermisosRol()
            {
                // Todos pueden crear tickets
                btnNuevoTicket.Enabled = true;

                switch (SesionSistema.IdRolUsuario)
                {
                    case 1: // ADMINISTRADOR
                        btnEditar.Enabled = true;           // Puede editar tickets
                        btnAsignarTecnico.Enabled = true;   // Puede asignar técnicos
                        btnVerHistorial.Enabled = true;     // Puede ver historial completo
                        lblRolActual.Text = "ADMINISTRADOR - Acceso Completo";
                        break;

                    case 2: // TÉCNICO
                        btnEditar.Enabled = true;           // Puede editar tickets asignados
                        btnAsignarTecnico.Enabled = false;  // NO puede asignar (RBAC)
                        btnVerHistorial.Enabled = true;     // Puede ver historial
                        lblRolActual.Text = "TÉCNICO - Acceso Limitado";
                        break;

                    case 3: // CLIENTE
                        btnEditar.Enabled = false;          // NO puede editar
                        btnAsignarTecnico.Enabled = false;  // NO puede asignar
                        btnVerHistorial.Enabled = false;    // NO puede ver historial completo
                        lblRolActual.Text = "CLIENTE - Solo Consulta";
                        break;
                }
            }
            */
        }

        // =====================================================
        // EJEMPLO 5: Cargar solo tickets del cliente (Rol 3)
        // =====================================================
        // Lugar: frmDashboardCliente.cs

        public static void CargarTicketsDelCliente()
        {
            /*
            private async void CargarTicketsAsync()
            {
                try
                {
                    var ticketService = new TicketService();

                    // Cargar SOLO tickets del cliente actual
                    var tickets = await ticketService.ObtenerTicketsPorUsuarioAsync(
                        SesionSistema.IdUsuario);

                    dgvTickets.DataSource = tickets;

                    // Personalizar columnas
                    dgvTickets.Columns["IdTicket"].HeaderText = "Folio";
                    dgvTickets.Columns["Alta"].HeaderText = "Fecha Creación";
                    dgvTickets.Columns["Status"].HeaderText = "Estado";
                    dgvTickets.Columns["Descripción"].HeaderText = "Problema";
                    dgvTickets.Columns["Solución"].HeaderText = "Solución";

                    // Ocultar columnas innecesarias
                    dgvTickets.Columns["IdUsuario"].Visible = false;
                    dgvTickets.Columns["IdTecnico"].Visible = false;

                    lblTotalTickets.Text = $"Total: {tickets.Count} tickets";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cargar tickets: {ex.Message}", "Error");
                }
            }
            */
        }

        // =====================================================
        // EJEMPLO 6: Flujo completo para Cliente
        // =====================================================

        public static void FlujoCompletoCliente()
        {
            /*
            // EN frmDashboardCliente.cs

            public partial class frmDashboardCliente : Form
            {
                private TicketService _ticketService;

                private void frmDashboardCliente_Load(object sender, EventArgs e)
                {
                    ConfigurarPermisosRol();
                    CargarTicketsAsync();
                }

                private void btnNuevoTicket_Click(object sender, EventArgs e)
                {
                    // Abre frmNuevoTicket para crear un nuevo ticket
                    using (var form = new frmNuevoTicket())
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            CargarTicketsAsync();
                        }
                    }
                }

                private void dgvTickets_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
                {
                    if (e.RowIndex < 0) return;

                    int idTicket = (int)dgvTickets.Rows[e.RowIndex].Cells["IdTicket"].Value;

                    // Abre frmDetalleCliente para ver el ticket en modo lectura
                    using (var form = new frmDetalleCliente(idTicket))
                    {
                        form.ShowDialog();
                    }
                }
            }
            */
        }

        // =====================================================
        // EJEMPLO 7: Flujo completo para Técnico
        // =====================================================

        public static void FlujoCompletoTecnico()
        {
            /*
            // EN frmDashboardTecnico.cs

            public partial class frmDashboardTecnico : Form
            {
                private TicketService _ticketService;

                private void frmDashboardTecnico_Load(object sender, EventArgs e)
                {
                    ConfigurarPermisosRol();
                    CargarTicketsAsignadosAsync();
                }

                private void btnNuevoTicket_Click(object sender, EventArgs e)
                {
                    // Técnico también puede crear tickets propios
                    using (var form = new frmNuevoTicket())
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            CargarTicketsAsignadosAsync();
                        }
                    }
                }

                private async void CargarTicketsAsignadosAsync()
                {
                    // Cargar tickets asignados a este técnico
                    var tickets = await _ticketService
                        .ObtenerTicketsAsignadosATecnicoAsync(SesionSistema.IdUsuario);
                    dgvTickets.DataSource = tickets;
                }

                private void dgvTickets_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
                {
                    if (e.RowIndex < 0) return;

                    int idTicket = (int)dgvTickets.Rows[e.RowIndex].Cells["IdTicket"].Value;

                    // Abre frmTicket para editar
                    // Nota: cmbAtendido estará DESHABILITADO por RBAC
                    using (var form = new frmTicket(idTicket))
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            CargarTicketsAsignadosAsync();
                        }
                    }
                }
            }
            */
        }

        // =====================================================
        // EJEMPLO 8: Flujo completo para Admin
        // =====================================================

        public static void FlujoCompletoAdmin()
        {
            /*
            // EN frmDashboardAdmin.cs

            public partial class frmDashboardAdmin : Form
            {
                private TicketService _ticketService;

                private void frmDashboardAdmin_Load(object sender, EventArgs e)
                {
                    ConfigurarPermisosRol();
                    CargarTodosTicketsAsync();
                }

                private void btnNuevoTicket_Click(object sender, EventArgs e)
                {
                    using (var form = new frmNuevoTicket())
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            CargarTodosTicketsAsync();
                        }
                    }
                }

                private async void CargarTodosTicketsAsync()
                {
                    // Admin ve TODOS los tickets
                    var tickets = await _ticketService.ObtenerTicketsAsync();
                    dgvTickets.DataSource = tickets;
                }

                private void dgvTickets_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
                {
                    if (e.RowIndex < 0) return;

                    int idTicket = (int)dgvTickets.Rows[e.RowIndex].Cells["IdTicket"].Value;

                    // Abre frmTicket para edición completa
                    // Nota: cmbAtendido estará HABILITADO para asignar técnicos
                    using (var form = new frmTicket(idTicket))
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            CargarTodosTicketsAsync();
                        }
                    }
                }
            }
            */
        }

        // =====================================================
        // EJEMPLO 9: Validaciones de seguridad (Servidor)
        // =====================================================

        public static void ValidacionesSeguridad()
        {
            /*
            // RECOMENDACIÓN: Agregar validaciones en el servidor (TicketService)

            // En TicketService.cs:

            public async Task ActualizarTicketConHistorialAsyncSeguro(
                Ticket ticketEditado, 
                int idUsuarioModifica,
                int rolUsuario)
            {
                // Validar que el usuario tenga permiso
                if (rolUsuario != 1 && rolUsuario != 2)
                {
                    throw new UnauthorizedAccessException(
                        "Solo Admins y Técnicos pueden editar tickets.");
                }

                // Si es Técnico, validar que solo edite tickets asignados a él
                if (rolUsuario == 2)
                {
                    var ticketOriginal = await db.Tickets
                        .FirstOrDefaultAsync(t => t.IdTicket == ticketEditado.IdTicket);

                    if (ticketOriginal.IdTecnico != idUsuarioModifica)
                    {
                        throw new UnauthorizedAccessException(
                            "Solo puedes editar tickets asignados a ti.");
                    }
                }

                // Si es Técnico, NO permitir cambiar IdTecnico
                if (rolUsuario == 2)
                {
                    var ticketOriginal = await db.Tickets
                        .AsNoTracking()
                        .FirstOrDefaultAsync(t => t.IdTicket == ticketEditado.IdTicket);

                    if (ticketOriginal.IdTecnico != ticketEditado.IdTecnico)
                    {
                        throw new UnauthorizedAccessException(
                            "No tienes permisos para cambiar el técnico asignado.");
                    }
                }

                // Si todo está bien, proceder con actualización
                await ActualizarTicketConHistorialAsync(ticketEditado, idUsuarioModifica);
            }
            */
        }
    }
}
