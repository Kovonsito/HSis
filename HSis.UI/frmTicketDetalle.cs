#nullable enable
using System.Runtime.Versioning;
using HSis.Logic.Services;
using HSis.Logic.DTOs;

namespace HSis.UI
{
    [SupportedOSPlatform("windows")]
    public partial class frmTicketDetalle : Form
    {
        private int _idTicket;
        private readonly TicketService _ticketService;
        private readonly UsuarioService _usuarioService;
        private TicketDto? _ticketActual;

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
            string? estatusSeleccionado = cmbEstatus.SelectedItem?.ToString();
            if (estatusSeleccionado == ConstantesEstatus.EN_PROCESO && cmbAtendido.SelectedIndex == -1)
            {
                cmbAtendido.SelectedValue = SesionSistema.IdUsuario;
            }
            else if (estatusSeleccionado == ConstantesEstatus.ABIERTO)
            {
                cmbAtendido.SelectedValue = -1;
            }
        }

        private void FormularioTicket_Load(object? sender, EventArgs e)
        {
        }

        public async void CargarDialogoTicket()
        {
            try
            {
                // Cargar el ticket
                _ticketActual = await _ticketService.ObtenerTicketPorIdAsync(_idTicket);
                var ticket = _ticketActual;

                if (ticket == null)
                {
                    MessageBox.Show("Ticket no encontrado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                // Mostrar datos del ticket
                lblFolio.Text = $"Folio: TK-{ticket.IdTicket:d6}";
                txtUsuario.Text = ticket.NombreUsuario;
                txtDepartamento.Text = ticket.DepartamentoUsuario;
                dtpAlta.Value = ticket.Alta ?? DateTime.Now;
                rtbDescripcion.Text = ticket.Descripcion ?? string.Empty;
                rtbSolucion.Text = ticket.Solucion ?? string.Empty;

                ConfigurarFecha(dtpAtencion, ticket.Atencion);
                ConfigurarFecha(dtpCierre, ticket.Cierre);

                // Bloquear siempre los datetimepickers para que las fechas sean 100% automáticas
                dtpAlta.Enabled = false;
                dtpAtencion.Enabled = false;
                dtpCierre.Enabled = false;

                bool esAdmin = SesionSistema.IdRolUsuario == 1;
                bool esPropietario = ticket.IdTecnico == SesionSistema.IdUsuario;
                string estatusActual = ticket.Status ?? ConstantesEstatus.ABIERTO;

                // Lógica del diccionario de opciones de estatus
                cmbEstatus.Items.Clear();

                var estatusPermitidos = _ticketService.ObtenerEstatusPermitidos(SesionSistema.IdRolUsuario, estatusActual);
                foreach (var estatus in estatusPermitidos)
                {
                    cmbEstatus.Items.Add(estatus);
                }

                cmbEstatus.SelectedItem = estatusActual;

                // Seleccionar prioridad si existe
                cmbPrioridad.SelectedItem = ticket.Prioridad;

                // Cargar combo de técnicos (usuarios con rol de Técnico o Administrador)
                // Roles: 1 = Admin, 2 = Técnico, 3 = Usuario
                await CargarTecnicosAsync();
                await CargarHistorialAsync();

                // 1. Asignar el técnico: Si el ticket ya tiene uno, se usa ese. Si no, se deja en blanco (libre).
                cmbAtendido.SelectedValue = (object?)ticket.IdTecnico ?? -1;

                // 2. Control de permisos: Solo el administrador (Rol 1) puede cambiar el técnico asignado a otros.
                // Sin embargo, si es Técnico y el ticket está abierto y no tiene técnico, puede tomarlo.
                // Dejaremos el combobox de Atendido bloqueado para Técnicos porque la asignación será automática al cambiar estatus.
                if (!esAdmin)
                {
                    cmbAtendido.Enabled = false;
                }

                // 3. Edición de tickets ajenos o cerrados (Modo Lectura)
                bool esSoloLectura = (!esAdmin && estatusActual == ConstantesEstatus.CERRADO) ||
                                     (!esAdmin && !esPropietario && ticket.IdTecnico != null);

                if (esSoloLectura)
                {
                    cmbEstatus.Enabled = false;
                    rtbSolucion.ReadOnly = true;
                    rtbDescripcion.ReadOnly = true;
                    btnGuardar.Enabled = false;
                    cmbPrioridad.Enabled = false;
                }

                // Suscribir el evento de cambio de estatus al final para evitar auto-asignaciones accidentales al cargar
                cmbEstatus.SelectedIndexChanged += CmbEstatus_SelectedIndexChanged;

                MostrarSeccionFeedback(ticket);
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

        private async void btnGuardar_Click(object? sender, EventArgs e)
        {
            try
            {
                var ticket = _ticketActual;
                if (ticket == null)
                {
                    MessageBox.Show("Error: El ticket no está disponible.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string estatusSeleccionado = cmbEstatus.SelectedItem?.ToString() ?? ConstantesEstatus.ABIERTO;
                int? idTecnico = cmbAtendido.SelectedValue != null ? (int?)cmbAtendido.SelectedValue : null;

                // Lógica de respaldo: Si el estatus es En Proceso y aún no hay técnico, asignamos al usuario actual
                if (estatusSeleccionado == ConstantesEstatus.EN_PROCESO && idTecnico == null)
                {
                    idTecnico = SesionSistema.IdUsuario;
                }

                string solucionIngresada = rtbSolucion.Text;
                string prioridadSeleccionada = cmbPrioridad.SelectedItem?.ToString() ?? string.Empty;

                // Validación de cambios: Evitar viajes a la BD e historial innecesario si nada cambió
                bool huboCambios = ticket.Status != estatusSeleccionado ||
                                   ticket.IdTecnico != idTecnico ||
                                   (ticket.Solucion ?? string.Empty) != solucionIngresada ||
                                   (ticket.Prioridad ?? string.Empty) != prioridadSeleccionada;

                if (!huboCambios)
                {
                    MessageBox.Show("No se detectaron cambios en el ticket para guardar.", "Sin cambios", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Crear DTO de actualización (las validaciones de solución residen en la capa de negocio mediante TicketUpdateValidator)
                var updateDto = new HSis.Logic.DTOs.TicketUpdateDto
                {
                    IdTicket = ticket.IdTicket,
                    Status = estatusSeleccionado,
                    IdTecnico = idTecnico,
                    Solucion = solucionIngresada,
                    Atencion = ticket.Atencion,
                    Cierre = ticket.Cierre,
                    Prioridad = prioridadSeleccionada
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
                var rawHistorial = await _ticketService.ObtenerHistorialPorTicketAsync(_idTicket);
                var historial = rawHistorial.Cast<HistorialCambiosDto>().ToList();

                // Asignamos al Grid
                dgvHistorial.DataSource = new SortableBindingList<HistorialCambiosDto>(historial);

                // Limpieza visual: Asegurarnos de que no se auto-seleccione la primera fila
                dgvHistorial.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el historial: {ex.Message}", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCancelar_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private static void ConfigurarFecha(DateTimePicker dtp, DateTime? fecha)
        {
            if (fecha.HasValue)
            {
                dtp.Value = fecha.Value;
                dtp.Format = DateTimePickerFormat.Custom;
                dtp.CustomFormat = " dd/MM/yyyy HH:mm";
            }
            else
            {
                dtp.Format = DateTimePickerFormat.Custom;
                dtp.CustomFormat = " ";
            }
        }

        private void MostrarSeccionFeedback(TicketDto ticket)
        {
            bool mostrarFdb = false;
            bool esEditable = false;

            if (SesionSistema.IdRolUsuario == 3) // Cliente
            {
                if (ticket.Status == ConstantesEstatus.CERRADO)
                {
                    mostrarFdb = true;
                    esEditable = !ticket.Calificacion.HasValue;
                }
            }
            else // Admin o Técnico
            {
                if (ticket.Calificacion.HasValue)
                {
                    mostrarFdb = true;
                    esEditable = false;
                }
            }

            if (!mostrarFdb)
            {
                grpFeedback.Visible = false;
                // Restaurar tamaño original por si cambia de estado
                this.Height = 735;
                lblHistoria.Location = new Point(12, 438);
                dgvHistorial.Location = new Point(12, 456);
                dgvHistorial.Height = 231;
                return;
            }

            // Mostrar el GroupBox y aumentar tamaño del formulario
            grpFeedback.Visible = true;
            this.Height = 850;

            // Desplazar el historial hacia abajo
            lblHistoria.Location = new Point(12, 570);
            dgvHistorial.Location = new Point(12, 590);
            dgvHistorial.Height = 200;

            // Configurar los controles internos según modo edición o lectura
            if (esEditable)
            {
                // Mostrar controles de edición
                lblEstrellas.Visible = true;
                cmbEstrellas.Visible = true;
                lblComentario.Visible = true;
                txtComentario.Visible = true;
                btnEnviar.Visible = true;

                // Ocultar controles de lectura
                lblResumen.Visible = false;
                lblComentarioLectura.Visible = false;

                cmbEstrellas.SelectedIndex = 4; // 5 estrellas por defecto
                txtComentario.Text = string.Empty;
            }
            else
            {
                // Ocultar controles de edición
                lblEstrellas.Visible = false;
                cmbEstrellas.Visible = false;
                lblComentario.Visible = false;
                txtComentario.Visible = false;
                btnEnviar.Visible = false;

                // Mostrar controles de lectura
                lblResumen.Visible = true;
                lblComentarioLectura.Visible = true;

                string estrellasStr = new string('⭐', ticket.Calificacion ?? 0);
                lblResumen.Text = $"Calificación recibida: {estrellasStr} ({ticket.Calificacion}/5)";
                lblComentarioLectura.Text = string.IsNullOrEmpty(ticket.ComentarioFeedback)
                    ? "El cliente no dejó comentarios."
                    : $"Comentario: \"{ticket.ComentarioFeedback}\"";
            }
        }
        private async void btnEnviarFeedback_Click(object? sender, EventArgs e)
        {
            try
            {
                int calificacion = cmbEstrellas.SelectedIndex + 1;
                string comentario = txtComentario.Text.Trim();

                bool ok = await _ticketService.RegistrarCalificacionAsync(_idTicket, calificacion, comentario);
                if (ok)
                {
                    MessageBox.Show("¡Gracias por tu retroalimentación!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Recargar ticket
                    CargarDialogoTicket();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar feedback: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
