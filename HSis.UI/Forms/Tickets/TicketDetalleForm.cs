#nullable enable
using System.Runtime.Versioning;
using HSis.Contracts.Constants;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.UI.Controls;
using HSis.UI.Helpers;

namespace HSis.UI.Forms.Tickets
{
    [SupportedOSPlatform("windows")]
    public partial class TicketDetalleForm : Form
    {
        private readonly int _idTicket;
        private readonly ITicketService _ticketService;
        private readonly ITicketDetalleService _ticketDetalleService;
        private readonly IUsuarioService _usuarioService;
        private readonly IAdministradorSesionUsuario _contextoSesion;

        private TicketDto? _ticketActual;
        private CajaTextoOrtograficaWpf rtbDescripcion = null!;
        private CajaTextoOrtograficaWpf rtbSolucion = null!;
        private DataGridView dgvMateriales = null!;

        public TicketDetalleForm(
            int idTicket,
            ITicketService ticketService,
            ITicketDetalleService ticketDetalleService,
            IUsuarioService usuarioService,
            IAdministradorSesionUsuario contextoSesion)
        {
            InitializeComponent();
            _idTicket = idTicket;
            _ticketService = ticketService;
            _ticketDetalleService = ticketDetalleService;
            _usuarioService = usuarioService;
            _contextoSesion = contextoSesion;

            InicializarLayoutDetalle();
        }

        public void MostrarTicket(TicketDto ticket)
        {
            _ticketActual = ticket;
            lblFolio.Text = $"Folio: TK-{ticket.IdTicket:d6}";
            txtUsuario.Text = ticket.NombreUsuario;
            txtDepartamento.Text = ticket.DepartamentoUsuario;
            txtAlta.Text = (ticket.FechaAlta ?? DateTime.Now).ToString("dddd, dd 'de' MMMM 'de' yyyy 'a las' HH:mm:ss");
            rtbDescripcion.Text = ticket.Descripcion ?? string.Empty;
            rtbSolucion.Text = ticket.Solucion ?? string.Empty;

            ConfigurarFecha(txtAtencion, ticket.FechaAtencion);
            ConfigurarFecha(txtCierre, ticket.FechaCierre);

            txtAlta.ReadOnly = true;
            txtAtencion.ReadOnly = true;
            txtCierre.ReadOnly = true;

            cmbPrioridad.SelectedItem = ticket.Prioridad;

            bool esAdmin = _contextoSesion.EsAdmin;
            bool esPropietario = ticket.IdTecnico == _contextoSesion.IdUsuario;
            string estatusActual = ticket.Estatus ?? ConstantesEstatus.ABIERTO;

            bool esSoloLectura = (!esAdmin && estatusActual == ConstantesEstatus.CERRADO) ||
                                 (!esAdmin && !esPropietario && ticket.IdTecnico != null);

            if (esSoloLectura)
            {
                cmbEstatus.Enabled = false;
                rtbSolucion.SoloLectura = true;
                rtbDescripcion.SoloLectura = true;
                btnGuardar.Enabled = false;
                cmbPrioridad.Enabled = false;
            }

            MostrarSeccionFeedback(ticket);
        }

        public void CargarEstatusPermitidos(List<string> estatusPermitidos, string estatusActual)
        {
            cmbEstatus.SelectedIndexChanged -= CmbEstatus_SelectedIndexChanged;
            cmbEstatus.Items.Clear();
            foreach (var estatus in estatusPermitidos)
            {
                cmbEstatus.Items.Add(estatus);
            }
            cmbEstatus.SelectedItem = estatusActual;
            cmbEstatus.SelectedIndexChanged += CmbEstatus_SelectedIndexChanged;
        }

        public void CargarTecnicos(List<UsuarioDto> tecnicos, int? idTecnicoActual, bool esAdmin)
        {
            cmbAtendido.DisplayMember = "Nombre";
            cmbAtendido.ValueMember = "IdUsuario";
            cmbAtendido.DataSource = tecnicos;

            cmbAtendido.SelectedValue = (object?)idTecnicoActual ?? -1;
            if (!esAdmin)
            {
                cmbAtendido.Enabled = false;
            }
        }

