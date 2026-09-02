using System.Runtime.Versioning;
using HSis.Data.Models;
using HSis.Logic.Constants;
using HSis.Logic.DTOs;
using HSis.UI.Controls;
using HSis.UI.Helpers;
using HSis.UI.Presenters;

namespace HSis.UI.Forms.Tickets
{
    [SupportedOSPlatform("windows")]
    public partial class TicketDetalleForm : Form, ITicketDetalleView
    {
        private readonly int _idTicket;
        private readonly TicketDetallePresenter _presenter;
        private TicketDto? _ticketActual;
        private CajaTextoOrtograficaWpf rtbDescripcion = null!;
        private CajaTextoOrtograficaWpf rtbSolucion = null!;
        private DataGridView dgvMateriales = null!;

        public TicketDetalleForm(int idTicket, TicketDetallePresenter presenter)
        {
            InitializeComponent();
            _idTicket = idTicket;
            _presenter = presenter;
            _presenter.SetView(this);

            InicializarLayoutDetalle();
        }

        #region Propiedades e Implementación de ITicketDetalleView
        public void MostrarTicket(TicketDto ticket)
        {
            _ticketActual = ticket;
            lblFolio.Text = $"Folio: TK-{ticket.IdTicket:d6}";
            txtUsuario.Text = ticket.NombreUsuario;
            txtDepartamento.Text = ticket.DepartamentoUsuario;
            txtAlta.Text = (ticket.FechaAlta ?? DateTime.Now).ToString("dddd, dd 'de' MMMM 'de' yyyy 'a las' HH:mm:ss");
            rtbDescripcion.Text = ticket.Descripcion ?? string.Empty;
            rtbSolucion.Text = ticket.Solucion ?? string.Empty;

            ConfigurarFecha(txtAtencion, ticket.FechaAtencion);
            ConfigurarFecha(txtCierre, ticket.FechaCierre);

            txtAlta.ReadOnly = true;
            txtAtencion.ReadOnly = true;
            txtCierre.ReadOnly = true;

            cmbPrioridad.SelectedItem = ticket.Prioridad;

            bool esAdmin = SesionSistema.EsAdmin;
            bool esPropietario = ticket.IdTecnico == SesionSistema.IdUsuario;
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

        public void CargarTecnicos(List<Usuario> tecnicos, int? idTecnicoActual, bool esAdmin)
        {
            cmbAtendido.DisplayMember = "Nombre";
            cmbAtendido.ValueMember = "IdUsuario";
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
            dgvMateriales.DataSource = detalles;
            ConfigurarEstilosGridMateriales();
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
            this.DialogResult = DialogResult.OK;
            this.Close();
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
        #endregion

        #region Form Handlers & Layout


        private async void FormularioTicket_Load(object? sender, EventArgs e)
        {
            await _presenter.CargarTicketDetallesAsync(_idTicket);
        }


        private void CmbEstatus_SelectedIndexChanged(object? sender, EventArgs e)
        {
            string? estatusSeleccionado = cmbEstatus.SelectedItem?.ToString();
            if (estatusSeleccionado == ConstantesEstatus.EN_PROCESO && cmbAtendido.SelectedIndex == -1)
            {
                cmbAtendido.SelectedValue = SesionSistema.IdUsuario;
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
                idTecnico = SesionSistema.IdUsuario;
            }

            string solucionIngresada = rtbSolucion.Text ?? string.Empty;
            string prioridadSeleccionada = cmbPrioridad.SelectedItem?.ToString() ?? string.Empty;

            if (estatusSeleccionado == ConstantesEstatus.EN_PROCESO && string.IsNullOrWhiteSpace(prioridadSeleccionada))
            {
                MessageBox.Show("Es necesario seleccionar una prioridad para el ticket para poder guardarlo y cambiarlo a estatus 'En Proceso'.", "Prioridad requerida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbPrioridad.Focus();
                return;
            }

            await _presenter.ActualizarTicketAsync(_ticketActual, estatusSeleccionado, idTecnico, solucionIngresada, prioridadSeleccionada);
        }

        private void btnCancelar_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnEnviarFeedback_Click(object? sender, EventArgs e)
        {
            // Evento derivado a la vista de cliente si aplica
        }

        private void ConfigurarFecha(TextBox txt, DateTime? fecha)
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
            grpFeedback.Visible = esCerrado;
            if (esCerrado && ticket.Calificacion.HasValue)
            {
                lblResumen.Text = $"Calificación: {new string('⭐', ticket.Calificacion.Value)} ({ticket.Calificacion}/5)";
                lblComentarioLectura.Text = string.IsNullOrWhiteSpace(ticket.ComentarioEvaluacion) ? "Sin comentarios." : ticket.ComentarioEvaluacion;
            }
        }

        private void ConfigurarEstilosGridHistorial()
        {
            dgvHistorial.AutoGenerateColumns = true;
            dgvHistorial.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHistorial.AplicarTemaModerno();
        }

        private void ConfigurarEstilosGridMateriales()
        {
            dgvMateriales.AutoGenerateColumns = true;
            dgvMateriales.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMateriales.AplicarTemaModerno();
        }
        #endregion
    }
}

