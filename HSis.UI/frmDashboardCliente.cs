#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;
using HSis.Data.Models;
using HSis.Logic.DTOs;
using HSis.Logic.Services;

namespace HSis.UI
{
    [SupportedOSPlatform("windows")]
    public partial class frmDashboardCliente : Form
    {
        private readonly ITicketService _ticketService;
        private readonly INotificationClientService _notificationClient;
        private readonly INotificacionStorageService _storageService;
        private Panel? _pnlResilienceBanner;
        private Label? _lblResilienceBanner;
        private ucPaginacion ucPaginacion = null!;
        private List<TicketClienteDto> _todosLosTickets = [];

        private ucIndicador ucMisCerrados = null!;
        private Panel? pnlNotificacionesHistorial;
        private FlowLayoutPanel? flpNotificaciones;
        private ToolStripMenuItem? menuCampanaItem;
        private int _notificacionesNoLeidas = 0;
        private readonly IFormFactory _formFactory;

        private enum VistaCliente
        {
            Todos,
            Activos,
            Cerrados
        }
        private VistaCliente _vistaActual = VistaCliente.Todos;

        private readonly ISessionCacheService _sessionCache;

        public frmDashboardCliente(ITicketService ticketService, INotificationClientService notificationClient, INotificacionStorageService storageService, IFormFactory formFactory, ISessionCacheService sessionCache)
        {
            InitializeComponent();
            _ticketService = ticketService;
            _notificationClient = notificationClient;
            _storageService = storageService;
            _formFactory = formFactory;
            _sessionCache = sessionCache;
        }

        private async void frmDashboardCliente_Load(object? sender, EventArgs e)
        {
            InicializarBannerResiliencia();
            SesionSistema.ConfigurarMenuSesion(this, _sessionCache);
            AjustarZOrderControles();

            // Configurar campana en el menú
            var menu = this.MainMenuStrip;
            if (menu != null)
            {
                menuCampanaItem = new ToolStripMenuItem("🔔 (0)")
                {
                    Alignment = ToolStripItemAlignment.Right
                };
                menuCampanaItem.Click += BtnCampana_Click;
                menu.Items.Add(menuCampanaItem);
            }

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
            await CargarHistorialNotificacionesAsync();
        }

        private async Task CargarDatosDashboardAsync()
        {
            try
            {
                var tickets = await _ticketService.ObtenerTicketsPorUsuarioAsync(SesionSistema.IdUsuario);

                // Actualizar Indicador
                ActualizarIndicador(tickets);

                // Actualizar Grid
                ActualizarGridTickets(tickets);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el dashboard: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarGridTickets(List<TicketDto> tickets)
        {
            _todosLosTickets = tickets.ConvertAll(t => new TicketClienteDto
            {
                IdTicket = t.IdTicket,
                FechaAlta = t.Alta,
                Status = t.Status,
                TecnicoAsignado = t.NombreTecnico ?? "Sin asignar",
                Descripcion = !string.IsNullOrEmpty(t.Descripcion) && t.Descripcion.Length > 50 ? string.Concat(t.Descripcion.AsSpan(0, 50), "...") : (t.Descripcion ?? ""),
                Feedback = t.Status == ConstantesEstatus.CERRADO
                    ? (t.Calificacion.HasValue ? $"Enviada ({new string('★', t.Calificacion.Value)}{new string('☆', 5 - t.Calificacion.Value)})" : "Pendiente")
                    : "N/A"
            });

            ucPaginacion.CurrentPage = 1;
            MostrarPaginaActual();
        }

        private void MostrarPaginaActual()
        {
            // 1. Filtrar en memoria según la vista actual
            var ticketsFiltrados = _todosLosTickets;
            if (_vistaActual == VistaCliente.Activos)
            {
                ticketsFiltrados = [.. _todosLosTickets.Where(t => t.Status != ConstantesEstatus.CERRADO)];
            }
            else if (_vistaActual == VistaCliente.Cerrados)
            {
                ticketsFiltrados = [.. _todosLosTickets.Where(t => t.Status == ConstantesEstatus.CERRADO)];
            }

            ucPaginacion.TotalRecords = ticketsFiltrados.Count;

            // 2. Segmentar la página actual
            var pageTickets = ticketsFiltrados
                .Skip((ucPaginacion.CurrentPage - 1) * ucPaginacion.PageSize)
                .Take(ucPaginacion.PageSize)
                .ToList();

            dgvMisTickets.DataSource = new SortableBindingList<TicketClienteDto>(pageTickets);
            PersonalizarColumnas();
            ucPaginacion.ActualizarInterfaz();
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
            var cerrados = tickets.FindAll(t => t.Status == ConstantesEstatus.CERRADO);

            ucMisActivos.Cantidad = activos.Count.ToString();
            ucMisActivos.Titulo = "Mis Tickets Activos";
            ucMisActivos.ColorFondo = Color.FromArgb(41, 128, 185); // Azul

            if (ucMisCerrados != null)
            {
                ucMisCerrados.Cantidad = cerrados.Count.ToString();
                ucMisCerrados.Titulo = "Mis Tickets Cerrados";
                ucMisCerrados.ColorFondo = Color.FromArgb(46, 204, 113); // Verde
            }
        }

        private void UcMisActivos_Click(object? sender, EventArgs e)
        {
            _vistaActual = _vistaActual == VistaCliente.Activos ? VistaCliente.Todos : VistaCliente.Activos;
            ucPaginacion.CurrentPage = 1;
            MostrarPaginaActual();
        }

        private void UcMisCerrados_Click(object? sender, EventArgs e)
        {
            _vistaActual = _vistaActual == VistaCliente.Cerrados ? VistaCliente.Todos : VistaCliente.Cerrados;
            ucPaginacion.CurrentPage = 1;
            MostrarPaginaActual();
        }

        private void btnNuevoReporte_Click(object? sender, EventArgs e)
        {
            using var frmNuevo = _formFactory.Create<frmNuevoTicket>();
            if (frmNuevo.ShowDialog() == DialogResult.OK)
            {
                _ = CargarDatosDashboardAsync();
            }
        }

        private void dgvMisTickets_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvMisTickets.Rows[e.RowIndex];
                if (int.TryParse(row.Cells["IdTicket"].Value?.ToString(), out int idTicket))
                {
                    using var frmDetalle = _formFactory.CreateDetalleCliente(idTicket);
                    frmDetalle.ShowDialog();
                    _ = CargarDatosDashboardAsync();
                }
            }
        }

