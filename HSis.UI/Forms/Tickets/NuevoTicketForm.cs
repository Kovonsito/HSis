using System.Runtime.Versioning;
using HSis.Data.Models;
using HSis.Logic.DTOs;
using HSis.Logic.Services;
using HSis.UI.Controls;
using HSis.UI.Helpers;

namespace HSis.UI.Forms.Tickets
{
    /// <summary>
    /// Formulario para crear y registrar nuevos tickets por parte de Clientes, Técnicos o Administradores.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public partial class NuevoTicketForm : Form
    {
        private readonly ITicketService _ticketService;
        private readonly IUsuarioService _usuarioService;
        private readonly ICatalogoService _catalogoService;
        private CajaTextoOrtograficaWpf rtbDescripcion;

        public NuevoTicketForm(
            ITicketService ticketService,
            IUsuarioService usuarioService,
            ICatalogoService catalogoService)
        {
            InitializeComponent();
            _ticketService = ticketService;
            _usuarioService = usuarioService;
            _catalogoService = catalogoService;
            InicializarLayoutNuevoTicket();
        }

        private void InicializarLayoutNuevoTicket()
        {
            // Inicializar el control de texto con corrección ortográfica
            rtbDescripcion = new CajaTextoOrtograficaWpf();

            bool esPerfilElevado = SesionSistema.EsAdmin || SesionSistema.EsTecnico;

            var tblPrincipal = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = esPerfilElevado ? 6 : 3,
                ColumnCount = 1,
                Padding = new Padding(15),
                Name = "tblPrincipal"
            };

            var pnlRepresentacion = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 1,
                ColumnCount = 2,
                Margin = new Padding(0, 0, 0, 10)
            };
            pnlRepresentacion.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pnlRepresentacion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            chkSolicitanteEnRepresentacion.Margin = new Padding(0, 5, 10, 0);
            txtNombreSolicitante.Dock = DockStyle.Fill;
            pnlRepresentacion.Controls.Add(chkSolicitanteEnRepresentacion, 0, 0);
            pnlRepresentacion.Controls.Add(txtNombreSolicitante, 1, 0);

            if (esPerfilElevado)
            {
                tblPrincipal.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Solicitante
                tblPrincipal.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Representación / Tercero
                tblPrincipal.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Prioridad / Técnico
                tblPrincipal.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Descripcion Label
                tblPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Descripcion Rtb
                tblPrincipal.RowStyles.Add(new RowStyle(SizeType.Absolute, 55F)); // Botones
            }
            else
            {
                tblPrincipal.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Descripcion Label
                tblPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Descripcion Rtb
                tblPrincipal.RowStyles.Add(new RowStyle(SizeType.Absolute, 55F)); // Botones
            }