        public void CargarHistorial(List<HistorialCambiosDto> historial)
        {
            dgvHistorial.DataSource = null;
            dgvHistorial.DataSource = historial;
            ConfigurarEstilosGridHistorial();
        }

        public void CargarDetallesMaterial(List<TicketDetalleDto> detalles)
        {
            dgvMateriales.DataSource = null;
            dgvMateriales.DataSource = detalles;
            ConfigurarEstilosGridMateriales();
        }

        #region Form Handlers & Layout

        private async void FormularioTicket_Load(object? sender, EventArgs e)
        {
            await CargarTicketDetallesAsync(_idTicket);
        }

        private async Task CargarTicketDetallesAsync(int idTicket)
        {
            await this.EjecutarOperacionAsync(async () =>
            {
                var ticket = await _ticketService.ObtenerTicketPorIdAsync(idTicket);
                if (ticket == null)
                {
                    DialogoUIHelper.MostrarAdvertencia("Ticket no encontrado.");
                    this.Close();
                    return;
                }

                MostrarTicket(ticket);

                string estatusActual = ticket.Estatus ?? ConstantesEstatus.ABIERTO;
                var estatusPermitidos = ReglasEstatusTicket.ObtenerEstatusPermitidos(_contextoSesion.IdRolUsuario, estatusActual);
                CargarEstatusPermitidos(estatusPermitidos, estatusActual);

                var tecnicos = await _usuarioService.ObtenerUsuariosPorRolAsync((int)RolUsuarioEnum.Tecnico);
                var admins = await _usuarioService.ObtenerUsuariosPorRolAsync((int)RolUsuarioEnum.Administrador);
                var personalAtencion = tecnicos.Concat(admins).OrderBy(u => u.Nombre).ToList();
                CargarTecnicos(personalAtencion, ticket.IdTecnico, _contextoSesion.EsAdmin);

                await RecargarHistorialYMaterialesAsync(idTicket);
            }, "Error al cargar ticket");
        }

        public async Task RecargarHistorialYMaterialesAsync(int idTicket)
        {
            await this.EjecutarOperacionAsync(async () =>
            {
                var historial = await _ticketService.ObtenerHistorialPorTicketAsync(idTicket);
                CargarHistorial(historial);

                var detalles = await _ticketDetalleService.ObtenerDetallesTicketAsync(idTicket);
                CargarDetallesMaterial(detalles);
            },
            mensajeErrorContexto: $"Error al recargar historial y materiales del ticket {idTicket}");
        }

        private void CmbEstatus_SelectedIndexChanged(object? sender, EventArgs e)
        {
            string? estatusSeleccionado = cmbEstatus.SelectedItem?.ToString();
            if (estatusSeleccionado == ConstantesEstatus.EN_PROCESO && cmbAtendido.SelectedIndex == -1)
            {
                cmbAtendido.SelectedValue = _contextoSesion.IdUsuario;
            }
            else if (estatusSeleccionado == ConstantesEstatus.ABIERTO)
            {
                cmbAtendido.SelectedValue = -1;
            }
        }

        private async void btnGuardar_Click(object? sender, EventArgs e)
        {
            if (_ticketActual == null) return;

            string estatusSeleccionado = cmbEstatus.SelectedItem?.ToString() ?? ConstantesEstatus.ABIERTO;
            int? idTecnico = cmbAtendido.SelectedValue != null ? (int?)cmbAtendido.SelectedValue : null;

            if (estatusSeleccionado == ConstantesEstatus.EN_PROCESO && idTecnico == null)
            {
                idTecnico = _contextoSesion.IdUsuario;
            }

            string solucionIngresada = rtbSolucion.Text ?? string.Empty;
            string prioridadSeleccionada = cmbPrioridad.SelectedItem?.ToString() ?? string.Empty;

            if (estatusSeleccionado == ConstantesEstatus.EN_PROCESO && string.IsNullOrWhiteSpace(prioridadSeleccionada))
            {
                DialogoUIHelper.MostrarAdvertencia("Es necesario seleccionar una prioridad para el ticket para poder guardarlo y cambiarlo a estatus 'En Proceso'.", "Prioridad requerida");
                cmbPrioridad.Focus();
                return;
            }

            bool huboCambios = _ticketActual.Estatus != estatusSeleccionado ||
                               _ticketActual.IdTecnico != idTecnico ||
                               (_ticketActual.Solucion ?? string.Empty) != solucionIngresada ||
                               (_ticketActual.Prioridad ?? string.Empty) != prioridadSeleccionada;

            if (!huboCambios)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
                return;
            }

