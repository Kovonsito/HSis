using HSis.Data.Models;
using HSis.Logic.Services;

namespace HSis.UI
{
    public partial class frmTicketDetalle : Form
    {
        private int _idTicket;
        private readonly TicketService _ticketService;
        private readonly UsuarioService _usuarioService;
        private HSis.Logic.DTOs.TicketDto? _ticketActual;

        public frmTicketDetalle(int idTicket, TicketService ticketService, UsuarioService usuarioService)
        {
            InitializeComponent();
            _idTicket = idTicket;
            _ticketService = ticketService;
            _usuarioService = usuarioService;

            CargarDialogoTicket();
        }

        private void CmbEstatus_SelectedIndexChanged(object? sender, EventArgs e)
        {
            // Si cambia a "En Proceso" y no hay nadie asignado, asignarlo al usuario de la sesión actual
            if (cmbEstatus.SelectedItem?.ToString() == ConstantesEstatus.EN_PROCESO && cmbAtendido.SelectedIndex == -1)
            {
                cmbAtendido.SelectedValue = SesionSistema.IdUsuario;
            }
        }

        private async void FormularioTicket_Load(object sender, EventArgs e)
        {
        }

        public async void CargarDialogoTicket()
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
                lblFolio.Text = $"Folio: TK-{_ticketActual.IdTicket:d6}";
                txtUsuario.Text = _ticketActual.NombreUsuario;
                dtpAlta.Value = _ticketActual.Alta ?? DateTime.Now;
                rtbDescripcion.Text = _ticketActual.Descripcion;
                rtbSolucion.Text = _ticketActual.Solucion ?? string.Empty;

                if (_ticketActual.Atencion.HasValue)
                {
                    dtpAtencion.Value = _ticketActual.Atencion.Value;
                    dtpAtencion.Format = DateTimePickerFormat.Custom;
                    dtpAtencion.CustomFormat = " dd/MM/yyyy HH:mm";
                }
                else
                {
                    dtpAtencion.Format = DateTimePickerFormat.Custom;
                    dtpAtencion.CustomFormat = " ";
                }

                if (_ticketActual.Cierre.HasValue)
                {
                    dtpCierre.Value = _ticketActual.Cierre.Value;
                    dtpCierre.Format = DateTimePickerFormat.Custom;
                    dtpCierre.CustomFormat = " dd/MM/yyyy HH:mm";
                }
                else
                {
                    dtpCierre.Format = DateTimePickerFormat.Custom;
                    dtpCierre.CustomFormat = " ";
                }

                // Bloquear siempre los datetimepickers para que las fechas sean 100% automáticas
                dtpAlta.Enabled = false;
                dtpAtencion.Enabled = false;
                dtpCierre.Enabled = false;

                bool esAdmin = SesionSistema.IdRolUsuario == 1;
                bool esPropietario = _ticketActual.IdTecnico == SesionSistema.IdUsuario;
                string estatusActual = _ticketActual.Status ?? ConstantesEstatus.ABIERTO;

                // Lógica del diccionario de opciones de estatus
                cmbEstatus.Items.Clear();

                var estatusPermitidos = _ticketService.ObtenerEstatusPermitidos(SesionSistema.IdRolUsuario, estatusActual);
                foreach (var estatus in estatusPermitidos)
                {
                    cmbEstatus.Items.Add(estatus);
                }

                cmbEstatus.SelectedItem = estatusActual;

                // Cargar combo de técnicos (usuarios con rol de Técnico o Administrador)
                // Roles: 1 = Admin, 2 = Técnico, 3 = Usuario
                await CargarTecnicosAsync();
                await CargarHistorialAsync();

                // 1. Asignar el técnico: Si el ticket ya tiene uno, se usa ese. Si no, se deja en blanco (libre).
                if (_ticketActual.IdTecnico != null)
                {
                    cmbAtendido.SelectedValue = _ticketActual.IdTecnico;
                }
                else
                {
                    cmbAtendido.SelectedIndex = -1; // Sin asignar
                }

