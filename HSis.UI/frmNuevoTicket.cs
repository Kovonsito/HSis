using HSis.Data.Models;
using HSis.Logic.Services;

namespace HSis.UI
{
    /// <summary>
    /// Formulario para crear nuevos tickets por parte de usuarios regulares.
    /// Responsabilidad única: Capturar descripción del problema y guardarlo.
    /// </summary>
    public partial class frmNuevoTicket : Form
    {
        private readonly TicketService _ticketService;

        public frmNuevoTicket(TicketService ticketService)
        {
            InitializeComponent();
            _ticketService = ticketService;
        }

        private void frmNuevoTicket_Load(object sender, EventArgs e)
        {
            // Inicialización básica del formulario
            rtbDescripcion.Clear();
        }

        private async void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar que la descripción no esté vacía
                if (string.IsNullOrWhiteSpace(rtbDescripcion.Text))
                {
                    MessageBox.Show("Por favor, ingrese una descripción del problema.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    rtbDescripcion.Focus();
                    return;
                }

                // Crear instancia de DTO para la creación del ticket
                var nuevoTicketDto = new HSis.Logic.DTOs.TicketCreateDto
                {
                    IdUsuario = SesionSistema.IdUsuario,
                    Descripcion = rtbDescripcion.Text.Trim()
                };

                // Guardar el ticket en la base de datos
                var ticketGuardado = await _ticketService.CrearTicketAsync(nuevoTicketDto);

                MessageBox.Show($"Ticket creado exitosamente con Folio: {ticketGuardado.IdTicket}",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Retornar resultado positivo
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
                MessageBox.Show($"Error al crear el ticket: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
