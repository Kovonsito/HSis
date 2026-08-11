#nullable enable
using System.Runtime.Versioning;
using HSis.Logic.Services;
using HSis.UI.Factories;
using HSis.UI.Helpers;

namespace HSis.UI.Services
{
    [SupportedOSPlatform("windows")]
    public class AdministradorUINotificaciones(
        INotificationClientService clienteNotificaciones,
        INotificacionStorageService servicioAlmacenamiento,
        IFabricaFormularios fabricaFormularios)
    {

        private Form? _formularioAnfitrion;
        private Control? _contenedorPrincipal;
        private Func<Task>? _callbackRecargaDatos;

        private Panel? _pnlBannerResiliencia;
        private Label? _lblBannerResiliencia;
        private Panel? _pnlNotificacionesHistorial;
        private FlowLayoutPanel? _flpNotificaciones;
        private ToolStripMenuItem? _itemMenuCampana;
        private int _notificacionesNoLeidas = 0;

        public void Adjuntar(Form formularioAnfitrion, Control contenedorPrincipal, Func<Task> callbackRecargaDatos)
        {
            _formularioAnfitrion = formularioAnfitrion;
            _contenedorPrincipal = contenedorPrincipal;
            _callbackRecargaDatos = callbackRecargaDatos;

            // 1. Inicializar banner de resiliencia y panel de notificaciones
            InicializarBannerResiliencia();

            // 2. Configurar campana en el menú principal del formulario
            ConfigurarCampanaMenu();

            // 3. Suscribirse a los eventos de SignalR
            clienteNotificaciones.OnNotificationReceived += EnNotificacionRecibida;
            clienteNotificaciones.OnReconnecting += EnReconectando;
            clienteNotificaciones.OnConnected += EnConectado;
            clienteNotificaciones.OnDisconnected += EnDesconectado;

            // 4. Limpieza automática de eventos al cerrar el formulario host
            _formularioAnfitrion.FormClosed += (s, args) =>
            {
                clienteNotificaciones.OnNotificationReceived -= EnNotificacionRecibida;
                clienteNotificaciones.OnReconnecting -= EnReconectando;
                clienteNotificaciones.OnConnected -= EnConectado;
                clienteNotificaciones.OnDisconnected -= EnDesconectado;
            };

            // 5. Ajustar orden de apilamiento visual de los controles inyectados
            AjustarOrdenZControles();

            // 6. Establecer estado de conexión inicial
            ActualizarEstadoConexion(
                clienteNotificaciones.IsConnected,
                "⚠️ Conectando al servidor de notificaciones...",
                Color.FromArgb(230, 126, 34)
            );

            // 7. Configurar evento para ocultar el panel al hacer clic fuera de él
            ConfigurarOcultarAlHacerClicFuera();

            // 8. Cargar el historial local de notificaciones
            _ = CargarHistorialNotificacionesAsync();
        }

        private void ConfigurarOcultarAlHacerClicFuera()
        {
            if (_formularioAnfitrion == null) return;

            SuscribirEventosClicRecursivo(_formularioAnfitrion);
        }

        private void SuscribirEventosClicRecursivo(Control container)
        {
            container.Click += (s, e) => OcultarSiClicFuera();

            foreach (Control ctrl in container.Controls)
            {
                if (ctrl == _pnlNotificacionesHistorial) continue; // No ocultar al hacer clic dentro del panel de notificaciones

                SuscribirEventosClicRecursivo(ctrl);
            }
        }

        private void OcultarSiClicFuera()
        {
            if (_pnlNotificacionesHistorial != null && _pnlNotificacionesHistorial.Visible)
            {
                // Verificar si el cursor del mouse está fuera de los límites del panel de notificaciones
                Point mousePos = _formularioAnfitrion?.PointToClient(Cursor.Position) ?? Point.Empty;
                if (!_pnlNotificacionesHistorial.Bounds.Contains(mousePos))
                {
                    _pnlNotificacionesHistorial.Visible = false;
                }
            }
        }

        private void InicializarBannerResiliencia()
        {
            if (_formularioAnfitrion == null) return;

            // Banner superior de resiliencia
            _pnlBannerResiliencia = new Panel
            {
                Dock = DockStyle.Top,
                Height = 35,
                BackColor = Color.FromArgb(231, 76, 60), // Rojo
                Visible = false
            };

            _lblBannerResiliencia = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "⚠️ Sin conexión con el servidor de notificaciones. Intentando reconectar..."
            };

            _pnlBannerResiliencia.Controls.Add(_lblBannerResiliencia);
            _formularioAnfitrion.Controls.Add(_pnlBannerResiliencia);

            // Panel flotante de historial de notificaciones
            _pnlNotificacionesHistorial = new Panel
            {
                Width = 330,
                Height = _formularioAnfitrion.ClientSize.Height - 35,
                Location = new Point(_formularioAnfitrion.ClientSize.Width - 330, 35),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right,
                BackColor = Color.FromArgb(245, 247, 250),
                Visible = false,
                Padding = new Padding(10)
            };

            var pnlNotifHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 65,
                Padding = new Padding(0, 0, 0, 5)
            };

