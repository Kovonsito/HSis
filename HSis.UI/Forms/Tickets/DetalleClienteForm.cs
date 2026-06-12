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

        private void InicializarLayoutDetalleCliente()
        {
            // 1. Crear el TableLayoutPanel principal
            var tblPrincipal = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                RowCount = 7,
                ColumnCount = 1,
                Padding = new Padding(12),
                Name = "tblPrincipal"
            };

            // 2. Grid de Información
            var tblInfo = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                RowCount = 5,
                ColumnCount = 2,
                Margin = new Padding(0, 0, 0, 10)
            };
            tblInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            tblInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            for (int i = 0; i < 5; i++)
            {
                tblInfo.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }

            lblFolio.Dock = DockStyle.Fill;
            lblFolioValor.Dock = DockStyle.Fill;
            lblFechaAlta.Dock = DockStyle.Fill;
            lblFechaAltaValor.Dock = DockStyle.Fill;
            lblFechaCierre.Dock = DockStyle.Fill;
            lblFechaCierreValor.Dock = DockStyle.Fill;
            lblEstatus.Dock = DockStyle.Fill;
            lblEstatusValor.Dock = DockStyle.Fill;
            lblTecnico.Dock = DockStyle.Fill;
            lblTecnicoValor.Dock = DockStyle.Fill;

            tblInfo.Controls.Add(lblFolio, 0, 0);
            tblInfo.Controls.Add(lblFolioValor, 1, 0);
            tblInfo.Controls.Add(lblFechaAlta, 0, 1);
            tblInfo.Controls.Add(lblFechaAltaValor, 1, 1);
            tblInfo.Controls.Add(lblFechaCierre, 0, 2);
            tblInfo.Controls.Add(lblFechaCierreValor, 1, 2);
            tblInfo.Controls.Add(lblEstatus, 0, 3);
            tblInfo.Controls.Add(lblEstatusValor, 1, 3);
            tblInfo.Controls.Add(lblTecnico, 0, 4);
            tblInfo.Controls.Add(lblTecnicoValor, 1, 4);

            // 3. Descripciones y soluciones
            lblDescripcion.Dock = DockStyle.Fill;
            lblDescripcion.Margin = new Padding(0, 0, 0, 5);
            txtDescripcion.Dock = DockStyle.Fill;
            txtDescripcion.Height = 80;
            txtDescripcion.Margin = new Padding(0, 0, 0, 10);

            lblSolucion.Dock = DockStyle.Fill;
            lblSolucion.Margin = new Padding(0, 0, 0, 5);
            txtSolucion.Dock = DockStyle.Fill;
            txtSolucion.Height = 80;
            txtSolucion.Margin = new Padding(0, 0, 0, 10);

            // 4. Seccion de feedback
            grpFeedback.Dock = DockStyle.Fill;
            grpFeedback.Margin = new Padding(0, 0, 0, 10);
            grpFeedback.AutoSize = true;

            var tblFeedback = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                RowCount = 3,
                ColumnCount = 1,
                Padding = new Padding(12)
            };
            tblFeedback.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblFeedback.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tblFeedback.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var flpEstrellas = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 10)
            };
            lblEstrellas.Margin = new Padding(0, 5, 10, 0);
            lblEstrellas.AutoSize = true;

            flpEstrellas.Controls.Add(lblEstrellas);
            flpEstrellas.Controls.Add(lblStar1);
            flpEstrellas.Controls.Add(lblStar2);
            flpEstrellas.Controls.Add(lblStar3);
            flpEstrellas.Controls.Add(lblStar4);
            flpEstrellas.Controls.Add(lblStar5);
            flpEstrellas.Controls.Add(lblResumen);

            lblComentario.Margin = new Padding(0, 0, 0, 5);
            lblComentario.AutoSize = true;

            var tblComentarioInput = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                RowCount = 1,
                ColumnCount = 2,
                Margin = new Padding(0)
            };
            tblComentarioInput.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblComentarioInput.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));

            var pnlComentario = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0) };
            txtComentario.Dock = DockStyle.Fill;
            lblComentarioLectura.Dock = DockStyle.Fill;
            pnlComentario.Controls.Add(txtComentario);
            pnlComentario.Controls.Add(lblComentarioLectura);

            btnEnviar.Dock = DockStyle.Fill;
            btnEnviar.Margin = new Padding(10, 0, 0, 0);

            tblComentarioInput.Controls.Add(pnlComentario, 0, 0);
            tblComentarioInput.Controls.Add(btnEnviar, 1, 0);

            grpFeedback.Controls.Clear();
            tblFeedback.Controls.Add(flpEstrellas, 0, 0);
            tblFeedback.Controls.Add(lblComentario, 0, 1);
            tblFeedback.Controls.Add(tblComentarioInput, 0, 2);
            grpFeedback.Controls.Add(tblFeedback);

            // 5. Botón cerrar
            var flpCerrar = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                Margin = new Padding(0)
            };
            btnCerrar.Margin = new Padding(0);
            btnCerrar.Dock = DockStyle.None;
            flpCerrar.Controls.Add(btnCerrar);

            // 6. Montar todo en tblPrincipal
            tblPrincipal.Controls.Add(tblInfo, 0, 0);
            tblPrincipal.Controls.Add(lblDescripcion, 0, 1);
            tblPrincipal.Controls.Add(txtDescripcion, 0, 2);
            tblPrincipal.Controls.Add(lblSolucion, 0, 3);
            tblPrincipal.Controls.Add(txtSolucion, 0, 4);
            tblPrincipal.Controls.Add(grpFeedback, 0, 5);
            tblPrincipal.Controls.Add(flpCerrar, 0, 6);

            // Remover de la ventana original
            this.Controls.Clear();
            this.Controls.Add(tblPrincipal);
        }
    }
}