                // 2. Control de permisos: Solo el administrador (Rol 1) puede cambiar el técnico asignado a otros.
                // Sin embargo, si es Técnico y el ticket está abierto y no tiene técnico, puede tomarlo.
                // Dejaremos el combobox de Atendido bloqueado para Técnicos porque la asignación será automática al cambiar estatus.
                if (!esAdmin)
                {
                    cmbAtendido.Enabled = false;
                }

                // 3. Edición de tickets ajenos o cerrados
                if (!esAdmin && !esPropietario && _ticketActual.IdTecnico != null)
                {
                    // Es un técnico viendo el ticket de otro técnico
                    cmbEstatus.Enabled = false;
                    rtbSolucion.ReadOnly = true;
                    rtbDescripcion.ReadOnly = true;
                    btnGuardar.Enabled = false;
                }

                if (estatusActual == ConstantesEstatus.CERRADO)
                {
                    if (!esAdmin)
                    {
                        cmbEstatus.Enabled = false;
                        rtbSolucion.ReadOnly = true;
                        btnGuardar.Enabled = false;
                    }
                }

                // Suscribir el evento de cambio de estatus al final para evitar auto-asignaciones accidentales al cargar
                cmbEstatus.SelectedIndexChanged += CmbEstatus_SelectedIndexChanged;
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

                string estatusSeleccionado = cmbEstatus.SelectedItem?.ToString() ?? ConstantesEstatus.ABIERTO;

                // Validación de cierre de ticket
                if (estatusSeleccionado == ConstantesEstatus.CERRADO && string.IsNullOrWhiteSpace(rtbSolucion.Text))
                {
                    MessageBox.Show("Debes ingresar una solución antes de poder cerrar el ticket.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int? idTecnico = cmbAtendido.SelectedValue != null ? (int?)cmbAtendido.SelectedValue : null;

                // Lógica de respaldo: Si el estatus es En Proceso y aún no hay técnico, asignamos al usuario actual
                if (estatusSeleccionado == ConstantesEstatus.EN_PROCESO && idTecnico == null)
                {
                    idTecnico = SesionSistema.IdUsuario;
                }

                string solucionIngresada = rtbSolucion.Text;

                // Validación: Evitar viajes a la BD y registros de historial innecesarios si nada cambió
                bool huboCambios = false;
                if (_ticketActual.Status != estatusSeleccionado) huboCambios = true;
                if (_ticketActual.IdTecnico != idTecnico) huboCambios = true;
                if ((_ticketActual.Solucion ?? string.Empty) != solucionIngresada) huboCambios = true;

                if (!huboCambios)
                {
                    MessageBox.Show("No se detectaron cambios en el ticket para guardar.", "Sin cambios", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Crear DTO de actualización
                var updateDto = new HSis.Logic.DTOs.TicketUpdateDto
                {
                    IdTicket = _ticketActual.IdTicket,
                    Status = estatusSeleccionado,
                    IdTecnico = idTecnico,
                    Solucion = solucionIngresada,
                    Atencion = _ticketActual.Atencion,
                    Cierre = _ticketActual.Cierre
                };

                // Lógica automática de fechas para KPIs
                if (updateDto.Status == ConstantesEstatus.REABIERTO)
                {
                    updateDto.Cierre = null;
                }
                else if (updateDto.Status == ConstantesEstatus.EN_PROCESO && updateDto.Atencion == null)
                {
                    updateDto.Atencion = DateTime.Now;
                }
                else if (updateDto.Status == ConstantesEstatus.CERRADO && updateDto.Cierre == null)
                {
                    updateDto.Cierre = DateTime.Now;
                }
                else if (updateDto.Status == ConstantesEstatus.ABIERTO)
                {
                    updateDto.Atencion = null;
                    updateDto.Cierre = null;
                }

                // Llamar al método de actualización
                await _ticketService.ActualizarTicketAsync(updateDto);

                MessageBox.Show("Ticket actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (FluentValidation.ValidationException ex)
            {
                string errores = string.Join("\n", ex.Errors.Select(e => "- " + e.ErrorMessage));
                MessageBox.Show($"Datos inválidos:\n{errores}", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
