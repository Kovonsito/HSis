#nullable enable
using HSis.Data.Models;
using HSis.Logic.DTOs;
using HSis.Logic.Services;
using System.Runtime.Versioning;

namespace HSis.UI
{
    [SupportedOSPlatform("windows")]
    public partial class frmDashboardCliente : Form
    {
        private readonly TicketService _ticketService;
        private readonly NotificationClientService _notificationClient;
        private Panel? _pnlResilienceBanner;
        private Label? _lblResilienceBanner;

        public frmDashboardCliente(TicketService ticketService, NotificationClientService notificationClient)
        {
            InitializeComponent();
            _ticketService = ticketService;
            _notificationClient = notificationClient;
        }

        private async void frmDashboardCliente_Load(object? sender, EventArgs e)
        {
            InicializarBannerResiliencia();
            SesionSistema.ConfigurarMenuSesion(this);

            // Suscribirse a los eventos de SignalR
            _notificationClient.OnNotificationReceived += OnNotificationReceived;
            _notificationClient.OnReconnecting += OnReconnecting;
            _notificationClient.OnConnected += OnConnected;
            _notificationClient.OnDisconnected += OnDisconnected;

            // Limpieza al cerrar formulario
            this.FormClosed += (s, args) =>
            {
                _notificationClient.OnNotificationReceived -= OnNotificationReceived;
                _notificationClient.OnReconnecting -= OnReconnecting;
                _notificationClient.OnConnected -= OnConnected;
                _notificationClient.OnDisconnected -= OnDisconnected;
            };

            // Establecer estado inicial según la conexión
            ActualizarEstadoConexion(_notificationClient.IsConnected, "⚠️ Conectando al servidor de notificaciones...", Color.FromArgb(230, 126, 34));

            // Cargamos la información una sola vez para evitar múltiples llamadas a la BD
            await CargarDatosDashboardAsync();
        }

        private async Task CargarDatosDashboardAsync()
        {
            try
            {
                var tickets = await _ticketService.ObtenerTicketsPorUsuarioAsync(SesionSistema.IdUsuario);

                // Actualizar Grid
                ActualizarGridTickets(tickets);

                // Actualizar Indicador
                ActualizarIndicador(tickets);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el dashboard: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarGridTickets(List<TicketDto> tickets)
        {
            var ticketsDto = tickets.ConvertAll(t => new TicketClienteDto
            {
                IdTicket = t.IdTicket,
                FechaAlta = t.Alta,
                Status = t.Status,
                TecnicoAsignado = t.NombreTecnico ?? "Sin asignar",
                Descripcion = !string.IsNullOrEmpty(t.Descripcion) && t.Descripcion.Length > 50 ? t.Descripcion.Substring(0, 50) + "..." : (t.Descripcion ?? ""),
                Feedback = t.Status == ConstantesEstatus.CERRADO
                    ? (t.Calificacion.HasValue ? $"Enviada ({new string('★', t.Calificacion.Value)}{new string('☆', 5 - t.Calificacion.Value)})" : "Pendiente")
                    : "N/A"
            });

            dgvMisTickets.DataSource = new SortableBindingList<TicketClienteDto>(ticketsDto);
            PersonalizarColumnas();
        }

        private void PersonalizarColumnas()
        {
            if (dgvMisTickets.Columns.Count > 0)
            {
                var colIdTicket = dgvMisTickets.Columns["IdTicket"];
                if (colIdTicket != null) colIdTicket.Visible = false;

                var colFolio = dgvMisTickets.Columns["Folio"];
                if (colFolio != null) colFolio.HeaderText = "Folio";

                var colFechaAlta = dgvMisTickets.Columns["FechaAlta"];
                if (colFechaAlta != null)
                {
                    colFechaAlta.HeaderText = "Fecha de Alta";
                    colFechaAlta.Width = 100;
                }

                var colStatus = dgvMisTickets.Columns["Status"];
                if (colStatus != null)
                {
                    colStatus.HeaderText = "Estatus";
                    colStatus.Width = 100;
                }

                var colTecnicoAsignado = dgvMisTickets.Columns["TecnicoAsignado"];
                if (colTecnicoAsignado != null)
                {
                    colTecnicoAsignado.HeaderText = "Técnico Asignado";
                    colTecnicoAsignado.Width = 120;
                }

                var colDescripcion = dgvMisTickets.Columns["Descripcion"];
                if (colDescripcion != null) colDescripcion.HeaderText = "Descripción";

                var colFeedback = dgvMisTickets.Columns["Feedback"];
                if (colFeedback != null)
                {
                    colFeedback.HeaderText = "Retroalimentación";
                    colFeedback.Width = 140;
                }
            }
        }

        private void ActualizarIndicador(List<TicketDto> tickets)
        {
            var activos = tickets.FindAll(t => t.Status != ConstantesEstatus.CERRADO);

            ucMisActivos.Cantidad = activos.Count.ToString();
            ucMisActivos.Titulo = "Mis Tickets Activos";
            ucMisActivos.ColorFondo = Color.FromArgb(41, 128, 185);
        }

        private void btnNuevoReporte_Click(object? sender, EventArgs e)
        {
            using (var frmNuevo = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance<frmNuevoTicket>(Program.ServiceProvider))
            {
                if (frmNuevo.ShowDialog() == DialogResult.OK)
                {
                    _ = CargarDatosDashboardAsync();
                }
            }
        }

        private void dgvMisTickets_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvMisTickets.Rows[e.RowIndex];
                if (int.TryParse(row.Cells["IdTicket"].Value?.ToString(), out int idTicket))
                {
                    using (var frmDetalle = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance<frmDetalleCliente>(Program.ServiceProvider, idTicket))
                    {
                        frmDetalle.ShowDialog();
                        _ = CargarDatosDashboardAsync();
                    }
                }
            }
        }

