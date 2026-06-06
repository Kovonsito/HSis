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
        private readonly int _idTicket;
        private readonly TicketService _ticketService;
        private TicketDto? _ticketActual;
        private int _calificacionSeleccionada = 5;

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

                MostrarSeccionFeedback(_ticketActual);
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

        private void MostrarSeccionFeedback(TicketDto ticket)
        {
            bool mostrarFdb = false;
            bool esEditable = false;

            if (ticket.Status == ConstantesEstatus.CERRADO)
            {
                mostrarFdb = true;
                esEditable = !ticket.Calificacion.HasValue;
            }

            if (!mostrarFdb)
            {
                grpFeedback.Visible = false;
                this.ClientSize = new Size(520, 391);
                btnCerrar.Location = new Point(413, 350);
                return;
            }

            grpFeedback.Visible = true;
            this.ClientSize = new Size(520, 640);
            btnCerrar.Location = new Point(413, 590);

            if (esEditable)
            {
                lblEstrellas.Visible = true;
                lblStar1.Visible = true;
                lblStar2.Visible = true;
                lblStar3.Visible = true;
                lblStar4.Visible = true;
                lblStar5.Visible = true;
                lblComentario.Visible = true;
                txtComentario.Visible = true;
                btnEnviar.Visible = true;

                lblResumen.Visible = false;
                lblComentarioLectura.Visible = false;

                _calificacionSeleccionada = 5;
                ActualizarEstrellasVisuales(5);
                txtComentario.Text = string.Empty;
            }
            else
            {
                lblEstrellas.Visible = false;
                lblStar1.Visible = false;
                lblStar2.Visible = false;
                lblStar3.Visible = false;
                lblStar4.Visible = false;
                lblStar5.Visible = false;
                lblComentario.Visible = false;
                txtComentario.Visible = false;
                btnEnviar.Visible = false;

                lblResumen.Visible = true;
                lblComentarioLectura.Visible = true;

                string estrellasStr = new('⭐', ticket.Calificacion ?? 0);
                lblResumen.Text = $"Calificación dada: {estrellasStr} ({ticket.Calificacion}/5)";
                lblComentarioLectura.Text = string.IsNullOrEmpty(ticket.ComentarioFeedback)
                    ? "No ingresaste comentarios."
                    : $"Comentario: \"{ticket.ComentarioFeedback}\"";
            }
        }

        private void lblStar_Click(object? sender, EventArgs e)
        {
            if (sender is Label lbl && int.TryParse(lbl.Tag?.ToString(), out int score))
            {
                _calificacionSeleccionada = score;
                ActualizarEstrellasVisuales(score);
            }
        }

        private void lblStar_MouseEnter(object? sender, EventArgs e)
        {
            if (sender is Label lbl && int.TryParse(lbl.Tag?.ToString(), out int score))
            {
                ActualizarEstrellasVisuales(score);
            }
        }

        private void lblStar_MouseLeave(object? sender, EventArgs e)
        {
            ActualizarEstrellasVisuales(_calificacionSeleccionada);
        }

        private void ActualizarEstrellasVisuales(int score)
        {
            Label[] stars = [lblStar1, lblStar2, lblStar3, lblStar4, lblStar5];
            for (int i = 0; i < stars.Length; i++)
            {
                if (i < score)
                {
                    stars[i].Text = "★";
                    stars[i].ForeColor = Color.FromArgb(241, 196, 15);
                }
                else
                {
                    stars[i].Text = "☆";
                    stars[i].ForeColor = Color.Gray;
                }
            }
        }

        private async void btnEnviarFeedback_Click(object? sender, EventArgs e)
        {
            try
            {
                int calificacion = _calificacionSeleccionada;
                string comentario = txtComentario.Text.Trim();

                bool ok = await _ticketService.RegistrarCalificacionAsync(_idTicket, calificacion, comentario);
                if (ok)
                {
                    MessageBox.Show("¡Gracias por tu retroalimentación!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _ticketActual = await _ticketService.ObtenerTicketPorIdAsync(_idTicket);
                    if (_ticketActual != null)
                    {
                        MostrarSeccionFeedback(_ticketActual);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar feedback: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