        private void InicializarBannerResiliencia()
        {
            // Instanciar control de paginación
            ucPaginacion = new ucPaginacion();
            ucPaginacion.PageChanged += (s, e) => MostrarPaginaActual();

            // Instanciar indicador de cerrados
            ucMisCerrados = new ucIndicador();

            // Suscribir eventos de filtrado
            ucMisActivos.ucIndicadorEvent += UcMisActivos_Click;
            ucMisCerrados.ucIndicadorEvent += UcMisCerrados_Click;

            // 1. Crear el TableLayoutPanel principal que ocupará todo el formulario
            var tblPrincipal = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Name = "tblPrincipal",
                RowCount = 4,
                ColumnCount = 1,
                Size = this.ClientSize
            };

            // Definir filas del grid principal
            tblPrincipal.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Fila 0: Título (lblTitulo)
            tblPrincipal.RowStyles.Add(new RowStyle(SizeType.Absolute, 110F)); // Fila 1: Indicador y Botón
            tblPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Fila 2: Grid de Tickets (dgvMisTickets)
            tblPrincipal.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F)); // Fila 3: Paginación

            // 2. Crear un TableLayoutPanel interno para el indicador y el botón de nuevo ticket
            var tblCabecera = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Name = "tblCabecera",
                RowCount = 1,
                ColumnCount = 3,
                Margin = new Padding(0)
            };
            tblCabecera.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F)); // Columna 0: ucMisActivos
            tblCabecera.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F)); // Columna 1: ucMisCerrados
            tblCabecera.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); // Columna 2: btnNuevoReporte centrado

            // Panel contenedor para centrar verticalmente el botón
            var pnlBoton = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0)
            };

            // Configurar el botón
            btnNuevoReporte.Location = new Point(10, 20); // Posicionarlo de forma prolija en el panel
            pnlBoton.Controls.Add(btnNuevoReporte);

            ucMisActivos.Dock = DockStyle.Fill;
            ucMisActivos.Margin = new Padding(12, 5, 5, 5);

            ucMisCerrados.Dock = DockStyle.Fill;
            ucMisCerrados.Margin = new Padding(5);

            tblCabecera.Controls.Add(ucMisActivos, 0, 0);
            tblCabecera.Controls.Add(ucMisCerrados, 1, 0);
            tblCabecera.Controls.Add(pnlBoton, 2, 0);

            // Configurar el título y el grid principal para que se estiren
            lblTitulo.Dock = DockStyle.Fill;
            lblTitulo.Margin = new Padding(12, 10, 12, 10);

            // Configurar el grid principal para que se estire
            dgvMisTickets.Dock = DockStyle.Fill;
            dgvMisTickets.Margin = new Padding(12, 10, 12, 12);

            // Crear Panel de Notificaciones (Flotante)
            pnlNotificacionesHistorial = new Panel
            {
                Width = 300,
                Height = this.ClientSize.Height - 35,
                Location = new Point(this.ClientSize.Width - 300, 35),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right,
                BackColor = Color.FromArgb(245, 247, 250),
                Visible = false,
                Padding = new Padding(10)
            };

            var pnlNotifHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(0, 0, 0, 5)
            };
            var lblNotifTitle = new Label
            {
                Text = "Notificaciones",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Dock = DockStyle.Left,
                AutoSize = true
            };
            var btnLimpiarNotif = new Button
            {
                Text = "Limpiar todo",
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Dock = DockStyle.Right,
                Width = 95,
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLimpiarNotif.FlatAppearance.BorderSize = 1;
            btnLimpiarNotif.FlatAppearance.BorderColor = Color.LightGray;
            btnLimpiarNotif.Click += BtnLimpiarNotif_Click;

            pnlNotifHeader.Controls.Add(lblNotifTitle);
            pnlNotifHeader.Controls.Add(btnLimpiarNotif);
            pnlNotificacionesHistorial.Controls.Add(pnlNotifHeader);

            flpNotificaciones = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(0, 5, 0, 0)
            };
            flpNotificaciones.SizeChanged += (s, e) =>
            {
                foreach (Control ctrl in flpNotificaciones.Controls)
                {
                    ctrl.Width = flpNotificaciones.ClientSize.Width - 10;
                }
            };
            pnlNotificacionesHistorial.Controls.Add(flpNotificaciones);

            pnlNotifHeader.SendToBack();
            flpNotificaciones.BringToFront();

            // Agregar componentes al TableLayoutPanel principal
            tblPrincipal.Controls.Add(lblTitulo, 0, 0);
            tblPrincipal.Controls.Add(tblCabecera, 0, 1);
            tblPrincipal.Controls.Add(dgvMisTickets, 0, 2);
            tblPrincipal.Controls.Add(ucPaginacion, 0, 3);

            // Remover controles del formulario para agregarlos al grid principal
            this.Controls.Remove(lblTitulo);
            this.Controls.Remove(ucMisActivos);
            this.Controls.Remove(btnNuevoReporte);
            this.Controls.Remove(dgvMisTickets);

            // 3. Crear el banner de resiliencia
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

            // 4. Agregar los paneles al formulario
            this.Controls.Add(tblPrincipal);
            this.Controls.Add(pnlNotificacionesHistorial);
            this.Controls.Add(_pnlResilienceBanner);
        }

        private void AjustarZOrderControles()
        {
            if (pnlNotificacionesHistorial != null)
            {
                this.Controls.SetChildIndex(pnlNotificacionesHistorial, 0); // Al frente de todo (flotante)
            }
            var tblPrincipal = this.Controls["tblPrincipal"];
            if (tblPrincipal != null)
            {
                this.Controls.SetChildIndex(tblPrincipal, 1); // Debajo del panel de notificaciones
            }
            if (_pnlResilienceBanner != null)
            {
                this.Controls.SetChildIndex(_pnlResilienceBanner, 2); // Debajo de tblPrincipal
            }
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is MenuStrip)
                {
                    this.Controls.SetChildIndex(ctrl, 3); // Al fondo en lógica de layout
                    break;
                }
            }
        }

        private async void OnNotificationReceived(string tipo, int ticketId, string mensaje)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => OnNotificationReceived(tipo, ticketId, mensaje)));
                return;
            }

            // Guardar persistentemente
            await _storageService.GuardarNotificacionAsync(SesionSistema.IdUsuario, ticketId, mensaje);

            MessageBox.Show(mensaje, "Notificación de HSis", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            _ = CargarDatosDashboardAsync();
            _ = CargarHistorialNotificacionesAsync();
        }

        private async Task CargarHistorialNotificacionesAsync()
        {
            if (flpNotificaciones == null) return;

            flpNotificaciones.Controls.Clear();
            var list = await _storageService.ObtenerNotificacionesAsync(SesionSistema.IdUsuario);

            _notificacionesNoLeidas = list.Count(n => !n.Leido);
            ActualizarCampanaBadge();

            foreach (var notif in list)
            {
                AgregarNotificacionAUI(notif);
            }
        }

        private void ActualizarCampanaBadge()
        {
            if (menuCampanaItem != null)
            {
                menuCampanaItem.Text = _notificacionesNoLeidas > 0 ? $"🔔 ({_notificacionesNoLeidas}) 🔴" : $"🔔 ({_notificacionesNoLeidas})";
                menuCampanaItem.ForeColor = _notificacionesNoLeidas > 0 ? Color.Red : Color.Black;
                menuCampanaItem.Font = new Font("Segoe UI", 10F, _notificacionesNoLeidas > 0 ? FontStyle.Bold : FontStyle.Regular);
            }
        }

        private void AgregarNotificacionAUI(NotificacionLocal notif)
        {
            if (flpNotificaciones == null) return;

            var pnlItem = new Panel
            {
                Width = flpNotificaciones.ClientSize.Width - 10,
                Height = 85,
                BackColor = notif.Leido ? Color.White : Color.FromArgb(235, 245, 251),
                Padding = new Padding(8),
                Margin = new Padding(0, 0, 0, 8),
                Cursor = Cursors.Hand
            };

            pnlItem.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, pnlItem.ClientRectangle, Color.FromArgb(220, 224, 230), ButtonBorderStyle.Solid);
                if (!notif.Leido)
                {
                    using var brush = new SolidBrush(Color.FromArgb(52, 152, 219));
                    e.Graphics.FillEllipse(brush, pnlItem.Width - 15, 8, 8, 8);
                }
            };

            var lblMsg = new Label
            {
                Text = notif.Mensaje,
                Font = new Font("Segoe UI", 9.5F, notif.Leido ? FontStyle.Regular : FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(8, 8),
                Size = new Size(pnlItem.Width - 25, 50),
                AutoEllipsis = true
            };

            var lblFecha = new Label
            {
                Text = notif.Fecha.ToString("g"),
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = Color.Gray,
                Location = new Point(8, 60),
                Size = new Size(pnlItem.Width - 20, 18)
            };

            lblMsg.Click += (s, e) => AbrirDetalleYMarcarLeido(notif, pnlItem);
            lblFecha.Click += (s, e) => AbrirDetalleYMarcarLeido(notif, pnlItem);
            pnlItem.Click += (s, e) => AbrirDetalleYMarcarLeido(notif, pnlItem);

            pnlItem.Controls.Add(lblMsg);
            pnlItem.Controls.Add(lblFecha);

            flpNotificaciones.Controls.Add(pnlItem);
        }

        private async void AbrirDetalleYMarcarLeido(NotificacionLocal notif, Panel pnlItem)
        {
            if (!notif.Leido)
            {
                await _storageService.MarcarComoLeidaAsync(SesionSistema.IdUsuario, notif.Id);
                notif.Leido = true;
                pnlItem.BackColor = Color.White;
                foreach (Control ctrl in pnlItem.Controls)
                {
                    if (ctrl is Label lbl && lbl.Text == notif.Mensaje)
                    {
                        lbl.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
                    }
                }
                _notificacionesNoLeidas = Math.Max(0, _notificacionesNoLeidas - 1);
                ActualizarCampanaBadge();
                pnlItem.Invalidate();
            }

            using var frmDetalle = _formFactory.CreateDetalleCliente(notif.TicketId);
            frmDetalle.ShowDialog();
            _ = CargarDatosDashboardAsync();
        }

        private async void BtnLimpiarNotif_Click(object? sender, EventArgs e)
        {
            await _storageService.LimpiarTodasAsync(SesionSistema.IdUsuario);
            _notificacionesNoLeidas = 0;
            ActualizarCampanaBadge();
            flpNotificaciones?.Controls.Clear();
        }

        private void BtnCampana_Click(object? sender, EventArgs e)
        {
            if (pnlNotificacionesHistorial != null)
            {
                pnlNotificacionesHistorial.Visible = !pnlNotificacionesHistorial.Visible;
                if (pnlNotificacionesHistorial.Visible)
                {
                    pnlNotificacionesHistorial.BringToFront();
                }
            }
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

        }
    }
}