            var flpBotones = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Margin = new Padding(0)
            };
            btnCancelar.Margin = new Padding(10, 10, 0, 10);
            btnGuardar.Margin = new Padding(0, 10, 0, 10);
            flpBotones.Controls.Add(btnCancelar);
            flpBotones.Controls.Add(btnGuardar);

            if (esPerfilElevado)
            {
                var pnlSolicitante = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    RowCount = 1,
                    ColumnCount = 2,
                    Margin = new Padding(0, 0, 0, 10)
                };
                pnlSolicitante.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                pnlSolicitante.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                lblSolicitante.Margin = new Padding(0, 5, 10, 0);
                cmbSolicitante.Dock = DockStyle.Fill;
                pnlSolicitante.Controls.Add(lblSolicitante, 0, 0);
                pnlSolicitante.Controls.Add(cmbSolicitante, 1, 0);

                var pnlCamposElevados = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    RowCount = 1,
                    ColumnCount = 4,
                    Margin = new Padding(0, 0, 0, 10)
                };
                pnlCamposElevados.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                pnlCamposElevados.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
                pnlCamposElevados.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                pnlCamposElevados.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));

                lblPrioridad.Margin = new Padding(0, 5, 10, 0);
                cmbPrioridad.Dock = DockStyle.Fill;
                lblTecnico.Margin = new Padding(15, 5, 10, 0);
                cmbTecnico.Dock = DockStyle.Fill;

                pnlCamposElevados.Controls.Add(lblPrioridad, 0, 0);
                pnlCamposElevados.Controls.Add(cmbPrioridad, 1, 0);
                pnlCamposElevados.Controls.Add(lblTecnico, 2, 0);
                pnlCamposElevados.Controls.Add(cmbTecnico, 3, 0);

                lblDescripcion.Dock = DockStyle.Fill;
                lblDescripcion.Margin = new Padding(0, 0, 0, 5);
                rtbDescripcion.Dock = DockStyle.Fill;
                rtbDescripcion.Margin = new Padding(0, 0, 0, 10);

                tblPrincipal.Controls.Add(pnlSolicitante, 0, 0);
                tblPrincipal.Controls.Add(pnlRepresentacion, 0, 1);
                tblPrincipal.Controls.Add(pnlCamposElevados, 0, 2);
                tblPrincipal.Controls.Add(lblDescripcion, 0, 3);
                tblPrincipal.Controls.Add(rtbDescripcion, 0, 4);
                tblPrincipal.Controls.Add(flpBotones, 0, 5);
            }
            else
            {
                lblDescripcion.Dock = DockStyle.Fill;
                lblDescripcion.Margin = new Padding(0, 0, 0, 5);
                rtbDescripcion.Dock = DockStyle.Fill;
                rtbDescripcion.Margin = new Padding(0, 0, 0, 10);

                tblPrincipal.Controls.Add(lblDescripcion, 0, 0);
                tblPrincipal.Controls.Add(rtbDescripcion, 0, 1);
                tblPrincipal.Controls.Add(flpBotones, 0, 2);
            }

            this.Controls.Clear();
            this.Controls.Add(tblPrincipal);
        }

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

            if (SesionSistema.EsAdmin || SesionSistema.EsTecnico)
            {
                await CargarCatalogosParaPerfilElevadoAsync();
            }
        }

        private async Task CargarCatalogosParaPerfilElevadoAsync()
        {
            try
            {
                cmbPrioridad.Items.Clear();
                cmbPrioridad.Items.Add(new ElementoCombo<string?>("Baja", "Baja"));
                cmbPrioridad.Items.Add(new ElementoCombo<string?>("Media", "Media"));
                cmbPrioridad.Items.Add(new ElementoCombo<string?>("Alta", "Alta"));
                cmbPrioridad.SelectedIndex = 0; // Prioridad Baja por defecto

                var todosUsuarios = await _catalogoService.ObtenerTodosAsync<Usuario>();

                // Cargar Solicitantes (Solo usuarios con IdRol == 3)
                cmbSolicitante.Items.Clear();
                var clientes = todosUsuarios.Where(u => u.IdRol == 3).OrderBy(u => u.Nombre);
                foreach (var u in clientes)
                {
                    string label = string.IsNullOrWhiteSpace(u.Nombre) ? $"Usuario #{u.IdUsuario}" : u.Nombre;
                    if (u.IdDepartamentoNavigation != null && !string.IsNullOrWhiteSpace(u.IdDepartamentoNavigation.Nombre))
                    {
                        label += $" ({u.IdDepartamentoNavigation.Nombre})";
                    }
                    cmbSolicitante.Items.Add(new ElementoCombo<int>(label, u.IdUsuario));
                }

                // Seleccionar al usuario actual por defecto en solicitante si está en la lista
                var elementoUsuarioActual = cmbSolicitante.Items.OfType<ElementoCombo<int>>().FirstOrDefault(x => x.Valor == SesionSistema.IdUsuario);
                if (elementoUsuarioActual != null)
                {
                    cmbSolicitante.SelectedItem = elementoUsuarioActual;
                }
                else if (cmbSolicitante.Items.Count > 0)
                {
                    cmbSolicitante.SelectedIndex = 0;
                }

                // Cargar Técnicos (Usuarios con IdRol = 2 ó 1)
                cmbTecnico.Items.Clear();
                cmbTecnico.Items.Add(new ElementoCombo<int?>("-- Sin Asignar --", null));

                var tecnicos = todosUsuarios.Where(u => u.IdRol == 2 || u.IdRol == 1).OrderBy(u => u.Nombre);
                foreach (var t in tecnicos)
                {
                    string labelTecnico = t.Nombre ?? $"Técnico #{t.IdUsuario}";
                    cmbTecnico.Items.Add(new ElementoCombo<int?>(labelTecnico, t.IdUsuario));
                }

                if (SesionSistema.EsTecnico)
                {
                    // Si quien registra es técnico, seleccionarse a sí mismo y bloquear la selección
                    var propioTecnico = cmbTecnico.Items.OfType<ElementoCombo<int?>>().FirstOrDefault(x => x.Valor == SesionSistema.IdUsuario);
                    if (propioTecnico != null)
                    {
                        cmbTecnico.SelectedItem = propioTecnico;
                    }
                    else
                    {
                        cmbTecnico.SelectedIndex = 0;
                    }
                    cmbTecnico.Enabled = false; // Bloquear selección ya que lo atiende el mismo técnico
                }
                else
                {
                    cmbTecnico.SelectedIndex = 0;
                    cmbTecnico.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al cargar catálogos para el registro: {ex.Message}",
                    "Error de Carga", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnGuardar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (chkSolicitanteEnRepresentacion.Checked && string.IsNullOrWhiteSpace(txtNombreSolicitante.Text))
                {
                    MessageBox.Show("Por favor, ingrese el nombre de la persona que solicitó la atención.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNombreSolicitante.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(rtbDescripcion.Text))
                {
                    MessageBox.Show("Por favor, ingrese una descripción del problema.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    rtbDescripcion.Focus();
                    return;
                }

                int idUsuarioSolicitante = SesionSistema.IdUsuario;
                int? idTecnicoAsignado = null;
                string? prioridad = null;

                if (SesionSistema.EsAdmin || SesionSistema.EsTecnico)
                {
                    if (!chkSolicitanteEnRepresentacion.Checked && cmbSolicitante.SelectedItem is ElementoCombo<int> selSolicitante)
                    {
                        idUsuarioSolicitante = selSolicitante.Valor;
                    }

                    if (cmbPrioridad.SelectedItem is ElementoCombo<string?> selPrioridad)
                    {
                        prioridad = selPrioridad.Valor;
                    }

                    if (cmbTecnico.SelectedItem is ElementoCombo<int?> selTecnico)
                    {
                        idTecnicoAsignado = selTecnico.Valor;
                    }
                }

                string descripcionFinal = rtbDescripcion.Text.Trim();
                if (chkSolicitanteEnRepresentacion.Checked && !string.IsNullOrWhiteSpace(txtNombreSolicitante.Text))
                {
                    descripcionFinal = $"[Solicitante no registrado: {txtNombreSolicitante.Text.Trim()}]\r\n\r\n{descripcionFinal}";
                }

                var nuevoTicketDto = new TicketCreateDto
                {
                    IdUsuario = idUsuarioSolicitante,
                    Descripcion = descripcionFinal,
                    IdTecnico = idTecnicoAsignado,
                    Prioridad = prioridad
                };

                var ticketGuardado = await _ticketService.CrearTicketAsync(nuevoTicketDto);

                MessageBox.Show($"Ticket registrado exitosamente con Folio: {ticketGuardado.IdTicket}",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
                MessageBox.Show($"Error al registrar el ticket: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
