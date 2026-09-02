using System.Runtime.Versioning;
using HSis.Logic.Constants;
using HSis.Logic.DTOs;
using HSis.UI.Presenters;

namespace HSis.UI.Forms.Tickets
{
    [SupportedOSPlatform("windows")]
    public partial class DetalleClienteForm : Form, IDetalleClienteView
    {
        private readonly int _idTicket;
        private readonly DetalleClientePresenter _presenter;
        private TicketDto? _ticketActual;
        private int _calificacionSeleccionada = 5;

        public DetalleClienteForm(int idTicket, DetalleClientePresenter presenter)
        {
            InitializeComponent();
            _idTicket = idTicket;
            _presenter = presenter;
            _presenter.SetView(this);

            InicializarLayoutDetalleCliente();
        }

        #region Propiedades de IDetalleClienteView
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

        public void MostrarError(string mensaje)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => MostrarError(mensaje)));
                return;
            }
            MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        public void CerrarFormulario()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(CerrarFormulario));
                return;
            }
            this.Close();
        }


        public void MostrarCargando(bool cargando)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => MostrarCargando(cargando)));
                return;
            }
            btnEnviar.Enabled = !cargando;
            this.UseWaitCursor = cargando;
        }
        #endregion

        #region Form Events
        private async void FrmDetalleCliente_Load(object? sender, EventArgs e)
        {
            await _presenter.CargarTicketAsync(_idTicket);
        }


        private async void BtnEnviarFeedback_Click(object? sender, EventArgs e)
        {
            int calificacion = _calificacionSeleccionada;
            string comentario = txtComentario.Text.Trim();

            await _presenter.RegistrarCalificacionAsync(_idTicket, calificacion, comentario);
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

