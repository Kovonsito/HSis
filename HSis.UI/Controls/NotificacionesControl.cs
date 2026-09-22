#nullable enable
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using FontAwesome.Sharp;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.UI.Services;
using HSis.UI.Factories;
using HSis.UI.Helpers;

namespace HSis.UI.Controls
{
    [SupportedOSPlatform("windows")]
    public partial class NotificacionesControl : UserControl
    {
        private readonly List<NotificacionLocal> _notificaciones = new();
        private IFabricaFormularios? _fabricaFormularios;
        private IAdministradorSesionUsuario? _contextoSesion;
        private IClienteSignalRNotificaciones? _clienteNotificaciones;
        private IBusEventosNotificaciones? _eventBus;
        private Func<Task>? _callbackRecargaDatos;
        private TopBarControl? _topBar;

        public NotificacionesControl()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        }

        public void Configurar(
            IFabricaFormularios fabricaFormularios,
            IAdministradorSesionUsuario contextoSesion,
            IClienteSignalRNotificaciones clienteNotificaciones,
            IBusEventosNotificaciones? eventBus = null,
            Func<Task>? callbackRecargaDatos = null)
        {
            _fabricaFormularios = fabricaFormularios;
            _contextoSesion = contextoSesion;
            _clienteNotificaciones = clienteNotificaciones;
            _eventBus = eventBus;
            _callbackRecargaDatos = callbackRecargaDatos;

            SuscribirEventos();
        }

        public void DesconectarEvents()
        {
            DesuscribirEventos();
        }

        private void SuscribirEventos()
        {
            if (_eventBus != null)
            {
                _eventBus.OnNotificacionPublicada += EnBusNotificacionPublicada;
                _eventBus.OnEstadoConexionCambiado += EnBusEstadoConexionCambiado;
            }
            else if (_clienteNotificaciones != null)
            {
                _clienteNotificaciones.OnNotificationReceived += EnNotificacionRecibida;
                _clienteNotificaciones.OnConnected += EnConectado;
            }
        }

        private void DesuscribirEventos()
        {
            if (_eventBus != null)
            {
                _eventBus.OnNotificacionPublicada -= EnBusNotificacionPublicada;
                _eventBus.OnEstadoConexionCambiado -= EnBusEstadoConexionCambiado;
            }
            else if (_clienteNotificaciones != null)
            {
                _clienteNotificaciones.OnNotificationReceived -= EnNotificacionRecibida;
                _clienteNotificaciones.OnConnected -= EnConectado;
            }
        }

        public Task CargarHistorialAsync()
        {
            int noLeidas = _notificaciones.Count(n => !n.Leido);
            ActualizarInsigniaCampana(noLeidas);
            MostrarNotificaciones(_notificaciones);
            return Task.CompletedTask;
        }

        public Task MarcarComoLeidaAsync(NotificacionLocal notif)
        {
            notif.Leido = true;
            _ = CargarHistorialAsync();
            AbrirDetalleTicket(notif.TicketId);
            return Task.CompletedTask;
        }

        public Task MarcarTodasComoLeidasAsync()
        {
            foreach (var n in _notificaciones) n.Leido = true;
            return CargarHistorialAsync();
        }

        public Task LimpiarTodasAsync()
        {
            _notificaciones.Clear();
            return CargarHistorialAsync();
        }

        private async void EnNotificacionRecibida(string tipo, int ticketId, string mensaje)
        {
            var notif = new NotificacionLocal
            {
                Id = Guid.NewGuid(),
                TicketId = ticketId,
                Mensaje = mensaje,
                Fecha = DateTime.Now,
                Leido = false
            };
            _notificaciones.Insert(0, notif);
            await CargarHistorialAsync();
            await RecargarDatosHostAsync();
        }

        private void EnConectado()
        {
            _ = RecargarDatosHostAsync();
        }

