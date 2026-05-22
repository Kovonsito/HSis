#nullable enable
using HSis.Data.Models;
using HSis.Logic.Services;
using HSis.Logic.DTOs;
using System.Runtime.Versioning;

namespace HSis.UI
{
    /// <summary>
    /// Formulario de solo lectura para que clientes (Rol 3) vean el progreso de sus tickets.
    /// Responsabilidad única: Mostrar información de ticket en modo consulta.
    /// Cumple SRP: No permite ediciones, solo visualización.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public partial class frmDetalleCliente : Form
    {
        private int _idTicket;
        private readonly TicketService _ticketService;
        private TicketDto? _ticketActual;

        public frmDetalleCliente(int idTicket, TicketService ticketService)
        {
            InitializeComponent();
            _idTicket = idTicket;
            _ticketService = ticketService;
        }

        private async void frmDetalleCliente_Load(object? sender, EventArgs e)
        {
            try
            {
                // Cargar el ticket desde la base de datos
                _ticketActual = await _ticketService.ObtenerTicketPorIdAsync(_idTicket);

                if (_ticketActual == null)
                {
                    MessageBox.Show("Ticket no encontrado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                // Mostrar datos del ticket (solo lectura)
                lblFolioValor.Text = $"TK{_ticketActual.IdTicket:D6}";
                lblFechaAltaValor.Text = _ticketActual.Alta?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";
                lblEstatusValor.Text = _ticketActual.Status ?? "Desconocido";
                lblTecnicoValor.Text = _ticketActual.NombreTecnico ?? "Sin asignar";
                txtDescripcion.Text = _ticketActual.Descripcion ?? string.Empty;
                txtSolucion.Text = _ticketActual.Solucion ?? string.Empty;

                if (_ticketActual.Status == ConstantesEstatus.CERRADO)
                {
                    lblFechaCierre.Visible = true;
                    lblFechaCierreValor.Visible = true;
                    lblFechaCierreValor.Text = _ticketActual.Cierre?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";
                }
                else
                {
                    lblFechaCierre.Visible = false;
                    lblFechaCierreValor.Visible = false;
                }

                // Aplicar estilo de color según el estatus
                AplicarEstiloEstatus(_ticketActual.Status);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el ticket: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        /// <summary>
        /// Aplica colores visuales según el estatus del ticket para mejor UX.
        /// </summary>
        private void AplicarEstiloEstatus(string? estatus)
        {
            if (string.IsNullOrEmpty(estatus)) return;

            (Color back, Color fore) = estatus switch
            {
                ConstantesEstatus.ABIERTO => (Color.LightBlue, Color.DarkBlue),
                ConstantesEstatus.EN_PROCESO => (Color.LightYellow, Color.DarkGoldenrod),
                ConstantesEstatus.CERRADO => (Color.LightGreen, Color.DarkGreen),
                ConstantesEstatus.REABIERTO => (Color.FromArgb(153, 102, 204), Color.LightPink),
                _ => (Color.Gray, Color.White)
            };

            lblEstatusValor.BackColor = back;
            lblEstatusValor.ForeColor = fore;
        }

        private void btnCerrar_Click(object? sender, EventArgs e)
        {
            this.Close();
        }
    }
}