            var lblNotifTitle = new Label
            {
                Text = "Notificaciones",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(0, 0),
                AutoSize = true
            };

            var btnMarcarTodasLeidas = new Button
            {
                Text = "Marcar todas leídas",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                Location = new Point(0, 32),
                Width = 145,
                Height = 26,
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnMarcarTodasLeidas.FlatAppearance.BorderSize = 1;
            btnMarcarTodasLeidas.FlatAppearance.BorderColor = Color.LightGray;
            btnMarcarTodasLeidas.Click += BtnMarcarTodasLeidas_Click;

            var btnLimpiarNotif = new Button
            {
                Text = "Limpiar todo",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                Location = new Point(152, 32),
                Width = 100,
                Height = 26,
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLimpiarNotif.FlatAppearance.BorderSize = 1;
            btnLimpiarNotif.FlatAppearance.BorderColor = Color.LightGray;
            btnLimpiarNotif.Click += BtnLimpiarNotif_Click;

            pnlNotifHeader.Controls.Add(lblNotifTitle);
            pnlNotifHeader.Controls.Add(btnMarcarTodasLeidas);
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

            _formularioAnfitrion.Controls.Add(_pnlNotificacionesHistorial);
        }

        private void ConfigurarCampanaMenu()
        {
            if (_formularioAnfitrion == null) return;

            var menu = _formularioAnfitrion.MainMenuStrip;
            if (menu != null)
            {
                _itemMenuCampana = new ToolStripMenuItem("🔔 (0)")
                {
                    Alignment = ToolStripItemAlignment.Right
                };
                _itemMenuCampana.Click += BtnCampana_Click;
                menu.Items.Add(_itemMenuCampana);
            }
        }

        private void AjustarOrdenZControles()
        {
            if (_formularioAnfitrion == null) return;

            if (_pnlNotificacionesHistorial != null)
            {
                _formularioAnfitrion.Controls.SetChildIndex(_pnlNotificacionesHistorial, 0); // Al frente
            }
            if (_contenedorPrincipal != null)
            {
                _formularioAnfitrion.Controls.SetChildIndex(_contenedorPrincipal, 1);
            }
            if (_pnlBannerResiliencia != null)
            {
                _formularioAnfitrion.Controls.SetChildIndex(_pnlBannerResiliencia, 2);
            }
            foreach (Control ctrl in _formularioAnfitrion.Controls)
            {
                if (ctrl is MenuStrip)
                {
                    _formularioAnfitrion.Controls.SetChildIndex(ctrl, 3);
                    break;
                }
            }
        }

        private async void EnNotificacionRecibida(string tipo, int ticketId, string mensaje)
        {
            if (_formularioAnfitrion == null) return;

            if (_formularioAnfitrion.InvokeRequired)
            {
                _formularioAnfitrion.BeginInvoke(new Action(() => EnNotificacionRecibida(tipo, ticketId, mensaje)));
                return;
            }

            // Guardar en almacenamiento local persistente
            await servicioAlmacenamiento.GuardarNotificacionAsync(SesionSistema.IdUsuario, ticketId, mensaje);

            // Recargar datos específicos del Dashboard inmediatamente en segundo plano
            if (_callbackRecargaDatos != null)
            {
                _ = _callbackRecargaDatos();
            }

            // Recargar historial local de notificaciones
            await CargarHistorialNotificacionesAsync();

            // Notificación discreta / Toast no bloqueante mediante Task.Run
            _ = Task.Run(() =>
            {
                MessageBox.Show(mensaje, "Notificación de HSis", MessageBoxButtons.OK, MessageBoxIcon.Information);
            });
        }

        private async Task CargarHistorialNotificacionesAsync()
        {
            if (_flpNotificaciones == null) return;

            try
            {
                await servicioAlmacenamiento.SincronizarDesdeBDAsync(SesionSistema.IdUsuario);
            }
            catch
            {
                // Ignorar errores durante la sincronización inicial
            }

            _flpNotificaciones.Controls.Clear();
            var list = await servicioAlmacenamiento.ObtenerNotificacionesAsync(SesionSistema.IdUsuario);

            _notificacionesNoLeidas = list.Count(n => !n.Leido);
            ActualizarInsigniaCampana();

            foreach (var notif in list)
            {
                AgregarNotificacionAInterfaz(notif);
            }
        }

        private void ActualizarInsigniaCampana()
        {
            if (_itemMenuCampana != null)
            {
                _itemMenuCampana.Text = _notificacionesNoLeidas > 0 ? $"🔔 ({_notificacionesNoLeidas}) 🔴" : $"🔔 ({_notificacionesNoLeidas})";
                _itemMenuCampana.ForeColor = _notificacionesNoLeidas > 0 ? Color.Red : Color.Black;
                _itemMenuCampana.Font = new Font("Segoe UI", 10F, _notificacionesNoLeidas > 0 ? FontStyle.Bold : FontStyle.Regular);
            }
        }

        private void AgregarNotificacionAInterfaz(NotificacionLocal notif)
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
            if (_formularioAnfitrion == null) return;

            if (!notif.Leido)
            {
                await servicioAlmacenamiento.MarcarComoLeidaAsync(SesionSistema.IdUsuario, notif.Id);
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
                ActualizarInsigniaCampana();
                pnlItem.Invalidate();
            }

            // Crear el formulario de detalle correcto según el rol de la sesión actual
            Form detailForm;
            if (SesionSistema.IdRolUsuario == 3) // Cliente
            {
                detailForm = fabricaFormularios.CrearDetalleCliente(notif.TicketId);
            }
            else // Admin o Tecnico
            {
                detailForm = fabricaFormularios.CrearTicketDetalle(notif.TicketId);
            }

            using (detailForm)
            {
                detailForm.ShowDialog();
            }

            if (_callbackRecargaDatos != null)
            {
                _ = _callbackRecargaDatos();
            }
        }