        private void InicializarBannerResiliencia()
        {
            _pnlResilienceBanner = new Panel
            {
                Dock = DockStyle.Top,
                Height = 35,
                BackColor = Color.FromArgb(231, 76, 60), // Rojo
                Visible = false
            };

            _lblResilienceBanner = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "⚠️ Sin conexión con el servidor de notificaciones. Intentando reconectar..."
            };

            _pnlResilienceBanner.Controls.Add(_lblResilienceBanner);
            this.Controls.Add(_pnlResilienceBanner);
            _pnlResilienceBanner.BringToFront();
        }

        private void OnNotificationReceived(string tipo, int ticketId, string mensaje)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => OnNotificationReceived(tipo, ticketId, mensaje)));
                return;
            }

            MessageBox.Show(mensaje, "Notificación de HSis", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _ = CargarDatosDashboardAsync();
        }

        private void OnReconnecting()
        {
            ActualizarEstadoConexion(false, "⚠️ Intentando reconectar con el servidor de notificaciones...", Color.FromArgb(230, 126, 34)); // Naranja
        }

        private void OnConnected()
        {
            ActualizarEstadoConexion(true, string.Empty, Color.Empty);
            _ = CargarDatosDashboardAsync(); // Recargar datos al reconectarse
        }

        private void OnDisconnected()
        {
            ActualizarEstadoConexion(false, "⚠️ Sin conexión con el servidor de notificaciones. Intentando reconectar...", Color.FromArgb(231, 76, 60)); // Rojo
        }

        private void ActualizarEstadoConexion(bool conectado, string mensaje, Color colorFondo)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => ActualizarEstadoConexion(conectado, mensaje, colorFondo)));
                return;
            }

            if (_pnlResilienceBanner != null && _lblResilienceBanner != null)
            {
                _pnlResilienceBanner.Visible = !conectado;
                _lblResilienceBanner.Text = mensaje;
                _pnlResilienceBanner.BackColor = colorFondo;
            }

            btnNuevoReporte.Enabled = conectado;
            dgvMisTickets.Enabled = conectado;
        }
    }
}
