#nullable enable
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using FontAwesome.Sharp;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.UI.Factories;
using HSis.UI.Helpers;
using NotificacionLocal = HSis.UI.Models.Notificaciones.NotificacionLocal;

namespace HSis.UI.Controls
{
    [SupportedOSPlatform("windows")]
    public partial class NotificacionesControl : UserControl
    {
        private readonly List<NotificacionLocal> _notificaciones = [];
        private readonly SemaphoreSlim _sincronizacionLock = new(1, 1);
        private IFabricaFormularios? _fabricaFormularios;
        private IAdministradorSesionUsuario? _contextoSesion;
        private IClienteSignalRNotificaciones? _clienteNotificaciones;
        private INotificacionesApiClient? _notificacionesApiClient;
        private IBusEventosNotificaciones? _eventBus;
        private Func<Task>? _callbackRecargaDatos;
        private TopBarControl? _topBar;
        private EventHandler? _notificacionesClicHandler;

        public NotificacionesControl()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        }

        public void Configurar(
            IFabricaFormularios fabricaFormularios,
            IAdministradorSesionUsuario contextoSesion,
            IClienteSignalRNotificaciones clienteNotificaciones,
            INotificacionesApiClient notificacionesApiClient,
            IBusEventosNotificaciones? eventBus = null,
            Func<Task>? callbackRecargaDatos = null)
        {
            DesuscribirEventos();
            _fabricaFormularios = fabricaFormularios;
            _contextoSesion = contextoSesion;
            _clienteNotificaciones = clienteNotificaciones;
            _notificacionesApiClient = notificacionesApiClient;
            _eventBus = eventBus;
            _callbackRecargaDatos = callbackRecargaDatos;

            SuscribirEventos();
            _ = SincronizarConServidorAsync();
        }

