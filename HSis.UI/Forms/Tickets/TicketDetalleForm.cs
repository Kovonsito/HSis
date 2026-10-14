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
        private readonly IMaterialService _materialService;
        private readonly IUsuarioService _usuarioService;
        private readonly IAdministradorSesionUsuario _contextoSesion;

        private TicketDto? _ticketActual;
        private List<MaterialDto> _materialesCatalogo = [];
        private CajaTextoOrtograficaWpf rtbDescripcion = null!;
        private CajaTextoOrtograficaWpf rtbSolucion = null!;
        private DataGridView dgvMateriales = null!;

        public TicketDetalleForm(
            int idTicket,
            ITicketService ticketService,
            ITicketDetalleService ticketDetalleService,
            IMaterialService materialService,
            IUsuarioService usuarioService,
            IAdministradorSesionUsuario contextoSesion)
        {
            InitializeComponent();
            _idTicket = idTicket;
            _ticketService = ticketService;
            _ticketDetalleService = ticketDetalleService;
            _materialService = materialService;
            _usuarioService = usuarioService;
            _contextoSesion = contextoSesion;

            InicializarLayoutDetalle();
            VincularEventosMateriales();
        }

        public void MostrarTicket(TicketDto ticket)
        {
            _ticketActual = ticket;
            lblFolio.Text = $"Folio: {ticket.IdTicket.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
            txtUsuario.Text = string.IsNullOrWhiteSpace(ticket.NombreUsuario) ? "Sin nombre" : ticket.NombreUsuario;
            txtDepartamento.Text = string.IsNullOrWhiteSpace(ticket.DepartamentoUsuario) ? "Sin departamento" : ticket.DepartamentoUsuario;
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

            bool puedeModificarMateriales = !esSoloLectura &&
                                            (esAdmin || (ticket.IdTecnico == _contextoSesion.IdUsuario && estatusActual != ConstantesEstatus.CERRADO));
            pnlAccionesMateriales.Enabled = puedeModificarMateriales;

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
            cmbAtendido.DisplayMember = nameof(UsuarioDto.Nombre);
            cmbAtendido.ValueMember = nameof(UsuarioDto.IdUsuario);
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
            dgvMateriales.DataSource = new ListaVinculableOrdenable<TicketDetalleDto>(detalles);
            ConfigurarEstilosGridMateriales();
        }

        public void SeleccionarPestanaRetroalimentacion()
        {
            tabControlTicket.SelectedTab = tbpFeedback;
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

                await CargarCatalogoMaterialesAsync();
                await RecargarHistorialYMaterialesAsync(idTicket);
            }, "Error al cargar ticket", "ticket-detalle");
        }

        private async Task CargarCatalogoMaterialesAsync()
        {
            try
            {
                _materialesCatalogo = await _materialService.ObtenerMaterialesAsync();
                cmbMaterialSeleccion.SelectedIndexChanged -= CmbMaterialSeleccion_SelectedIndexChanged;
                cmbMaterialSeleccion.DataSource = _materialesCatalogo;
                cmbMaterialSeleccion.DisplayMember = nameof(MaterialDto.Nombre);
                cmbMaterialSeleccion.ValueMember = nameof(MaterialDto.IdMaterial);
                cmbMaterialSeleccion.SelectedIndex = -1;
                cmbMaterialSeleccion.SelectedIndexChanged += CmbMaterialSeleccion_SelectedIndexChanged;
                ActualizarEtiquetaStock();
            }
            catch (Exception ex)
            {
                DialogoUIHelper.MostrarAdvertencia($"No se pudieron cargar los materiales para selección: {ex.Message}");
            }
        }

        private void CmbMaterialSeleccion_SelectedIndexChanged(object? sender, EventArgs e)
        {
            ActualizarEtiquetaStock();
        }

        private void ActualizarEtiquetaStock()
        {
            if (cmbMaterialSeleccion.SelectedItem is MaterialDto mat)
            {
                lblStockDisponible.Text = $"Stock: {mat.StockActual} {mat.UnidadMedida}";
                lblStockDisponible.ForeColor = mat.StockActual > 0 ? TemaVisual.TextoSecundario : TemaVisual.Peligro;
                numCantidadMaterial.Maximum = Math.Max(1, mat.StockActual);
            }
            else
            {
                lblStockDisponible.Text = "Stock: -";
                lblStockDisponible.ForeColor = TemaVisual.TextoSecundario;
            }
        }

        private void VincularEventosMateriales()
        {
            btnAgregarMaterial.Click += async (s, e) => await AgregarMaterialConsumidoAsync();
            btnEliminarMaterial.Click += async (s, e) => await EliminarMaterialConsumidoAsync();
        }

        private async Task AgregarMaterialConsumidoAsync()
        {
            if (cmbMaterialSeleccion.SelectedItem is not MaterialDto mat)
            {
                DialogoUIHelper.MostrarAdvertencia("Por favor selecciona un material para agregar.", "Material requerido");
                return;
            }

            int cantidad = (int)numCantidadMaterial.Value;
            if (cantidad <= 0)
            {
                DialogoUIHelper.MostrarAdvertencia("La cantidad debe ser mayor a 0.", "Cantidad inválida");
                return;
            }

            if (cantidad > mat.StockActual)
            {
                DialogoUIHelper.MostrarAdvertencia($"Stock insuficiente para '{mat.Nombre}'. Stock disponible: {mat.StockActual}.", "Stock insuficiente");
                return;
            }

            await this.EjecutarOperacionAsync(async () =>
            {
                var dto = new TicketDetalleDto
                {
                    IdTicket = _idTicket,
                    IdMaterial = mat.IdMaterial,
                    Cantidad = cantidad
                };

                await _ticketDetalleService.AgregarMaterialATicketAsync(dto);
                DialogoUIHelper.MostrarExito($"Se agregaron {cantidad} {mat.UnidadMedida} de {mat.Nombre} al ticket.");
                numCantidadMaterial.Value = 1;
                await CargarCatalogoMaterialesAsync();
                await RecargarHistorialYMaterialesAsync(_idTicket);
            },
            mensajeErrorContexto: "Error al agregar material al ticket",
            claveOperacion: "ticket-agregar-material",
            controlesADeshabilitar: [btnAgregarMaterial, btnEliminarMaterial]);
        }

        private async Task EliminarMaterialConsumidoAsync()
        {
            if (dgvMateriales.CurrentRow?.DataBoundItem is not TicketDetalleDto detalle)
            {
                DialogoUIHelper.MostrarAdvertencia("Por favor selecciona una fila de material para retirar del ticket.", "Selección requerida");
                return;
            }

            string nombreMat = detalle.NombreMaterial ?? $"Material #{detalle.IdMaterial}";
            bool confirmar = DialogoUIHelper.Confirmar(
                $"¿Deseas retirar '{nombreMat}' (Cantidad: {detalle.Cantidad}) de este ticket?\nEl inventario se reincorporará automáticamente.",
                "Confirmar devolución de material");

            if (!confirmar) return;

            await this.EjecutarOperacionAsync(async () =>
            {
                await _ticketDetalleService.EliminarMaterialDeTicketAsync(_idTicket, detalle.IdMaterial);
                DialogoUIHelper.MostrarExito($"Se retiró el material '{nombreMat}' del ticket.");
                await CargarCatalogoMaterialesAsync();
                await RecargarHistorialYMaterialesAsync(_idTicket);
            },
            mensajeErrorContexto: "Error al retirar material del ticket",
            claveOperacion: "ticket-eliminar-material",
            controlesADeshabilitar: [btnAgregarMaterial, btnEliminarMaterial]);
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
            mensajeErrorContexto: $"Error al recargar historial y materiales del ticket {idTicket}",
            claveOperacion: "ticket-historial-materiales");
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
            claveOperacion: "ticket-actualizar",
            controlesADeshabilitar: [btnGuardar, btnCancelar]);
        }

        private void btnCancelar_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ConfigurarFecha(CajaTextoModerna txt, DateTime? fecha)
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

            // Este formulario es el detalle para personal de soporte. La captura
            // de la calificación corresponde exclusivamente a DetalleClienteForm.
            bool esPersonalSoporte = _contextoSesion.EsAdmin || _contextoSesion.EsTecnico;

            if (esPersonalSoporte)
            {
                grpFeedback.Visible = esCerrado;
                pnlFeedbackEstado.Visible = !esCerrado;

                // El técnico y el administrador solo consultan la valoración.
                lblEstrellas.Visible = false;
                cmbEstrellas.Visible = false;
                lblComentario.Visible = false;
                txtComentario.Visible = false;
                btnEnviar.Visible = false;
                lblResumen.Visible = esCerrado;
                lblComentarioLectura.Visible = esCerrado;

                if (!esCerrado)
                {
                    return;
                }

                if (ticket.Calificacion.HasValue)
                {
                    lblResumen.Text = $"Calificación: {new string('⭐', ticket.Calificacion.Value)} ({ticket.Calificacion}/5)";
                    lblComentarioLectura.Text = string.IsNullOrWhiteSpace(ticket.ComentarioEvaluacion)
                        ? "Sin comentarios."
                        : $"Comentario: {ticket.ComentarioEvaluacion}";
                }
                else
                {
                    lblResumen.Text = "Aún no hay una calificación registrada.";
                    lblComentarioLectura.Text = "El cliente todavía no ha calificado la atención.";
                }

                return;
            }

            grpFeedback.Visible = esCerrado;
            pnlFeedbackEstado.Visible = !esCerrado;

            if (!esCerrado)
            {
                return;
            }

            if (ticket.Calificacion.HasValue)
            {
                lblResumen.Text = $"Calificación: {new string('⭐', ticket.Calificacion.Value)} ({ticket.Calificacion}/5)";
                lblComentarioLectura.Text = string.IsNullOrWhiteSpace(ticket.ComentarioEvaluacion)
                    ? "Sin comentarios."
                    : ticket.ComentarioEvaluacion;
            }
            else
            {
                lblResumen.Text = "Aún no hay una calificación registrada.";
                lblComentarioLectura.Text = "Comparte tu experiencia para ayudarnos a mejorar la atención.";
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
            }, "Error al enviar feedback", "ticket-feedback", btnEnviar);
        }

        private void ConfigurarEstilosGridHistorial()
        {
            dgvHistorial.AutoGenerateColumns = true;
            dgvHistorial.AplicarTemaModerno();
        }

        private void ConfigurarEstilosGridMateriales()
        {
            dgvMateriales.AutoGenerateColumns = true;
            dgvMateriales.AplicarTemaModerno();

            if (dgvMateriales.Columns["IdTicket"] != null)
                dgvMateriales.Columns["IdTicket"]!.Visible = false;

            if (dgvMateriales.Columns["IdMaterial"] != null)
                dgvMateriales.Columns["IdMaterial"]!.Visible = false;

            if (dgvMateriales.Columns["NombreMaterial"] != null)
            {
                dgvMateriales.Columns["NombreMaterial"]!.HeaderText = "Material";
                dgvMateriales.Columns["NombreMaterial"]!.FillWeight = 40;
            }

            if (dgvMateriales.Columns["Cantidad"] != null)
            {
                dgvMateriales.Columns["Cantidad"]!.HeaderText = "Cantidad";
                dgvMateriales.Columns["Cantidad"]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvMateriales.Columns["Cantidad"]!.FillWeight = 15;
            }

            if (dgvMateriales.Columns["UnidadMedidaMaterial"] != null)
            {
                dgvMateriales.Columns["UnidadMedidaMaterial"]!.HeaderText = "U.M.";
                dgvMateriales.Columns["UnidadMedidaMaterial"]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvMateriales.Columns["UnidadMedidaMaterial"]!.FillWeight = 15;
            }

            if (dgvMateriales.Columns["CostoUnitarioAplicado"] != null)
            {
                dgvMateriales.Columns["CostoUnitarioAplicado"]!.HeaderText = "Costo Unit.";
                dgvMateriales.Columns["CostoUnitarioAplicado"]!.DefaultCellStyle.Format = "C2";
                dgvMateriales.Columns["CostoUnitarioAplicado"]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvMateriales.Columns["CostoUnitarioAplicado"]!.Visible = _contextoSesion.EsAdmin;
                dgvMateriales.Columns["CostoUnitarioAplicado"]!.FillWeight = 20;
            }
        }
        #endregion
    }
}