            await this.EjecutarOperacionAsync(async () =>
            {
                var updateDto = new TicketUpdateDto
                {
                    IdTicket = _ticketActual.IdTicket,
                    Estatus = estatusSeleccionado,
                    IdTecnico = idTecnico,
                    Solucion = solucionIngresada,
                    FechaAtencion = _ticketActual.FechaAtencion,
                    FechaCierre = _ticketActual.FechaCierre,
                    Prioridad = prioridadSeleccionada
                };

                await _ticketService.ActualizarTicketAsync(updateDto);
                DialogoUIHelper.MostrarExito("Ticket actualizado correctamente.");
                this.DialogResult = DialogResult.OK;
                this.Close();
            },
            mensajeErrorContexto: "Error al actualizar ticket",
            controlesADeshabilitar: [btnGuardar, btnCancelar]);
        }

        private void btnCancelar_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ConfigurarFecha(TextBox txt, DateTime? fecha)
        {
            if (fecha.HasValue)
            {
                txt.Text = fecha.Value.ToString("dddd, dd 'de' MMMM 'de' yyyy 'a las' HH:mm:ss");
            }
            else
            {
                txt.Text = "Pendiente";
            }
        }

        private void MostrarSeccionFeedback(TicketDto ticket)
        {
            bool esCerrado = ticket.Estatus == ConstantesEstatus.CERRADO;
            grpFeedback.Visible = esCerrado;
            if (esCerrado && ticket.Calificacion.HasValue)
            {
                lblResumen.Text = $"Calificación: {new string('⭐', ticket.Calificacion.Value)} ({ticket.Calificacion}/5)";
                lblComentarioLectura.Text = string.IsNullOrWhiteSpace(ticket.ComentarioEvaluacion) ? "Sin comentarios." : ticket.ComentarioEvaluacion;
            }
        }

        private async void btnEnviarFeedback_Click(object? sender, EventArgs e)
        {
            if (cmbEstrellas.SelectedIndex < 0)
            {
                DialogoUIHelper.MostrarAdvertencia("Por favor seleccione una calificación.", "Calificación requerida");
                return;
            }

            int calificacion = cmbEstrellas.SelectedIndex + 1;
            string? comentario = string.IsNullOrWhiteSpace(txtComentario.Text) ? null : txtComentario.Text.Trim();

            await this.EjecutarOperacionAsync(async () =>
            {
                bool exito = await _ticketService.RegistrarCalificacionAsync(_idTicket, calificacion, comentario);
                if (exito)
                {
                    DialogoUIHelper.MostrarExito("¡Gracias por tu retroalimentación! La calificación fue registrada.");
                    await CargarTicketDetallesAsync(_idTicket);
                }
                else
                {
                    DialogoUIHelper.MostrarError("No se pudo registrar la calificación.");
                }
            }, "Error al enviar feedback", btnEnviar);
        }

        private void ConfigurarEstilosGridHistorial()
        {
            dgvHistorial.AutoGenerateColumns = true;
            dgvHistorial.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHistorial.AplicarTemaModerno();
        }

        private void ConfigurarEstilosGridMateriales()
        {
            dgvMateriales.AutoGenerateColumns = true;
            dgvMateriales.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMateriales.AplicarTemaModerno();
        }
        #endregion
    }
}