        private async void BtnMarcarTodasLeidas_Click(object? sender, EventArgs e)
        {
            await servicioAlmacenamiento.MarcarTodasComoLeidasAsync(SesionSistema.IdUsuario);
            await CargarHistorialNotificacionesAsync();
        }

        private async void BtnLimpiarNotif_Click(object? sender, EventArgs e)
        {
            await servicioAlmacenamiento.LimpiarTodasAsync(SesionSistema.IdUsuario);
            _notificacionesNoLeidas = 0;
            ActualizarInsigniaCampana();
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

        private void EnReconectando()
        {
            ActualizarEstadoConexion(false, "⚠️ Intentando reconectar con el servidor de notificaciones...", Color.FromArgb(230, 126, 34)); // Naranja
        }

        private void EnConectado()
        {
            ActualizarEstadoConexion(true, string.Empty, Color.Empty);
            if (_callbackRecargaDatos != null)
            {
                _ = _callbackRecargaDatos();
            }
        }

        private void EnDesconectado()
        {
            ActualizarEstadoConexion(false, "⚠️ Sin conexión con el servidor de notificaciones. Intentando reconectar...", Color.FromArgb(231, 76, 60)); // Rojo
        }

        private void ActualizarEstadoConexion(bool conectado, string mensaje, Color colorFondo)
        {
            if (_formularioAnfitrion == null) return;

            if (_formularioAnfitrion.InvokeRequired)
            {
                _formularioAnfitrion.BeginInvoke(new Action(() => ActualizarEstadoConexion(conectado, mensaje, colorFondo)));
                return;
            }

            if (_pnlBannerResiliencia != null && _lblBannerResiliencia != null)
            {
                _pnlBannerResiliencia.Visible = !conectado;
                _lblBannerResiliencia.Text = mensaje;
                _pnlBannerResiliencia.BackColor = colorFondo;
            }
        }
    }
}