        private void EnBusNotificacionPublicada(object? sender, NotificacionEventArgs e)
        {
            EnNotificacionRecibida(e.Tipo, e.TicketId, e.Mensaje);
        }

        private void EnBusEstadoConexionCambiado(object? sender, EstadoConexionEventArgs e)
        {
            if (e.Conectado)
            {
                _ = RecargarDatosHostAsync();
            }
        }

        public void VincularTopBar(TopBarControl topBar)
        {
            _topBar = topBar;
            _topBar.NotificacionesClic += (s, e) => AlternarVisibilidad();
        }

        public void AlternarVisibilidad()
        {
            Visible = !Visible;
            if (Visible)
            {
                BringToFront();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Borde exterior moderno tipo tarjeta elevada
            using var penBorder = new Pen(Color.FromArgb(203, 213, 225), 1f);
            g.DrawRectangle(penBorder, 0, 0, Width - 1, Height - 1);

            // Línea divisoria bajo el header
            using var penHeader = new Pen(Color.FromArgb(226, 232, 240), 1f);
            g.DrawLine(penHeader, 0, pnlHeader.Height, Width, pnlHeader.Height);
        }

        public void ActualizarInsigniaCampana(int noLeidas)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => ActualizarInsigniaCampana(noLeidas)));
                return;
            }

            if (_topBar != null)
            {
                _topBar.NotificacionesNoLeidas = noLeidas;
            }

            if (lblBadgeCount != null)
            {
                lblBadgeCount.Visible = noLeidas > 0;
                lblBadgeCount.Text = noLeidas > 99 ? "99+" : noLeidas.ToString();
            }
        }

        public void MostrarNotificaciones(IEnumerable<NotificacionLocal> notificaciones)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => MostrarNotificaciones(notificaciones)));
                return;
            }

            flpNotificaciones.SuspendLayout();
            flpNotificaciones.Controls.Clear();

            var lista = notificaciones.ToList();
            if (lista.Count == 0)
            {
                var pnlVacio = new Panel
                {
                    Width = flpNotificaciones.ClientSize.Width - 24,
                    Height = 220,
                    BackColor = Color.Transparent
                };

                var picVacio = new PictureBox
                {
                    Image = IconChar.BellSlash.ToBitmap(Color.FromArgb(148, 163, 184), 36),
                    SizeMode = PictureBoxSizeMode.CenterImage,
                    Size = new Size(50, 50),
                    Location = new Point((pnlVacio.Width - 50) / 2, 45)
                };

                var lblVacio = new Label
                {
                    Text = "No tienes notificaciones",
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(71, 85, 105),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = new Point(10, 105),
                    Size = new Size(pnlVacio.Width - 20, 24)
                };

                var lblSubVacio = new Label
                {
                    Text = "Te avisaremos cuando ocurran actualizaciones.",
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                    ForeColor = Color.FromArgb(148, 163, 184),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = new Point(10, 130),
                    Size = new Size(pnlVacio.Width - 20, 20)
                };

                pnlVacio.Controls.Add(picVacio);
                pnlVacio.Controls.Add(lblVacio);
                pnlVacio.Controls.Add(lblSubVacio);
                flpNotificaciones.Controls.Add(pnlVacio);
            }
            else
            {
                foreach (var notif in lista)
                {
                    var pnlItem = CrearItemNotificacion(notif);
                    flpNotificaciones.Controls.Add(pnlItem);
                }
            }

            flpNotificaciones.ResumeLayout();
        }

        public async Task RecargarDatosHostAsync()
        {
            if (_callbackRecargaDatos != null)
            {
                if (InvokeRequired)
                {
                    await Task.Run(() => Invoke(new Action(async () => await _callbackRecargaDatos())));
                }
                else
                {
                    await _callbackRecargaDatos();
                }
            }
        }

        public void AbrirDetalleTicket(int ticketId)
        {
            if (_fabricaFormularios == null || _contextoSesion == null) return;

            Form detailForm = _contextoSesion.IdRolUsuario == 3
                ? _fabricaFormularios.CrearDetalleCliente(ticketId)
                : _fabricaFormularios.CrearTicketDetalle(ticketId);

            using (detailForm)
            {
                detailForm.ShowDialog();
            }
        }

        private Panel CrearItemNotificacion(NotificacionLocal notif)
        {
            int itemW = Math.Max(flpNotificaciones.ClientSize.Width - 26, 300);
            int itemH = 76;

            var pnlItem = new Panel
            {
                Width = itemW,
                Height = itemH,
                BackColor = notif.Leido ? Color.White : Color.FromArgb(248, 250, 252),
                Margin = new Padding(0, 0, 0, 8),
                Cursor = Cursors.Hand
            };

            bool isHovered = false;

            pnlItem.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var rect = new Rectangle(0, 0, pnlItem.Width - 1, pnlItem.Height - 1);
                using var path = TemaVisual.CrearRectanguloRedondeado(rect, 8);

                Color bg = isHovered
                    ? Color.FromArgb(241, 245, 249)
                    : (notif.Leido ? Color.White : Color.FromArgb(248, 250, 252));

                using var brushBg = new SolidBrush(bg);
                g.FillPath(brushBg, path);

                using var penBorder = new Pen(notif.Leido ? Color.FromArgb(226, 232, 240) : Color.FromArgb(191, 219, 254), 1f);
                g.DrawPath(penBorder, path);

                if (!notif.Leido)
                {
                    using var brushAccent = new SolidBrush(Color.FromArgb(37, 99, 235));
                    g.FillRectangle(brushAccent, 0, 6, 4, pnlItem.Height - 12);

                    using var brushDot = new SolidBrush(Color.FromArgb(37, 99, 235));
                    g.FillEllipse(brushDot, pnlItem.Width - 16, 12, 8, 8);
                }
            };

            pnlItem.MouseEnter += (s, e) => { isHovered = true; pnlItem.Invalidate(); };
            pnlItem.MouseLeave += (s, e) => { isHovered = false; pnlItem.Invalidate(); };

            var picIcon = new PictureBox
            {
                Size = new Size(24, 24),
                Location = new Point(12, 14),
                Image = IconChar.TicketAlt.ToBitmap(notif.Leido ? Color.FromArgb(148, 163, 184) : Color.FromArgb(37, 99, 235), 18),
                SizeMode = PictureBoxSizeMode.CenterImage,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };

            var lblMsg = new Label
            {
                Text = notif.Mensaje,
                Font = new Font("Segoe UI", 9F, notif.Leido ? FontStyle.Regular : FontStyle.Bold),
                ForeColor = notif.Leido ? Color.FromArgb(71, 85, 105) : Color.FromArgb(15, 23, 42),
                Location = new Point(42, 10),
                Size = new Size(itemW - 68, 38),
                AutoEllipsis = true,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };

            var lblFecha = new Label
            {
                Text = notif.Fecha.ToString("dd/MM/yyyy HH:mm"),
                Font = new Font("Segoe UI", 7.8F, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(42, 50),
                Size = new Size(itemW - 70, 18),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };

            async void ClickAccion(object? s, EventArgs e)
            {
                await MarcarComoLeidaAsync(notif);
            }

            pnlItem.Click += ClickAccion;
            picIcon.Click += ClickAccion;
            lblMsg.Click += ClickAccion;
            lblFecha.Click += ClickAccion;

            pnlItem.Controls.Add(picIcon);
            pnlItem.Controls.Add(lblMsg);
            pnlItem.Controls.Add(lblFecha);

            return pnlItem;
        }

        private async void BtnMarcarTodasLeidas_Click(object sender, EventArgs e)
        {
            await MarcarTodasComoLeidasAsync();
        }

        private async void BtnLimpiar_Click(object sender, EventArgs e)
        {
            await LimpiarTodasAsync();
        }
    }
}
