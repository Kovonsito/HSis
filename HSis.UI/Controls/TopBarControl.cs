#nullable enable
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using FontAwesome.Sharp;
using HSis.Contracts.Services;
using HSis.UI.Services;
using HSis.UI.Helpers;

namespace HSis.UI.Controls
{
    [SupportedOSPlatform("windows")]
    public class TopBarControl : UserControl
    {
        public event EventHandler? NotificacionesClic;
        public event EventHandler? HamburguesaClic;
        public event EventHandler? CerrarSesionClic;

        private string _titulo = "Panel Principal";
        private string _subtitulo = "Mesa de Servicio";
        private bool _conectado = true;
        private string _textoConexion = "En Línea";
        private int _notificacionesNoLeidas = 0;

        private bool _hoverHamburguesa = false;
        private bool _hoverCampana = false;
        private bool _hoverUsuario = false;
        private bool _hoverSalir = false;
        private IAlmacenamientoCredencialesLocal? _sessionCache;
        private IAdministradorSesionUsuario? _sesionUsuario;
        private ContextMenuStrip? _menuHamburguesa;
        private readonly List<ItemSidebar> _itemsHamburguesa = new();
        private string _itemActivoClave = string.Empty;
        private Action<string>? _onItemSelected;
        private Action? _onToggleSidebar;
        private Func<bool>? _isSidebarVisible;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Titulo
        {
            get => _titulo;
            set { _titulo = value; Invalidate(); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Subtitulo
        {
            get => _subtitulo;
            set { _subtitulo = value; Invalidate(); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int NotificacionesNoLeidas
        {
            get => _notificacionesNoLeidas;
            set { _notificacionesNoLeidas = Math.Max(0, value); Invalidate(); }
        }

        public TopBarControl()
        {
            Dock = DockStyle.Top;
            Height = 56;
            BackColor = Color.White;
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);

            MouseMove += TopBarControl_MouseMove;
            MouseLeave += (s, e) =>
            {
                _hoverHamburguesa = false;
                _hoverCampana = false;
                _hoverUsuario = false;
                _hoverSalir = false;
                Cursor = Cursors.Default;
                Invalidate();
            };
            MouseClick += TopBarControl_MouseClick;
        }

        public void ConfigurarSesion(IAlmacenamientoCredencialesLocal sessionCache, IAdministradorSesionUsuario? sesionUsuario = null)
        {
            _sessionCache = sessionCache;
            if (sesionUsuario != null)
            {
                _sesionUsuario = sesionUsuario;
            }
            Invalidate();
        }

        public void ActualizarConexion(bool conectado, string? mensaje = null)
        {
            _conectado = conectado;
            _textoConexion = conectado ? "En Línea" : (!string.IsNullOrEmpty(mensaje) ? mensaje : "Reconectando...");
            Invalidate();
        }

        public void ConfigurarMenuHamburguesa(
            IEnumerable<ItemSidebar> items,
            string itemInicial,
            Action<string> onItemSelected,
            Action? onToggleSidebar = null,
            Func<bool>? isSidebarVisible = null)
        {
            _itemsHamburguesa.Clear();
            _itemsHamburguesa.AddRange(items);
            _itemActivoClave = itemInicial;
            _onItemSelected = onItemSelected;
            _onToggleSidebar = onToggleSidebar;
            _isSidebarVisible = isSidebarVisible;

            ReconstruirMenuHamburguesa();
        }

        public void ActualizarItemActivo(string clave)
        {
            if (_itemActivoClave != clave)
            {
                _itemActivoClave = clave;
                ReconstruirMenuHamburguesa();
            }
        }

        private void ReconstruirMenuHamburguesa()
        {
            _menuHamburguesa?.Dispose();
            _menuHamburguesa = new ContextMenuStrip
            {
                Renderer = new MenuHamburguesaRenderer(),
                ShowImageMargin = true,
                Font = new Font("Segoe UI", 9.5f),
                BackColor = Color.White,
                DropShadowEnabled = true
            };

            var lblHeader = new ToolStripLabel("  OPCIONES DE NAVEGACIÓN")
            {
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = Color.FromArgb(148, 163, 184)
            };
            _menuHamburguesa.Items.Add(lblHeader);
            _menuHamburguesa.Items.Add(new ToolStripSeparator());

            foreach (var item in _itemsHamburguesa)
            {
                bool esActivo = item.Clave == _itemActivoClave;
                string texto = item.Titulo + (item.BadgeCount > 0 ? $" ({item.BadgeCount})" : "");

                var menuItem = new ToolStripMenuItem(texto)
                {
                    Tag = item.Clave,
                    Font = new Font("Segoe UI", 9.5f, esActivo ? FontStyle.Bold : FontStyle.Regular),
                    ForeColor = esActivo ? Color.FromArgb(37, 99, 235) : Color.FromArgb(30, 41, 59)
                };

                if (item.Icono != IconChar.None)
                {
                    menuItem.Image = item.Icono.ToBitmap(esActivo ? Color.FromArgb(37, 99, 235) : Color.FromArgb(100, 116, 139), 18);
                }

                menuItem.Click += (s, e) =>
                {
                    _itemActivoClave = item.Clave;
                    ReconstruirMenuHamburguesa();
                    _onItemSelected?.Invoke(item.Clave);
                };

                _menuHamburguesa.Items.Add(menuItem);
            }

            if (_onToggleSidebar != null)
            {
                _menuHamburguesa.Items.Add(new ToolStripSeparator());
                bool lateralVisible = _isSidebarVisible?.Invoke() ?? true;
                string toggleTexto = lateralVisible ? "  Ocultar Barra Lateral" : "  Mostrar Barra Lateral";

                var itemToggle = new ToolStripMenuItem(toggleTexto)
                {
                    Font = new Font("Segoe UI", 9f),
                    ForeColor = Color.FromArgb(71, 85, 105),
                    Image = (lateralVisible ? IconChar.EyeSlash : IconChar.Eye).ToBitmap(Color.FromArgb(100, 116, 139), 18)
                };

                itemToggle.Click += (s, e) =>
                {
                    _onToggleSidebar.Invoke();
                    ReconstruirMenuHamburguesa();
                };

                _menuHamburguesa.Items.Add(itemToggle);
            }
        }

        private Rectangle ObtenerRectHamburguesa() => new Rectangle(12, 13, 38, 38);
        private Rectangle ObtenerRectBotonSalir() => new Rectangle(Width - 14 - 78, 15, 78, 34);
        private Rectangle ObtenerRectUsuario() => new Rectangle(ObtenerRectBotonSalir().Left - 10 - 150, 12, 150, 40);
        private Rectangle ObtenerRectCampana() => new Rectangle(ObtenerRectUsuario().Left - 14 - 38, 13, 38, 38);
        private Rectangle ObtenerRectPill()
        {
            int pillW = 108;
            return new Rectangle(ObtenerRectCampana().Left - 10 - pillW, 18, pillW, 28);
        }

        private void TopBarControl_MouseMove(object? sender, MouseEventArgs e)
        {
            bool hHamburguesa = ObtenerRectHamburguesa().Contains(e.Location);
            bool hCampana = ObtenerRectCampana().Contains(e.Location);
            bool hUsuario = ObtenerRectUsuario().Contains(e.Location);
            bool hSalir = ObtenerRectBotonSalir().Contains(e.Location);

            if (_hoverHamburguesa != hHamburguesa || _hoverCampana != hCampana || _hoverUsuario != hUsuario || _hoverSalir != hSalir)
            {
                _hoverHamburguesa = hHamburguesa;
                _hoverCampana = hCampana;
                _hoverUsuario = hUsuario;
                _hoverSalir = hSalir;

                Cursor = (hHamburguesa || hCampana || hUsuario || hSalir) ? Cursors.Hand : Cursors.Default;
                Invalidate();
            }
        }

        private void TopBarControl_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            if (ObtenerRectHamburguesa().Contains(e.Location))
            {
                HamburguesaClic?.Invoke(this, EventArgs.Empty);
                if (_menuHamburguesa != null && _menuHamburguesa.Items.Count > 0)
                {
                    _menuHamburguesa.Show(this, new Point(ObtenerRectHamburguesa().Left, ObtenerRectHamburguesa().Bottom + 4));
                }
            }
            else if (ObtenerRectCampana().Contains(e.Location))
            {
                NotificacionesClic?.Invoke(this, EventArgs.Empty);
            }
            else if (ObtenerRectUsuario().Contains(e.Location))
            {
                MostrarInfoUsuario();
            }
            else if (ObtenerRectBotonSalir().Contains(e.Location))
            {
                EjecutarCierreSesion();
            }
        }

        private void MostrarInfoUsuario()
        {
            string rol = (_sesionUsuario?.EsAdmin ?? false) ? "Administrador" : ((_sesionUsuario?.EsTecnico ?? false) ? "Técnico" : "Cliente");
            string depto = _sesionUsuario?.UsuarioActual?.DepartamentoNombre ?? "Sin Asignar";
            string puesto = _sesionUsuario?.UsuarioActual?.PuestoNombre ?? "Sin Asignar";
            string sucursal = _sesionUsuario?.UsuarioActual?.SucursalNombre ?? "Sin Asignar";

            string info = $"Usuario: {_sesionUsuario?.NombreUsuario ?? "Desconocido"}\n" +
                          $"Rol asignado: {rol}\n\n" +
                          $"Departamento: {depto}\n" +
                          $"Puesto: {puesto}\n" +
                          $"Sucursal: {sucursal}";

            MessageBox.Show(info, "Información de Sesión", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void EjecutarCierreSesion()
        {
            var confirmResult = MessageBox.Show("¿Estás seguro de que deseas cerrar sesión?", "Cerrar Sesión", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmResult == DialogResult.Yes)
            {
                _sessionCache?.ClearCredentials();
                CerrarSesionClic?.Invoke(this, EventArgs.Empty);
                Application.Restart();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // 1. Botón Menú Hamburguesa
            var rectHam = ObtenerRectHamburguesa();
            if (_hoverHamburguesa)
            {
                using var pathH = TemaVisual.CrearRectanguloRedondeado(rectHam, 8);
                using var brushH = new SolidBrush(Color.FromArgb(241, 245, 249));
                g.FillPath(brushH, pathH);
            }

            using (var bmpHam = IconChar.Bars.ToBitmap(Color.FromArgb(51, 65, 85), 18))
            {
                int hamX = rectHam.X + (rectHam.Width - bmpHam.Width) / 2;
                int hamY = rectHam.Y + (rectHam.Height - bmpHam.Height) / 2;
                g.DrawImage(bmpHam, hamX, hamY);
            }

            // 2. Título y Subtítulo
            using (var brushTitulo = new SolidBrush(TemaVisual.TextoPrincipal))
            using (var fontTitulo = new Font("Segoe UI", 13f, FontStyle.Bold))
            {
                g.DrawString(_titulo, fontTitulo, brushTitulo, new PointF(58, 11));
            }

            using (var brushSub = new SolidBrush(TemaVisual.TextoSecundario))
            using (var fontSub = new Font("Segoe UI", 8.5f, FontStyle.Regular))
            {
                g.DrawString(_subtitulo, fontSub, brushSub, new PointF(58, 36));
            }

            // 3. Indicador SignalR
            var rectPill = ObtenerRectPill();
            using (var pathPill = TemaVisual.CrearRectanguloRedondeado(rectPill, 14))
            {
                using var brushPill = new SolidBrush(_conectado ? Color.FromArgb(236, 253, 245) : Color.FromArgb(254, 243, 199));
                g.FillPath(brushPill, pathPill);

                using var penPill = new Pen(_conectado ? Color.FromArgb(167, 243, 208) : Color.FromArgb(253, 230, 138), 1f);
                g.DrawPath(penPill, pathPill);
            }

            using (var brushDot = new SolidBrush(_conectado ? Color.FromArgb(16, 185, 129) : Color.FromArgb(245, 158, 11)))
            {
                g.FillEllipse(brushDot, rectPill.X + 10, rectPill.Y + (rectPill.Height - 8) / 2, 8, 8);
            }

            using (var brushPillTxt = new SolidBrush(_conectado ? Color.FromArgb(6, 95, 70) : Color.FromArgb(146, 64, 14)))
            using (var fontPill = new Font("Segoe UI Semibold", 8.5f, FontStyle.Bold))
            {
                var rectTxt = new Rectangle(rectPill.X + 22, rectPill.Y, rectPill.Width - 24, rectPill.Height);
                using var sfPill = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(_textoConexion, fontPill, brushPillTxt, rectTxt, sfPill);
            }

            // 4. Botón Campana
            var rectCampana = ObtenerRectCampana();
            if (_hoverCampana)
            {
                using var pathC = TemaVisual.CrearRectanguloRedondeado(rectCampana, 8);
                using var brushC = new SolidBrush(Color.FromArgb(241, 245, 249));
                g.FillPath(brushC, pathC);
            }

            using (var bmpCampana = IconChar.Bell.ToBitmap(TemaVisual.TextoPrincipal, 18))
            {
                int campX = rectCampana.X + (rectCampana.Width - bmpCampana.Width) / 2;
                int campY = rectCampana.Y + (rectCampana.Height - bmpCampana.Height) / 2;
                g.DrawImage(bmpCampana, campX, campY);
            }

            if (_notificacionesNoLeidas > 0)
            {
                var rectBadge = new Rectangle(rectCampana.Right - 14, rectCampana.Top - 2, 18, 18);
                using var brushBadge = new SolidBrush(Color.FromArgb(239, 68, 68));
                g.FillEllipse(brushBadge, rectBadge);

                using var sfB = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                using var fontB = new Font("Segoe UI", 7.5f, FontStyle.Bold);
                using var brushBTxt = new SolidBrush(Color.White);
                g.DrawString(_notificacionesNoLeidas > 9 ? "9+" : _notificacionesNoLeidas.ToString(), fontB, brushBTxt, rectBadge, sfB);
            }

            // 5. Divisor sutil
            var rectUser = ObtenerRectUsuario();
            int divX = rectUser.Left - 8;
            using (var penDiv = new Pen(Color.FromArgb(226, 232, 240), 1f))
            {
                g.DrawLine(penDiv, divX, 18, divX, 46);
            }

            // 6. Chip de Usuario (Avatar + Nombre + Rol)
            if (_hoverUsuario)
            {
                using var pathU = TemaVisual.CrearRectanguloRedondeado(rectUser, 8);
                using var brushU = new SolidBrush(Color.FromArgb(248, 250, 252));
                g.FillPath(brushU, pathU);
            }

            // Círculo de Avatar con iniciales
            string nombre = _sesionUsuario?.NombreUsuario ?? string.Empty;
            string iniciales = string.IsNullOrWhiteSpace(nombre) ? "U" : (nombre.Length >= 2 ? nombre[..2].ToUpperInvariant() : nombre.ToUpperInvariant());
            var rectAvatar = new Rectangle(rectUser.X + 4, rectUser.Y + 4, 32, 32);

            using (var brushAvatar = new SolidBrush(Color.FromArgb(37, 99, 235)))
            {
                g.FillEllipse(brushAvatar, rectAvatar);
            }

            using (var sfAv = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            using (var brushAvTxt = new SolidBrush(Color.White))
            using (var fontAv = new Font("Segoe UI", 9f, FontStyle.Bold))
            {
                g.DrawString(iniciales, fontAv, brushAvTxt, rectAvatar, sfAv);
            }

            // Nombre y Rol del usuario
            using (var brushNom = new SolidBrush(TemaVisual.TextoPrincipal))
            using (var fontNom = new Font("Segoe UI", 8.5f, FontStyle.Bold))
            {
                string nombreCorto = nombre.Length > 12 ? string.Concat(nombre.AsSpan(0, 12), "...") : nombre;
                g.DrawString(nombreCorto, fontNom, brushNom, new PointF(rectUser.X + 40, rectUser.Y + 4));
            }

            string rolTexto = (_sesionUsuario?.EsAdmin ?? false) ? "Administrador" : ((_sesionUsuario?.EsTecnico ?? false) ? "Técnico" : "Cliente");
            using (var brushRol = new SolidBrush(TemaVisual.TextoSecundario))
            using (var fontRol = new Font("Segoe UI", 7.5f, FontStyle.Regular))
            {
                g.DrawString(rolTexto, fontRol, brushRol, new PointF(rectUser.X + 40, rectUser.Y + 21));
            }

            // 7. Botón Salir / Cerrar Sesión
            var rectSalir = ObtenerRectBotonSalir();
            using (var pathSalir = TemaVisual.CrearRectanguloRedondeado(rectSalir, 6))
            {
                Color bgSalir = _hoverSalir ? Color.FromArgb(254, 226, 226) : Color.FromArgb(248, 250, 252);
                Color borderSalir = _hoverSalir ? Color.FromArgb(252, 165, 165) : Color.FromArgb(226, 232, 240);
                Color fgSalir = _hoverSalir ? Color.FromArgb(220, 38, 38) : Color.FromArgb(71, 85, 105);

                using (var brushSalir = new SolidBrush(bgSalir))
                {
                    g.FillPath(brushSalir, pathSalir);
                }

                using (var penSalir = new Pen(borderSalir, 1f))
                {
                    g.DrawPath(penSalir, pathSalir);
                }

                using (var bmpSalir = IconChar.SignOutAlt.ToBitmap(fgSalir, 14))
                {
                    g.DrawImage(bmpSalir, rectSalir.X + 11, rectSalir.Y + 10);
                }

                using (var brushSalirTxt = new SolidBrush(fgSalir))
                using (var fontSalir = new Font("Segoe UI", 8.5f, FontStyle.Bold))
                {
                    g.DrawString("Salir", fontSalir, brushSalirTxt, new PointF(rectSalir.X + 31, rectSalir.Y + 9));
                }
            }

            // Línea divisoria inferior
            using var penBorde = new Pen(TemaVisual.BordeSutil, 1f);
            g.DrawLine(penBorde, 0, Height - 1, Width, Height - 1);
        }

        private class MenuHamburguesaRenderer : ToolStripProfessionalRenderer
        {
            public MenuHamburguesaRenderer() : base(new MenuColorTable()) { }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                if (e.Item.Selected)
                {
                    var rc = new Rectangle(2, 0, e.Item.Width - 4, e.Item.Height);
                    using var brush = new SolidBrush(Color.FromArgb(241, 245, 249));
                    using var path = TemaVisual.CrearRectanguloRedondeado(rc, 4);
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.FillPath(brush, path);
                }
            }
        }

        private class MenuColorTable : ProfessionalColorTable
        {
            public override Color MenuItemSelected => Color.FromArgb(241, 245, 249);
            public override Color MenuItemSelectedGradientBegin => Color.FromArgb(241, 245, 249);
            public override Color MenuItemSelectedGradientEnd => Color.FromArgb(241, 245, 249);
            public override Color MenuItemBorder => Color.Transparent;
            public override Color MenuBorder => Color.FromArgb(226, 232, 240);
            public override Color ToolStripDropDownBackground => Color.White;
            public override Color ImageMarginGradientBegin => Color.White;
            public override Color ImageMarginGradientMiddle => Color.White;
            public override Color ImageMarginGradientEnd => Color.White;
            public override Color SeparatorDark => Color.FromArgb(226, 232, 240);
            public override Color SeparatorLight => Color.White;
        }
    }
}
