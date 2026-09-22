#nullable enable
using System.Runtime.Versioning;
using HSis.Logic.Constants;
using HSis.Logic.DTOs;
using HSis.Logic.Services;
using HSis.UI.Controls;
using HSis.UI.Helpers;

using HSis.UI.Services.Coordinators;

namespace HSis.UI.Forms.Tickets
{
    [SupportedOSPlatform("windows")]
    public partial class NuevoTicketForm : Form
    {
        private readonly ITicketService _ticketService;
        private readonly IUsuarioService _usuarioService;
        private readonly IContextoSesion _contextoSesion;
        private CajaTextoOrtograficaWpf rtbDescripcion = null!;

        public NuevoTicketForm(
            ITicketService ticketService,
            IUsuarioService usuarioService,
            IUiSessionCoordinator sessionCoordinator)
        {
            InitializeComponent();
            _ticketService = ticketService;
            _usuarioService = usuarioService;
            _contextoSesion = sessionCoordinator.ContextoSesion;

            InicializarLayoutNuevoTicket();
        }

        public void CargarClientes(List<UsuarioDto> clientes, int idUsuarioSesion)
        {
            cmbSolicitante.Items.Clear();
            foreach (var u in clientes)
            {
                string label = string.IsNullOrWhiteSpace(u.Nombre) ? $"Usuario #{u.IdUsuario}" : u.Nombre;
                if (!string.IsNullOrWhiteSpace(u.DepartamentoNombre))
                {
                    label += $" ({u.DepartamentoNombre})";
                }
                cmbSolicitante.Items.Add(new ElementoCombo<int>(label, u.IdUsuario));
            }

            var elementoUsuarioActual = cmbSolicitante.Items.OfType<ElementoCombo<int>>().FirstOrDefault(x => x.Valor == idUsuarioSesion);
            if (elementoUsuarioActual != null)
            {
                cmbSolicitante.SelectedItem = elementoUsuarioActual;
            }
            else if (cmbSolicitante.Items.Count > 0)
            {
                cmbSolicitante.SelectedIndex = 0;
            }
        }

        public void CargarTecnicos(List<UsuarioDto> tecnicos, bool esTecnicoSesion, int idUsuarioSesion)
        {
            cmbTecnico.Items.Clear();
            cmbTecnico.Items.Add(new ElementoCombo<int?>("-- Sin Asignar --", null));

            foreach (var t in tecnicos)
            {
                string labelTecnico = t.Nombre ?? $"Técnico #{t.IdUsuario}";
                cmbTecnico.Items.Add(new ElementoCombo<int?>(labelTecnico, t.IdUsuario));
            }

            if (esTecnicoSesion)
            {
                var propioTecnico = cmbTecnico.Items.OfType<ElementoCombo<int?>>().FirstOrDefault(x => x.Valor == idUsuarioSesion);
                if (propioTecnico != null)
                {
                    cmbTecnico.SelectedItem = propioTecnico;
                }
                else
                {
                    cmbTecnico.SelectedIndex = 0;
                }
                cmbTecnico.Enabled = false;
            }
            else
            {
                cmbTecnico.SelectedIndex = 0;
                cmbTecnico.Enabled = true;
            }
        }

        public void CargarPrioridades()
        {
            cmbPrioridad.Items.Clear();
            cmbPrioridad.Items.Add(new ElementoCombo<string?>(ConstantesPrioridad.BAJA, ConstantesPrioridad.BAJA));
            cmbPrioridad.Items.Add(new ElementoCombo<string?>(ConstantesPrioridad.MEDIA, ConstantesPrioridad.MEDIA));
            cmbPrioridad.Items.Add(new ElementoCombo<string?>(ConstantesPrioridad.ALTA, ConstantesPrioridad.ALTA));
            cmbPrioridad.SelectedIndex = 0;
        }

