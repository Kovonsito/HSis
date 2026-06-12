#nullable enable
using System.Runtime.Versioning;
using HSis.Logic.Services;
using HSis.Logic.DTOs;

namespace HSis.UI
{
    [SupportedOSPlatform("windows")]
    public partial class TicketDetalleForm : Form
    {
        private int _idTicket;
        private readonly ITicketService _ticketService;
        private readonly IUsuarioService _usuarioService;
        private TicketDto? _ticketActual;

        public TicketDetalleForm(int idTicket, ITicketService ticketService, IUsuarioService usuarioService)
        {
            InitializeComponent();
            _idTicket = idTicket;
            _ticketService = ticketService;
            _usuarioService = usuarioService;

            InicializarLayoutDetalle();
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
                txtAlta.Text = (ticket.Alta ?? DateTime.Now).ToString("dddd, dd 'de' MMMM 'de' yyyy 'a las' HH:mm:ss");
                rtbDescripcion.Text = ticket.Descripcion ?? string.Empty;
                rtbSolucion.Text = ticket.Solucion ?? string.Empty;

                ConfigurarFecha(txtAtencion, ticket.Atencion);
                ConfigurarFecha(txtCierre, ticket.Cierre);

                // Bloquear siempre los controles para que las fechas sean 100% automáticas
                txtAlta.ReadOnly = true;
                txtAtencion.ReadOnly = true;
                txtCierre.ReadOnly = true;

                bool esAdmin = SesionSistema.IdRolUsuario == 1;
                bool esPropietario = ticket.IdTecnico == SesionSistema.IdUsuario;
                string estatusActual = ticket.Status ?? ConstantesEstatus.ABIERTO;

                // Lógica del diccionario de opciones de estatus
                cmbEstatus.Items.Clear();

                var estatusPermitidos = TicketService.ObtenerEstatusPermitidos(SesionSistema.IdRolUsuario, estatusActual);
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

                string solucionIngresada = rtbSolucion.Text ?? string.Empty;
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

        private static void ConfigurarFecha(TextBox txt, DateTime? fecha)
        {
            if (fecha.HasValue)
            {
                txt.Text = fecha.Value.ToString("dddd, dd 'de' MMMM 'de' yyyy 'a las' HH:mm:ss");
            }
            else
            {
                txt.Text = string.Empty;
            }
        }

        private void MostrarSeccionFeedback(TicketDto ticket)
        {
            // La pestaña siempre debe estar visible
            if (!tabControlTicket.TabPages.Contains(tbpFeedback))
            {
                tabControlTicket.TabPages.Add(tbpFeedback);
            }

            bool esCliente = SesionSistema.IdRolUsuario == 3;
            bool esCerrado = ticket.Status == ConstantesEstatus.CERRADO;
            bool tieneCalificacion = ticket.Calificacion.HasValue;

            if (esCliente && esCerrado && !tieneCalificacion)
            {
                // Modo Edición: El cliente puede calificar el ticket cerrado
                lblEstrellas.Visible = true;
                cmbEstrellas.Visible = true;
                lblComentario.Visible = true;
                txtComentario.Visible = true;
                btnEnviar.Visible = true;

                lblResumen.Visible = false;
                lblComentarioLectura.Visible = false;

                cmbEstrellas.SelectedIndex = 4; // 5 estrellas por defecto
                txtComentario.Text = string.Empty;
            }
            else if (tieneCalificacion)
            {
                // Modo Lectura: Mostrar la calificación existente (para Cliente, Técnico o Admin)
                lblEstrellas.Visible = false;
                cmbEstrellas.Visible = false;
                lblComentario.Visible = false;
                txtComentario.Visible = false;
                btnEnviar.Visible = false;

                lblResumen.Visible = true;
                lblComentarioLectura.Visible = true;

                string estrellasStr = new string('⭐', ticket.Calificacion ?? 0);
                lblResumen.Text = $"Calificación recibida: {estrellasStr} ({ticket.Calificacion}/5)";
                lblComentarioLectura.Text = string.IsNullOrEmpty(ticket.ComentarioFeedback)
                    ? "El cliente no dejó comentarios."
                    : $"Comentario: \"{ticket.ComentarioFeedback}\"";
            }
            else
            {
                // Modo Sin Calificación: Mostrar mensaje informativo
                lblEstrellas.Visible = false;
                cmbEstrellas.Visible = false;
                lblComentario.Visible = false;
                txtComentario.Visible = false;
                btnEnviar.Visible = false;

                lblComentarioLectura.Visible = false;

                lblResumen.Visible = true;
                if (esCliente && !esCerrado)
                {
                    lblResumen.Text = "El ticket debe estar Cerrado para poder calificar el servicio.";
                }
                else
                {
                    lblResumen.Text = "Aún no tienes calificación/comentarios.";
                }
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

        private void InicializarLayoutDetalle()
        {
            // 1. Crear panel de botones inferior
            var flpBotones = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Margin = new Padding(0)
            };
            btnCancelar.Margin = new Padding(12, 10, 12, 10);
            btnGuardar.Margin = new Padding(0, 10, 0, 10);
            btnCancelar.Dock = DockStyle.None;
            btnGuardar.Dock = DockStyle.None;
            flpBotones.Controls.Add(btnCancelar);
            flpBotones.Controls.Add(btnGuardar);

            // Grid principal
            var tblPrincipal = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1,
                Size = this.ClientSize
            };
            tblPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tblPrincipal.RowStyles.Add(new RowStyle(SizeType.Absolute, 55F));

            this.Controls.Remove(tabControlTicket);
            this.Controls.Remove(btnGuardar);
            this.Controls.Remove(btnCancelar);

            tblPrincipal.Controls.Add(tabControlTicket, 0, 0);
            tblPrincipal.Controls.Add(flpBotones, 0, 1);
            this.Controls.Add(tblPrincipal);

            // 2. Pestaña: tabInfoGeneral
            var tblFields = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 6,
                ColumnCount = 4,
                Padding = new Padding(12),
                AutoScroll = true
            };
            tblFields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            tblFields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tblFields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            tblFields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            for (int i = 0; i < 5; i++)
            {
                tblFields.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            }
            tblFields.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            lblFolio.Dock = DockStyle.Fill;
            lblFolio.TextAlign = ContentAlignment.MiddleLeft;
            lblUsuario.Dock = DockStyle.Fill;
            lblUsuario.TextAlign = ContentAlignment.MiddleLeft;
            txtUsuario.Dock = DockStyle.Fill;
            lblDepartamento.Dock = DockStyle.Fill;
            lblDepartamento.TextAlign = ContentAlignment.MiddleLeft;
            txtDepartamento.Dock = DockStyle.Fill;
            lblEstatus.Dock = DockStyle.Fill;
            lblEstatus.TextAlign = ContentAlignment.MiddleLeft;
            cmbEstatus.Dock = DockStyle.Fill;
            lblPrioridad.Dock = DockStyle.Fill;
            lblPrioridad.TextAlign = ContentAlignment.MiddleLeft;
            cmbPrioridad.Dock = DockStyle.Fill;
            lblAlta.Dock = DockStyle.Fill;
            lblAlta.TextAlign = ContentAlignment.MiddleLeft;
            txtAlta.Dock = DockStyle.Fill;
            lblAtencion.Dock = DockStyle.Fill;
            lblAtencion.TextAlign = ContentAlignment.MiddleLeft;
            txtAtencion.Dock = DockStyle.Fill;
            lblCierre.Dock = DockStyle.Fill;
            lblCierre.TextAlign = ContentAlignment.MiddleLeft;
            txtCierre.Dock = DockStyle.Fill;
            lblAtendido.Dock = DockStyle.Fill;
            lblAtendido.TextAlign = ContentAlignment.MiddleLeft;
            cmbAtendido.Dock = DockStyle.Fill;

            tabInfoGeneral.Controls.Clear();
            tblFields.Controls.Add(lblFolio, 0, 0);
            tblFields.SetColumnSpan(lblFolio, 4);
            tblFields.Controls.Add(lblUsuario, 0, 1);
            tblFields.Controls.Add(txtUsuario, 1, 1);
            tblFields.Controls.Add(lblDepartamento, 2, 1);
            tblFields.Controls.Add(txtDepartamento, 3, 1);
            tblFields.Controls.Add(lblEstatus, 0, 2);
            tblFields.Controls.Add(cmbEstatus, 1, 2);
            tblFields.Controls.Add(lblPrioridad, 2, 2);
            tblFields.Controls.Add(cmbPrioridad, 3, 2);
            tblFields.Controls.Add(lblAlta, 0, 3);
            tblFields.Controls.Add(txtAlta, 1, 3);
            tblFields.Controls.Add(lblAtencion, 2, 3);
            tblFields.Controls.Add(txtAtencion, 3, 3);
            tblFields.Controls.Add(lblCierre, 0, 4);
            tblFields.Controls.Add(txtCierre, 1, 4);
            tblFields.Controls.Add(lblAtendido, 2, 4);
            tblFields.Controls.Add(cmbAtendido, 3, 4);
            tabInfoGeneral.Controls.Add(tblFields);

            // 3. Pestaña: tabDescripcionSolucion
            var tblTextos = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 4,
                ColumnCount = 1,
                Padding = new Padding(12)
            };
            tblTextos.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblTextos.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tblTextos.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblTextos.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            lblDescripcion.Dock = DockStyle.Fill;
            lblDescripcion.Margin = new Padding(0, 0, 0, 5);
            rtbDescripcion.Dock = DockStyle.Fill;
            rtbDescripcion.Margin = new Padding(0, 0, 0, 15);
            lblSolucion.Dock = DockStyle.Fill;
            lblSolucion.Margin = new Padding(0, 0, 0, 5);
            rtbSolucion.Dock = DockStyle.Fill;
            rtbSolucion.Margin = new Padding(0);