        public void DesconectarEvents()
        {
            DesuscribirEventos();
            if (_topBar != null && _notificacionesClicHandler != null)
            {
                _topBar.NotificacionesClic -= _notificacionesClicHandler;
                _notificacionesClicHandler = null;
            }
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
            => SincronizarConServidorAsync();

        public async Task MarcarComoLeidaAsync(NotificacionLocal notif)
        {
            try
            {
                if (!notif.DbId.HasValue || _notificacionesApiClient == null)
                {
                    notif.Leido = true;
                    await CargarHistorialAsync();
                }
                else
                {
                    await _notificacionesApiClient.MarcarComoLeidaAsync(notif.DbId.Value);
                    notif.Leido = true;
                    await CargarHistorialAsync();
                }

                if (notif.TicketId.HasValue)
                {
                    AbrirDetalleTicket(notif.TicketId.Value);
                }
            }
            catch (Exception ex)
            {
                ManejadorErroresUI.Manejar(
                    ex,
                    "marcar una notificación como leída",
                    FindForm());
            }
        }

        public async Task MarcarTodasComoLeidasAsync()
        {
            try
            {
                if (_notificacionesApiClient != null)
                {
                    await _notificacionesApiClient.MarcarTodasComoLeidasAsync();
                }

                foreach (var n in _notificaciones)
                {
                    n.Leido = true;
                }

                await CargarHistorialAsync();
            }
            catch (Exception ex)
            {
                ManejadorErroresUI.Manejar(
                    ex,
                    "marcar todas las notificaciones como leídas",
                    FindForm());
            }
        }

        public async Task LimpiarTodasAsync()
        {
            try
            {
                if (_notificacionesApiClient != null)
                {
                    await _notificacionesApiClient.LimpiarTodasAsync();
                }

                _notificaciones.Clear();
                await CargarHistorialAsync();
            }
            catch (Exception ex)
            {
                ManejadorErroresUI.Manejar(
                    ex,
                    "limpiar las notificaciones",
                    FindForm());
            }
        }

        private async void EnNotificacionRecibida(NotificacionDto notification)
        {
            await EjecutarEnHiloUIAsync(async () =>
            {
                UpsertNotificacion(notification);
                MostrarNotificaciones(_notificaciones);
                ActualizarInsigniaCampana(_notificaciones.Count(n => !n.Leido));
                await RecargarDatosHostAsync();
            });
        }

        private async void EnConectado()
        {
            await SincronizarConServidorAsync();
            await RecargarDatosHostAsync();
        }

        private async void EnBusNotificacionPublicada(object? sender, NotificacionEventArgs e)
        {
            await EjecutarEnHiloUIAsync(async () =>
            {
                if (e.IdNotificacion > 0)
                {
                    UpsertNotificacion(e.Notificacion);
                    MostrarNotificaciones(_notificaciones);
                    ActualizarInsigniaCampana(_notificaciones.Count(n => !n.Leido));
                }

                await SincronizarConServidorAsync();
                await RecargarDatosHostAsync();
            });
        }

        private void EnBusEstadoConexionCambiado(object? sender, EstadoConexionEventArgs e)
        {
            if (e.Conectado)
            {
                _ = SincronizarConServidorAsync();
            }
        }

        private async Task SincronizarConServidorAsync()
        {
            if (_notificacionesApiClient == null)
            {
                return;
            }

            await _sincronizacionLock.WaitAsync();
            try
            {
                var resumen = await _notificacionesApiClient.ObtenerResumenAsync();
                await EjecutarEnHiloUIAsync(() =>
                {
                    ReemplazarNotificaciones(resumen.Notificaciones);
                    MostrarNotificaciones(_notificaciones);
                    ActualizarInsigniaCampana(resumen.NoLeidas);
                    return Task.CompletedTask;
                });
            }
            catch (Exception ex)
            {
                ManejadorErroresUI.Manejar(
                    ex,
                    "cargar las notificaciones",
                    FindForm(),
                    mostrarDialogo: false);
            }
            finally
            {
                _sincronizacionLock.Release();
            }
        }

        private Task EjecutarEnHiloUIAsync(Func<Task> operacion)
        {
            if (IsDisposed || Disposing)
            {
                return Task.CompletedTask;
            }

            if (!InvokeRequired)
            {
                return operacion();
            }

            if (!IsHandleCreated)
            {
                return Task.CompletedTask;
            }

            try
            {
                return (Task)Invoke(operacion);
            }
            catch (InvalidOperationException) when (IsDisposed || Disposing)
            {
                return Task.CompletedTask;
            }
            catch (ObjectDisposedException)
            {
                return Task.CompletedTask;
            }
        }

        private void ReemplazarNotificaciones(IEnumerable<NotificacionDto> notificaciones)
        {
            _notificaciones.Clear();
            foreach (var notification in notificaciones)
            {
                _notificaciones.Add(ConvertirNotificacion(notification));
            }
        }

        private void UpsertNotificacion(NotificacionDto notification)
        {
            var existente = notification.IdNotificacion > 0
                ? _notificaciones.FirstOrDefault(n => n.DbId == notification.IdNotificacion)
                : _notificaciones.FirstOrDefault(n =>
                    n.TicketId == notification.TicketId &&
                    n.MaterialId == notification.MaterialId &&
                    string.Equals(n.Tipo, notification.Tipo, StringComparison.Ordinal) &&
                    string.Equals(n.Mensaje, notification.Mensaje, StringComparison.Ordinal));
            if (existente != null)
            {
                existente.TicketId = notification.TicketId;
                existente.MaterialId = notification.MaterialId;
                existente.Tipo = notification.Tipo;
                existente.Mensaje = notification.Mensaje;
                existente.Fecha = notification.FechaCreacion.LocalDateTime;
                existente.Leido = notification.Leido;
                return;
            }

            _notificaciones.Insert(0, ConvertirNotificacion(notification));
        }

        private static NotificacionLocal ConvertirNotificacion(NotificacionDto notification)
            => new()
            {
                Id = Guid.NewGuid(),
                DbId = notification.IdNotificacion,
                TicketId = notification.TicketId,
                MaterialId = notification.MaterialId,
                Tipo = notification.Tipo,
                Mensaje = notification.Mensaje,
                Fecha = notification.FechaCreacion.LocalDateTime,
                Leido = notification.Leido
            };

        public void VincularTopBar(TopBarControl topBar)
        {
            if (_topBar != null && _notificacionesClicHandler != null)
            {
                _topBar.NotificacionesClic -= _notificacionesClicHandler;
            }

            _topBar = topBar;
            _notificacionesClicHandler = (_, _) => AlternarVisibilidad();
            _topBar.NotificacionesClic += _notificacionesClicHandler;
        }

        public void AlternarVisibilidad()
        {
            bool mostrar = !Visible;
            if (mostrar)
            {
                MostrarNotificaciones(_notificaciones);
                BringToFront();
            }

            Visible = mostrar;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Parent?.BackColor ?? TemaVisual.FondoApp);

            if (Width <= 1 || Height <= 1) return;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using var path = TemaVisual.CrearRectanguloRedondeado(rect, 10);
            using var brush = new SolidBrush(Color.White);
            g.FillPath(brush, path);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (Width <= 1 || Height <= 1) return;

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using var path = TemaVisual.CrearRectanguloRedondeado(rect, 10);
            using var penBorder = new Pen(TemaVisual.BordeSutil, 1f);
            g.DrawPath(penBorder, path);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (Width <= 1 || Height <= 1) return;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using var path = TemaVisual.CrearRectanguloRedondeado(rect, 10);
            Region = new Region(path);
        }