        public void MostrarError(string titulo, string mensaje)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => MostrarError(titulo, mensaje)));
                return;
            }
            MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void MostrarExito(string mensaje)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => MostrarExito(mensaje)));
                return;
            }
            MessageBox.Show(mensaje, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void MostrarCargando(bool cargando)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => MostrarCargando(cargando)));
                return;
            }
            btnGuardar.Enabled = !cargando;
            this.UseWaitCursor = cargando;
        }

        #region UI Handlers

        private void chkSolicitanteEnRepresentacion_CheckedChanged(object? sender, EventArgs e)
        {
            txtNombreSolicitante.Enabled = chkSolicitanteEnRepresentacion.Checked;
            cmbSolicitante.Enabled = !chkSolicitanteEnRepresentacion.Checked;

            if (chkSolicitanteEnRepresentacion.Checked)
            {
                txtNombreSolicitante.Focus();
            }
            else
            {
                txtNombreSolicitante.Clear();
            }
        }

        private async void frmNuevoTicket_Load(object? sender, EventArgs e)
        {
            rtbDescripcion.Limpiar();
            if (_contextoSesion.EsAdmin || _contextoSesion.EsTecnico)
            {
                await CargarCatalogosAsync();
            }
        }

        private async Task CargarCatalogosAsync()
        {
            try
            {
                MostrarCargando(true);
                CargarPrioridades();

                var clientes = await _usuarioService.ObtenerUsuariosPorRolAsync((int)RolUsuarioEnum.Cliente);
                var tecnicos = await _usuarioService.ObtenerUsuariosPorRolAsync((int)RolUsuarioEnum.Tecnico);
                var admins = await _usuarioService.ObtenerUsuariosPorRolAsync((int)RolUsuarioEnum.Administrador);
                var personalAtencion = tecnicos.Concat(admins).OrderBy(u => u.Nombre).ToList();

                CargarClientes(clientes.OrderBy(u => u.Nombre).ToList(), _contextoSesion.IdUsuario);
                CargarTecnicos(personalAtencion, _contextoSesion.EsTecnico, _contextoSesion.IdUsuario);
            }
            catch (Exception ex)
            {
                MostrarError("Error de Carga", $"Ocurrió un error al cargar catálogos: {ex.Message}");
            }
            finally
            {
                MostrarCargando(false);
            }
        }

        private async void btnGuardar_Click(object? sender, EventArgs e)
        {
            if (chkSolicitanteEnRepresentacion.Checked && string.IsNullOrWhiteSpace(txtNombreSolicitante.Text))
            {
                MostrarError("Validación", "Por favor, ingrese el nombre de la persona que solicitó la atención.");
                return;
            }

            if (string.IsNullOrWhiteSpace(rtbDescripcion.Text))
            {
                MostrarError("Validación", "Por favor, ingrese una descripción del problema.");
                return;
            }

            try
            {
                MostrarCargando(true);

                int idUsuarioFinal = _contextoSesion.IdUsuario;
                int? idTecnicoFinal = null;
                string? prioridadFinal = null;

                if (_contextoSesion.EsAdmin || _contextoSesion.EsTecnico)
                {
                    if (!chkSolicitanteEnRepresentacion.Checked && cmbSolicitante.SelectedItem is ElementoCombo<int> selSolicitante)
                    {
                        idUsuarioFinal = selSolicitante.Valor;
                    }
                    if (cmbPrioridad.SelectedItem is ElementoCombo<string?> selPrioridad)
                    {
                        prioridadFinal = selPrioridad.Valor;
                    }
                    if (cmbTecnico.SelectedItem is ElementoCombo<int?> selTecnico)
                    {
                        idTecnicoFinal = selTecnico.Valor;
                    }
                }

                string descripcionFinal = rtbDescripcion.Text.Trim();
                if (chkSolicitanteEnRepresentacion.Checked && !string.IsNullOrWhiteSpace(txtNombreSolicitante.Text))
                {
                    descripcionFinal = $"[Solicitante no registrado: {txtNombreSolicitante.Text.Trim()}]\r\n\r\n{descripcionFinal}";
                }

                var nuevoTicketDto = new TicketCreateDto
                {
                    IdUsuario = idUsuarioFinal,
                    Descripcion = descripcionFinal,
                    IdTecnico = idTecnicoFinal,
                    Prioridad = prioridadFinal
                };

                var ticketGuardado = await _ticketService.CrearTicketAsync(nuevoTicketDto);
                MostrarExito($"Ticket registrado exitosamente con Folio: TK-{ticketGuardado.IdTicket:d6}");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (FluentValidation.ValidationException ex)
            {
                string errores = string.Join("\n", ex.Errors.Select(err => "- " + err.ErrorMessage));
                MostrarError("Validación", $"Datos inválidos:\n{errores}");
            }
            catch (Exception ex)
            {
                MostrarError("Error", $"Error al registrar el ticket: {ex.Message}");
            }
            finally
            {
                MostrarCargando(false);
            }
        }

        private void btnCancelar_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        #endregion
    }
}
