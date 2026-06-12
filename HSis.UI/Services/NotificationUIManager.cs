#nullable enable
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Windows.Forms;
using HSis.Data.Models;
using HSis.Logic.Services;

namespace HSis.UI
{
    [SupportedOSPlatform("windows")]
    public class NotificationUIManager
    {
        private readonly INotificationClientService _notificationClient;
        private readonly INotificacionStorageService _storageService;
        private readonly IFormFactory _formFactory;

        private Form? _hostForm;
        private Control? _mainContainer;
        private Func<Task>? _onDataReloadCallback;

        private Panel? _pnlResilienceBanner;
        private Label? _lblResilienceBanner;
        private Panel? _pnlNotificacionesHistorial;
        private FlowLayoutPanel? _flpNotificaciones;
        private ToolStripMenuItem? _menuCampanaItem;
        private int _notificacionesNoLeidas = 0;

        public NotificationUIManager(
            INotificationClientService notificationClient,
            INotificacionStorageService storageService,
            IFormFactory formFactory)
        {
            _notificationClient = notificationClient;
            _storageService = storageService;
            _formFactory = formFactory;
        }

        public void Attach(Form hostForm, Control mainContainer, Func<Task> onDataReloadCallback)
        {
            _hostForm = hostForm;
            _mainContainer = mainContainer;
            _onDataReloadCallback = onDataReloadCallback;

            // 1. Inicializar banner de resiliencia y panel de notificaciones
            InicializarBannerResiliencia();

            // 2. Configurar campana en el menú principal del formulario
            ConfigurarCampanaMenu();

            // 3. Suscribirse a los eventos de SignalR
            _notificationClient.OnNotificationReceived += OnNotificationReceived;
            _notificationClient.OnReconnecting += OnReconnecting;
            _notificationClient.OnConnected += OnConnected;
            _notificationClient.OnDisconnected += OnDisconnected;

            // 4. Limpieza automática de eventos al cerrar el formulario host
            _hostForm.FormClosed += (s, args) =>
            {
                _notificationClient.OnNotificationReceived -= OnNotificationReceived;
                _notificationClient.OnReconnecting -= OnReconnecting;
                _notificationClient.OnConnected -= OnConnected;
                _notificationClient.OnDisconnected -= OnDisconnected;
            };

            // 5. Ajustar orden de apilamiento visual de los controles inyectados
            AjustarZOrderControles();

            // 6. Establecer estado de conexión inicial
            ActualizarEstadoConexion(
                _notificationClient.IsConnected,
                "⚠️ Conectando al servidor de notificaciones...",
                Color.FromArgb(230, 126, 34)
            );

            // 7. Cargar el historial local de notificaciones
            _ = CargarHistorialNotificacionesAsync();
        }

        private void InicializarBannerResiliencia()
        {
            if (_hostForm == null) return;

            // Banner superior de resiliencia
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
            _hostForm.Controls.Add(_pnlResilienceBanner);

            // Panel flotante de historial de notificaciones
            _pnlNotificacionesHistorial = new Panel
            {
                Width = 300,
                Height = _hostForm.ClientSize.Height - 35,
                Location = new Point(_hostForm.ClientSize.Width - 300, 35),
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
            _pnlNotificacionesHistorial.Controls.Add(pnlNotifHeader);

            _flpNotificaciones = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(0, 5, 0, 0)
            };

            _flpNotificaciones.SizeChanged += (s, e) =>
            {
                if (_flpNotificaciones == null) return;
                foreach (Control ctrl in _flpNotificaciones.Controls)
                {
                    ctrl.Width = _flpNotificaciones.ClientSize.Width - 10;
                }
            };

            _pnlNotificacionesHistorial.Controls.Add(_flpNotificaciones);
            pnlNotifHeader.SendToBack();
            _flpNotificaciones.BringToFront();

            _hostForm.Controls.Add(_pnlNotificacionesHistorial);
        }

        private void ConfigurarCampanaMenu()
        {
            if (_hostForm == null) return;

            var menu = _hostForm.MainMenuStrip;
            if (menu != null)
            {
                _menuCampanaItem = new ToolStripMenuItem("🔔 (0)")
                {
                    Alignment = ToolStripItemAlignment.Right
                };
                _menuCampanaItem.Click += BtnCampana_Click;
                menu.Items.Add(_menuCampanaItem);
            }
        }

