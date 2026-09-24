#nullable enable
using System.Runtime.Versioning;
using HSis.Contracts.Constants;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.UI.Helpers;

namespace HSis.UI.Forms.Tickets
{
    [SupportedOSPlatform("windows")]
    public partial class DetalleClienteForm : Form
    {
        private readonly int _idTicket;
        private readonly ITicketService _ticketService;
        private TicketDto? _ticketActual;
        private int _calificacionSeleccionada = 5;

        public DetalleClienteForm(int idTicket, ITicketService ticketService)
        {
            InitializeComponent();
            _idTicket = idTicket;
            _ticketService = ticketService;

            InicializarLayoutDetalleCliente();
        }

        public void MostrarTicket(TicketDto ticket)
        {
            _ticketActual = ticket;

            lblFolioValor.Text = $"TK{ticket.IdTicket:D6}";
            lblFechaAltaValor.Text = ticket.FechaAlta?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";
            lblEstatusValor.Text = ticket.Estatus ?? "Desconocido";
            lblTecnicoValor.Text = ticket.NombreTecnico ?? "Sin asignar";
            txtDescripcion.Text = ticket.Descripcion ?? string.Empty;
            txtSolucion.Text = ticket.Solucion ?? string.Empty;

            if (ticket.Estatus == ConstantesEstatus.CERRADO)
            {
                lblFechaCierre.Visible = true;
                lblFechaCierreValor.Visible = true;
                lblFechaCierreValor.Text = ticket.FechaCierre?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";
            }
            else
            {
                lblFechaCierre.Visible = false;
                lblFechaCierreValor.Visible = false;
            }

            AplicarEstiloEstatus(ticket.Estatus);
            MostrarSeccionFeedback(ticket);
        }

        #region Form Events
        private async void FrmDetalleCliente_Load(object? sender, EventArgs e)
        {
            await CargarTicketAsync(_idTicket);
        }

        private async Task CargarTicketAsync(int idTicket)
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
            }, "Error al cargar ticket");
        }

        private async void BtnEnviarFeedback_Click(object? sender, EventArgs e)
        {
            int calificacion = _calificacionSeleccionada;
            string comentario = txtComentario.Text.Trim();

            await this.EjecutarOperacionAsync(async () =>
            {
                bool exito = await _ticketService.RegistrarCalificacionAsync(_idTicket, calificacion, comentario);
                if (exito)
                {
                    DialogoUIHelper.MostrarExito("¡Gracias por tu retroalimentación! La calificación fue registrada.");
                    await CargarTicketAsync(_idTicket);
                }
                else
                {
                    DialogoUIHelper.MostrarError("No se pudo registrar la calificación.");
                }
            }, "Error al registrar calificación", btnEnviar);
        }

        /// <summary>
        /// Aplica colores visuales según el estatus del ticket para mejor UX.
        /// </summary>
        private void AplicarEstiloEstatus(string? estatus)
        {
            if (string.IsNullOrEmpty(estatus)) return;

            var (fore, back) = TemaVisual.ObtenerColoresBadge(estatus);
            lblEstatusValor.ForeColor = fore;
            lblEstatusValor.BackColor = back;
        }

        private void BtnCerrar_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        private void MostrarSeccionFeedback(TicketDto ticket)
        {
            bool mostrarFdb = false;
            bool esEditable = false;

            if (ticket.Estatus == ConstantesEstatus.CERRADO)
            {
                mostrarFdb = true;
                esEditable = !ticket.Calificacion.HasValue;
            }

            if (!mostrarFdb)
            {
                grpFeedback.Visible = false;
                return;
            }

            grpFeedback.Visible = true;

            if (esEditable)
            {
                lblEstrellas.Visible = true;
                lblComentario.Visible = true;
                txtComentario.Visible = true;
                btnEnviar.Visible = true;
                txtComentario.Clear();

                lblResumen.Visible = false;
                lblComentarioLectura.Visible = false;

                _calificacionSeleccionada = 5;
                ActualizarEstrellasVisuales(5);
            }
            else
            {
                lblEstrellas.Visible = false;
                lblComentario.Visible = false;
                txtComentario.Visible = false;
                btnEnviar.Visible = false;

                lblResumen.Visible = true;
                lblComentarioLectura.Visible = true;

                string estrellasStr = new('⭐', ticket.Calificacion ?? 0);
                lblResumen.Text = $"Calificación dada: {estrellasStr} ({ticket.Calificacion}/5)";
                lblComentarioLectura.Text = string.IsNullOrEmpty(ticket.ComentarioEvaluacion)
                    ? "No ingresaste comentarios."
                    : $"Comentario: \"{ticket.ComentarioEvaluacion}\"";
            }
        }

        private void LblStar_Click(object? sender, EventArgs e)
        {
            if (sender is Label lbl && int.TryParse(lbl.Tag?.ToString(), out int score))
            {
                _calificacionSeleccionada = score;
                ActualizarEstrellasVisuales(score);
            }
        }

        private void LblStar_MouseEnter(object? sender, EventArgs e)
        {
            if (sender is Label lbl && int.TryParse(lbl.Tag?.ToString(), out int score))
            {
                ActualizarEstrellasVisuales(score);
            }
        }

        private void LblStar_MouseLeave(object? sender, EventArgs e)
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
        #endregion
    }
}