            tabDescripcionSolucion.Controls.Clear();
            tblTextos.Controls.Add(lblDescripcion, 0, 0);
            tblTextos.Controls.Add(rtbDescripcion, 0, 1);
            tblTextos.Controls.Add(lblSolucion, 0, 2);
            tblTextos.Controls.Add(rtbSolucion, 0, 3);
            tabDescripcionSolucion.Controls.Add(tblTextos);

            // 4. Pestaña: tbpFeedback
            var tblFeedback = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1,
                Padding = new Padding(12)
            };
            tblFeedback.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblFeedback.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblFeedback.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var flpEstrellas = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 10)
            };
            lblEstrellas.Margin = new Padding(0, 5, 10, 0);
            lblEstrellas.AutoSize = true;
            cmbEstrellas.Margin = new Padding(0);
            lblResumen.Margin = new Padding(0, 5, 0, 0);
            lblResumen.AutoSize = true;

            flpEstrellas.Controls.Add(lblEstrellas);
            flpEstrellas.Controls.Add(cmbEstrellas);
            flpEstrellas.Controls.Add(lblResumen);

            lblComentario.Margin = new Padding(0, 0, 0, 5);
            lblComentario.AutoSize = true;

            var tblComentarioInput = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 1,
                ColumnCount = 2,
                Margin = new Padding(0)
            };
            tblComentarioInput.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblComentarioInput.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));

            var pnlComentarioText = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0) };
            txtComentario.Dock = DockStyle.Fill;
            lblComentarioLectura.Dock = DockStyle.Fill;
            pnlComentarioText.Controls.Add(txtComentario);
            pnlComentarioText.Controls.Add(lblComentarioLectura);

            btnEnviar.Dock = DockStyle.Fill;
            btnEnviar.Margin = new Padding(10, 0, 0, 0);

            tblComentarioInput.Controls.Add(pnlComentarioText, 0, 0);
            tblComentarioInput.Controls.Add(btnEnviar, 1, 0);

            grpFeedback.Controls.Clear();
            tblFeedback.Controls.Add(flpEstrellas, 0, 0);
            tblFeedback.Controls.Add(lblComentario, 0, 1);
            tblFeedback.Controls.Add(tblComentarioInput, 0, 2);
            grpFeedback.Controls.Add(tblFeedback);
        }
    }
}