        public void ActualizarInsigniaCampana(int noLeidas)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => ActualizarInsigniaCampana(noLeidas)));
                return;
            }

            _topBar?.NotificacionesNoLeidas = noLeidas;

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
            foreach (Control control in flpNotificaciones.Controls.Cast<Control>().ToList())
            {
                flpNotificaciones.Controls.Remove(control);
                control.Dispose();
            }

            var lista = notificaciones.ToList();
            if (lista.Count == 0)
            {
                int anchoContenido = Math.Max(flpNotificaciones.ClientSize.Width - flpNotificaciones.Padding.Horizontal - 4, 260);
                var pnlVacio = new Panel
                {
                    Width = anchoContenido,
                    Height = 210,
                    BackColor = Color.White,
                    Margin = new Padding(0, 0, 0, 8)
                };

                pnlVacio.Paint += (s, e) =>
                {
                    var g = e.Graphics;
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    var rect = new Rectangle(0, 0, pnlVacio.Width - 1, pnlVacio.Height - 1);
                    using var path = TemaVisual.CrearRectanguloRedondeado(rect, 10);
                    using var brush = new SolidBrush(Color.White);
                    using var pen = new Pen(TemaVisual.BordeSutil, 1f);
                    g.FillPath(brush, path);
                    g.DrawPath(pen, path);

                    var circle = new Rectangle((pnlVacio.Width - 54) / 2, 34, 54, 54);
                    using var brushCircle = new SolidBrush(TemaVisual.PrimarioSuave);
                    g.FillEllipse(brushCircle, circle);
                    using var bmp = IconChar.BellSlash.ToBitmap(TemaVisual.TextoMuted, 28);
                    g.DrawImage(bmp, circle.X + (circle.Width - bmp.Width) / 2, circle.Y + (circle.Height - bmp.Height) / 2);
                };

                var lblVacio = new Label
                {
                    Text = "No tienes notificaciones",
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    ForeColor = TemaVisual.TextoMedio,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = new Point(10, 104),
                    Size = new Size(pnlVacio.Width - 20, 24)
                };

                var lblSubVacio = new Label
                {
                    Text = "Te avisaremos cuando ocurran actualizaciones.",
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                    ForeColor = TemaVisual.TextoMuted,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = new Point(10, 130),
                    Size = new Size(pnlVacio.Width - 20, 20)
                };

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

        public Task RecargarDatosHostAsync()
        {
            if (_callbackRecargaDatos == null)
            {
                return Task.CompletedTask;
            }

            return EjecutarEnHiloUIAsync(_callbackRecargaDatos);
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
            int itemW = Math.Max(flpNotificaciones.ClientSize.Width - flpNotificaciones.Padding.Horizontal - 4, 280);
            int itemH = 82;

            var pnlItem = new Panel
            {
                Width = itemW,
                Height = itemH,
                BackColor = Color.Transparent,
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

                var iconCircle = new Rectangle(12, 14, 32, 32);
                using var brushIconCircle = new SolidBrush(notif.Leido ? TemaVisual.FondoTenue : TemaVisual.PrimarioSuave);
                g.FillEllipse(brushIconCircle, iconCircle);

                if (!notif.Leido)
                {
                    using var brushAccent = new SolidBrush(TemaVisual.Primario);
                    g.FillRectangle(brushAccent, 0, 8, 4, pnlItem.Height - 16);

                    using var brushDot = new SolidBrush(TemaVisual.Primario);
                    g.FillEllipse(brushDot, pnlItem.Width - 16, 12, 8, 8);
                }
            };

            pnlItem.MouseEnter += (s, e) => { isHovered = true; pnlItem.Invalidate(); };
            pnlItem.MouseLeave += (s, e) => { isHovered = false; pnlItem.Invalidate(); };

            var picIcon = new PictureBox
            {
                Size = new Size(32, 32),
                Location = new Point(12, 14),
                Image = IconChar.TicketAlt.ToBitmap(notif.Leido ? TemaVisual.TextoMuted : TemaVisual.Primario, 17),
                SizeMode = PictureBoxSizeMode.CenterImage,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };

            var lblMsg = new Label
            {
                Text = notif.Mensaje,
                Font = new Font("Segoe UI", 9F, notif.Leido ? FontStyle.Regular : FontStyle.Bold),
                ForeColor = notif.Leido ? Color.FromArgb(71, 85, 105) : Color.FromArgb(15, 23, 42),
                Location = new Point(56, 10),
                Size = new Size(itemW - 82, 40),
                AutoEllipsis = true,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };

            var lblFecha = new Label
            {
                Text = notif.Fecha.ToString("dd/MM/yyyy HH:mm"),
                Font = new Font("Segoe UI", 7.8F, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(56, 56),
                Size = new Size(itemW - 82, 18),
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
