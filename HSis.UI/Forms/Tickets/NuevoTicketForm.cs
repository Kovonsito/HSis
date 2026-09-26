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
    public partial class NuevoTicketForm : Form
    {
        private readonly ITicketService _ticketService;
        private readonly IUsuarioService _usuarioService;
        private readonly IAdministradorSesionUsuario _contextoSesion;
        private CajaTextoOrtograficaWpf rtbDescripcion = null!;

        public NuevoTicketForm(
            ITicketService ticketService,
            IUsuarioService usuarioService,
            IAdministradorSesionUsuario contextoSesion)
        {
            InitializeComponent();
            _ticketService = ticketService;
            _usuarioService = usuarioService;
            _contextoSesion = contextoSesion;

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
            await this.EjecutarOperacionAsync(async () =>
            {
                CargarPrioridades();

                var clientes = await _usuarioService.ObtenerUsuariosPorRolAsync((int)RolUsuarioEnum.Cliente);
                var tecnicos = await _usuarioService.ObtenerUsuariosPorRolAsync((int)RolUsuarioEnum.Tecnico);
                var admins = await _usuarioService.ObtenerUsuariosPorRolAsync((int)RolUsuarioEnum.Administrador);
                var personalAtencion = tecnicos.Concat(admins).OrderBy(u => u.Nombre).ToList();

                CargarClientes(clientes.OrderBy(u => u.Nombre).ToList(), _contextoSesion.IdUsuario);
                CargarTecnicos(personalAtencion, _contextoSesion.EsTecnico, _contextoSesion.IdUsuario);
            },
             mensajeErrorContexto: "Error al cargar catálogos",
             claveOperacion: "nuevo-ticket-catalogos");
        }

        private async void btnGuardar_Click(object? sender, EventArgs e)
        {
            if (chkSolicitanteEnRepresentacion.Checked && string.IsNullOrWhiteSpace(txtNombreSolicitante.Text))
            {
                DialogoUIHelper.MostrarAdvertencia("Por favor, ingrese el nombre de la persona que solicitó la atención.", "Validación requerida");
                txtNombreSolicitante.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(rtbDescripcion.Text))
            {
                DialogoUIHelper.MostrarAdvertencia("Por favor, ingrese una descripción del problema.", "Validación requerida");
                rtbDescripcion.Focus();
                return;
            }

            await this.EjecutarOperacionAsync(async () =>
            {
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
                DialogoUIHelper.MostrarExito($"Ticket registrado exitosamente con Folio: {ticketGuardado.IdTicket}");
                this.DialogResult = DialogResult.OK;
                this.Close();
            },
            mensajeErrorContexto: "Error al registrar el ticket",
            controlesADeshabilitar: [btnGuardar, btnCancelar]);
        }

        private void btnCancelar_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        #endregion
    }
}