        private void AjustarZOrderControles()
        {
            if (_hostForm == null) return;

            if (_pnlNotificacionesHistorial != null)
            {
                _hostForm.Controls.SetChildIndex(_pnlNotificacionesHistorial, 0); // Al frente
            }
            if (_mainContainer != null)
            {
                _hostForm.Controls.SetChildIndex(_mainContainer, 1);
            }
            if (_pnlResilienceBanner != null)
            {
                _hostForm.Controls.SetChildIndex(_pnlResilienceBanner, 2);
            }
            foreach (Control ctrl in _hostForm.Controls)
            {
                if (ctrl is MenuStrip)
                {
                    _hostForm.Controls.SetChildIndex(ctrl, 3);
                    break;
                }
            }
        }

        private async void OnNotificationReceived(string tipo, int ticketId, string mensaje)
        {
            if (_hostForm == null) return;

            if (_hostForm.InvokeRequired)
            {
                _hostForm.BeginInvoke(new Action(() => OnNotificationReceived(tipo, ticketId, mensaje)));
                return;
            }

            // Guardar en almacenamiento local persistente
            await _storageService.GuardarNotificacionAsync(SesionSistema.IdUsuario, ticketId, mensaje);

            MessageBox.Show(mensaje, "Notificación de HSis", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Recargar datos específicos del Dashboard
            if (_onDataReloadCallback != null)
            {
                _ = _onDataReloadCallback();
            }

            // Recargar listado en la interfaz
            await CargarHistorialNotificacionesAsync();
        }

        private async Task CargarHistorialNotificacionesAsync()
        {
            if (_flpNotificaciones == null) return;

            _flpNotificaciones.Controls.Clear();
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
            if (_menuCampanaItem != null)
            {
                _menuCampanaItem.Text = _notificacionesNoLeidas > 0 ? $"🔔 ({_notificacionesNoLeidas}) 🔴" : $"🔔 ({_notificacionesNoLeidas})";
                _menuCampanaItem.ForeColor = _notificacionesNoLeidas > 0 ? Color.Red : Color.Black;
                _menuCampanaItem.Font = new Font("Segoe UI", 10F, _notificacionesNoLeidas > 0 ? FontStyle.Bold : FontStyle.Regular);
            }
        }

        private void AgregarNotificacionAUI(NotificacionLocal notif)
        {
            if (_flpNotificaciones == null) return;

            var pnlItem = new Panel
            {
                Width = _flpNotificaciones.ClientSize.Width - 10,
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

            _flpNotificaciones.Controls.Add(pnlItem);
        }

        private async void AbrirDetalleYMarcarLeido(NotificacionLocal notif, Panel pnlItem)
        {
            if (_hostForm == null) return;

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

            // Crear el formulario de detalle correcto según el rol de la sesión actual
            Form detailForm;
            if (SesionSistema.IdRolUsuario == 3) // Cliente
            {
                detailForm = _formFactory.CreateDetalleCliente(notif.TicketId);
            }
            else // Admin o Tecnico
            {
                detailForm = _formFactory.CreateTicketDetalle(notif.TicketId);
            }

            using (detailForm)
            {
                detailForm.ShowDialog();
            }

            if (_onDataReloadCallback != null)
            {
                _ = _onDataReloadCallback();
            }
        }

        private async void BtnLimpiarNotif_Click(object? sender, EventArgs e)
        {
            await _storageService.LimpiarTodasAsync(SesionSistema.IdUsuario);
            _notificacionesNoLeidas = 0;
            ActualizarCampanaBadge();
            _flpNotificaciones?.Controls.Clear();
        }

        private void BtnCampana_Click(object? sender, EventArgs e)
        {
            if (_pnlNotificacionesHistorial != null)
            {
                _pnlNotificacionesHistorial.Visible = !_pnlNotificacionesHistorial.Visible;
                if (_pnlNotificacionesHistorial.Visible)
                {
                    _pnlNotificacionesHistorial.BringToFront();
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
            if (_onDataReloadCallback != null)
            {
                _ = _onDataReloadCallback();
            }
        }

        private void OnDisconnected()
        {
            ActualizarEstadoConexion(false, "⚠️ Sin conexión con el servidor de notificaciones. Intentando reconectar...", Color.FromArgb(231, 76, 60)); // Rojo
        }

        private void ActualizarEstadoConexion(bool conectado, string mensaje, Color colorFondo)
        {
            if (_hostForm == null) return;

            if (_hostForm.InvokeRequired)
            {
                _hostForm.BeginInvoke(new Action(() => ActualizarEstadoConexion(conectado, mensaje, colorFondo)));
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
