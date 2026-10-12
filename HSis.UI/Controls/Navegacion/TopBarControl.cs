#nullable enable
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using FontAwesome.Sharp;
using HSis.Contracts.Services;
using HSis.UI.Forms.Otros;
using HSis.UI.Helpers;

namespace HSis.UI.Controls
{
    [SupportedOSPlatform("windows")]
    public class TopBarControl : UserControl
    {
        public event EventHandler? NotificacionesClic;
        public event EventHandler? HamburguesaClic;
        public event EventHandler? HamburguesaDobleClic;
        public event EventHandler? BotonIzquierdoClic;
        public event EventHandler? CerrarSesionClic;

        // Fuentes y recursos GDI estáticos reutilizables para máximo rendimiento
        private static readonly Font FontTitulo = new("Segoe UI", 13f, FontStyle.Bold);
        private static readonly Font FontSub = new("Segoe UI", 8.5f, FontStyle.Regular);
        private static readonly Font FontPill = new("Segoe UI Semibold", 8.5f, FontStyle.Bold);
        private static readonly Font FontBadge = new("Segoe UI", 7.5f, FontStyle.Bold);
        private static readonly Font FontAvatar = new("Segoe UI", 9f, FontStyle.Bold);
        private static readonly Font FontNom = new("Segoe UI", 8.5f, FontStyle.Bold);
        private static readonly Font FontRol = new("Segoe UI", 7.5f, FontStyle.Regular);
        private static readonly Font FontSalir = new("Segoe UI", 8.5f, FontStyle.Bold);
        private static readonly Font FontBotonKardex = new("Segoe UI Semibold", 8.5f, FontStyle.Bold);

        private static readonly Bitmap BmpHamburguesa = IconChar.Bars.ToBitmap(Color.FromArgb(51, 65, 85), 18);
        private static readonly Bitmap BmpKardexNormal = IconChar.BoxesStacked.ToBitmap(Color.FromArgb(37, 99, 235), 15);
        private static readonly Bitmap BmpKardexHover = IconChar.BoxesStacked.ToBitmap(Color.FromArgb(29, 78, 216), 15);
        private static readonly Bitmap BmpCampana = IconChar.Bell.ToBitmap(TemaVisual.TextoPrincipal, 18);
        private static readonly Bitmap BmpSalirNormal = IconChar.SignOutAlt.ToBitmap(Color.FromArgb(71, 85, 105), 14);
        private static readonly Bitmap BmpSalirHover = IconChar.SignOutAlt.ToBitmap(Color.FromArgb(220, 38, 38), 14);

        private static readonly StringFormat SfCentro = new() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        private static readonly StringFormat SfNearCenter = new() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };

        private string _titulo = "Panel Principal";
        private string _subtitulo = "Mesa de Servicio";
        private bool _conectado = true;
        private string _textoConexion = "En Línea";
        private int _notificacionesNoLeidas = 0;

        private bool _hoverHamburguesa = false;
        private bool _hoverCampana = false;
        private bool _hoverUsuario = false;
        private bool _hoverSalir = false;
        private bool _mostrarHamburguesa = true;
        private IAlmacenamientoCredencialesLocal? _sessionCache;
        private IAdministradorSesionUsuario? _sesionUsuario;
        private ContextMenuStrip? _menuHamburguesa;
        private readonly List<ItemSidebar> _itemsHamburguesa = [];
        private string _itemActivoClave = string.Empty;
        private Action<string>? _onItemSelected;

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

        public enum ModoBotonIzquierdoTopBar
        {
            Hamburguesa,
            Kardex
        }

        private ModoBotonIzquierdoTopBar _modoBotonIzquierdo = ModoBotonIzquierdoTopBar.Hamburguesa;
        private readonly ToolTip _toolTip = new() { InitialDelay = 300, AutoPopDelay = 4000 };

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ModoBotonIzquierdoTopBar ModoBotonIzquierdo
        {
            get => _modoBotonIzquierdo;
            set
            {
                if (_modoBotonIzquierdo == value) return;
                _modoBotonIzquierdo = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool MostrarHamburguesa
        {
            get => _mostrarHamburguesa;
            set
            {
                if (_mostrarHamburguesa == value) return;

                _mostrarHamburguesa = value;
                _hoverHamburguesa = false;
                Invalidate();
            }
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
                _toolTip.RemoveAll();
                Cursor = Cursors.Default;
                Invalidate();
            };
            MouseClick += TopBarControl_MouseClick;
            MouseDoubleClick += TopBarControl_MouseDoubleClick;
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
            Action<string> onItemSelected)
        {
            _itemsHamburguesa.Clear();
            _itemsHamburguesa.AddRange(items);
            _itemActivoClave = itemInicial;
            _onItemSelected = onItemSelected;

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

        }

        private Rectangle ObtenerRectHamburguesa() => new Rectangle(12, 13, 38, 38);

        private Rectangle ObtenerRectBotonKardex()
        {
            float posX = 20;
            if (_mostrarHamburguesa && _modoBotonIzquierdo == ModoBotonIzquierdoTopBar.Hamburguesa)
            {
                posX = 58;
            }

            using var g = CreateGraphics();
            var sizeTitulo = g.MeasureString(_titulo, FontTitulo);
            var sizeSub = g.MeasureString(_subtitulo, FontSub);
            float maxTextoAncho = Math.Max(sizeTitulo.Width, sizeSub.Width);

            int btnX = (int)(posX + maxTextoAncho + 20);
            int btnW = 195;
            int btnH = 34;
            int btnY = 12;
            return new Rectangle(btnX, btnY, btnW, btnH);
        }

        private Rectangle ObtenerRectBotonAccionIzquierda()
        {
            return _modoBotonIzquierdo == ModoBotonIzquierdoTopBar.Kardex
                ? ObtenerRectBotonKardex()
                : ObtenerRectHamburguesa();
        }

        private Rectangle ObtenerRectBotonSalir() => new Rectangle(Width - 14 - 78, 15, 78, 34);
        private Rectangle ObtenerRectUsuario() => new Rectangle(ObtenerRectBotonSalir().Left - 10 - 150, 12, 150, 40);
        public bool EsPuntoDeCampana(Point puntoPantalla) => ObtenerRectCampana().Contains(PointToClient(puntoPantalla));

        private Rectangle ObtenerRectCampana() => new Rectangle(ObtenerRectUsuario().Left - 14 - 38, 13, 38, 38);
        private Rectangle ObtenerRectPill()
        {
            int pillW = 108;
            return new Rectangle(ObtenerRectCampana().Left - 10 - pillW, 18, pillW, 28);
        }

        private void TopBarControl_MouseMove(object? sender, MouseEventArgs e)
        {
            bool hHamburguesa = _mostrarHamburguesa && ObtenerRectBotonAccionIzquierda().Contains(e.Location);
            bool hCampana = ObtenerRectCampana().Contains(e.Location);
            bool hUsuario = ObtenerRectUsuario().Contains(e.Location);
            bool hSalir = ObtenerRectBotonSalir().Contains(e.Location);

            if (_hoverHamburguesa != hHamburguesa || _hoverCampana != hCampana || _hoverUsuario != hUsuario || _hoverSalir != hSalir)
            {
                _hoverHamburguesa = hHamburguesa;
                _hoverCampana = hCampana;
                _hoverUsuario = hUsuario;
                _hoverSalir = hSalir;

                if (hHamburguesa && _modoBotonIzquierdo == ModoBotonIzquierdoTopBar.Hamburguesa)
                {
                    _toolTip.SetToolTip(this, "Menú de Navegación");
                }
                else
                {
                    _toolTip.RemoveAll();
                }

                Cursor = (hHamburguesa || hCampana || hUsuario || hSalir) ? Cursors.Hand : Cursors.Default;
                Invalidate();
            }
        }

        private void TopBarControl_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            if (_mostrarHamburguesa && ObtenerRectBotonAccionIzquierda().Contains(e.Location))
            {
                if (_modoBotonIzquierdo == ModoBotonIzquierdoTopBar.Kardex)
                {
                    BotonIzquierdoClic?.Invoke(this, EventArgs.Empty);
                    return;
                }

                if (e.Clicks != 1) return;

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

        private void TopBarControl_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || !_mostrarHamburguesa || !ObtenerRectHamburguesa().Contains(e.Location))
                return;

            if (_modoBotonIzquierdo == ModoBotonIzquierdoTopBar.Kardex)
                return;

            _menuHamburguesa?.Close();
            HamburguesaDobleClic?.Invoke(this, EventArgs.Empty);
        }

        private void MostrarInfoUsuario()
        {
            string rol = (_sesionUsuario?.EsAdmin ?? false) ? "Administrador" : ((_sesionUsuario?.EsTecnico ?? false) ? "Técnico" : "Cliente");
            string depto = _sesionUsuario?.UsuarioActual?.DepartamentoNombre ?? "Sin Asignar";
            string puesto = _sesionUsuario?.UsuarioActual?.PuestoNombre ?? "Sin Asignar";
            string sucursal = _sesionUsuario?.UsuarioActual?.SucursalNombre ?? "Sin Asignar";

        using var dialogo = new InformacionSesionForm(
            FindForm(),
            _sesionUsuario?.NombreUsuario ?? "Desconocido",
            rol,
            depto,
            puesto,
            sucursal);
        dialogo.ShowDialog();
        }

        private void EjecutarCierreSesion()
        {
            if (DialogoUIHelper.Confirmar("¿Estás seguro de que deseas cerrar sesión?", "Cerrar Sesión", FindForm()))
            {
                _sessionCache?.ClearCredentials();
                _sesionUsuario?.CerrarSesion();
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

            // 1. Título y Subtítulo
            float posTitulosX = (_mostrarHamburguesa && _modoBotonIzquierdo == ModoBotonIzquierdoTopBar.Hamburguesa) ? 58 : 20;
            g.DrawString(_titulo, FontTitulo, TemaVisual.BrushTextoPrincipal, new PointF(posTitulosX, 11));
            using (var brushSub = new SolidBrush(TemaVisual.TextoSecundario))
            {
                g.DrawString(_subtitulo, FontSub, brushSub, new PointF(posTitulosX, 36));
            }

            // 2. Botón Izquierdo (Menú Hamburguesa o Botón Kardex a la derecha de títulos)
            if (_mostrarHamburguesa)
            {
                if (_modoBotonIzquierdo == ModoBotonIzquierdoTopBar.Hamburguesa)
                {
                    var rectHam = ObtenerRectHamburguesa();
                    if (_hoverHamburguesa)
                    {
                        using var pathH = TemaVisual.CrearRectanguloRedondeado(rectHam, 8);
                        g.FillPath(TemaVisual.BrushFondoTenue, pathH);
                    }

                    int hamX = rectHam.X + (rectHam.Width - BmpHamburguesa.Width) / 2;
                    int hamY = rectHam.Y + (rectHam.Height - BmpHamburguesa.Height) / 2;
                    g.DrawImage(BmpHamburguesa, hamX, hamY);
                }
                else if (_modoBotonIzquierdo == ModoBotonIzquierdoTopBar.Kardex)
                {
                    var rectKardex = ObtenerRectBotonKardex();
                    Color bgBtn = _hoverHamburguesa ? Color.FromArgb(239, 246, 255) : Color.FromArgb(248, 250, 252);
                    Color borderBtn = _hoverHamburguesa ? Color.FromArgb(191, 219, 254) : Color.FromArgb(226, 232, 240);
                    Color fgBtn = _hoverHamburguesa ? Color.FromArgb(29, 78, 216) : Color.FromArgb(37, 99, 235);

                    using (var pathK = TemaVisual.CrearRectanguloRedondeado(rectKardex, 8))
                    {
                        using (var brushBg = new SolidBrush(bgBtn))
                        {
                            g.FillPath(brushBg, pathK);
                        }
                        using (var penBorde = new Pen(borderBtn, 1f))
                        {
                            g.DrawPath(penBorde, pathK);
                        }
                    }

                    var bmpIcon = _hoverHamburguesa ? BmpKardexHover : BmpKardexNormal;
                    int iconX = rectKardex.X + 10;
                    int iconY = rectKardex.Y + (rectKardex.Height - bmpIcon.Height) / 2;
                    g.DrawImage(bmpIcon, iconX, iconY);

                    using var brushTxt = new SolidBrush(fgBtn);
                    var rectTexto = new Rectangle(rectKardex.X + 28, rectKardex.Y, rectKardex.Width - 32, rectKardex.Height);
                    g.DrawString("Ver Kardex de inventario", FontBotonKardex, brushTxt, rectTexto, SfNearCenter);
                }
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
            {
                var rectTxt = new Rectangle(rectPill.X + 22, rectPill.Y, rectPill.Width - 24, rectPill.Height);
                g.DrawString(_textoConexion, FontPill, brushPillTxt, rectTxt, SfNearCenter);
            }

            // 4. Botón Campana
            var rectCampana = ObtenerRectCampana();
            if (_hoverCampana)
            {
                using var pathC = TemaVisual.CrearRectanguloRedondeado(rectCampana, 8);
                g.FillPath(TemaVisual.BrushFondoTenue, pathC);
            }

            int campX = rectCampana.X + (rectCampana.Width - BmpCampana.Width) / 2;
            int campY = rectCampana.Y + (rectCampana.Height - BmpCampana.Height) / 2;
            g.DrawImage(BmpCampana, campX, campY);

            if (_notificacionesNoLeidas > 0)
            {
                var rectBadge = new Rectangle(rectCampana.Right - 14, rectCampana.Top - 2, 18, 18);
                using var brushBadge = new SolidBrush(Color.FromArgb(239, 68, 68));
                g.FillEllipse(brushBadge, rectBadge);

                g.DrawString(_notificacionesNoLeidas > 9 ? "9+" : _notificacionesNoLeidas.ToString(), FontBadge, Brushes.White, rectBadge, SfCentro);
            }

            // 5. Divisor sutil
            var rectUser = ObtenerRectUsuario();
            int divX = rectUser.Left - 8;
            g.DrawLine(TemaVisual.PenBordeSutil, divX, 18, divX, 46);

            // 6. Chip de Usuario (Avatar + Nombre + Rol)
            if (_hoverUsuario)
            {
                using var pathU = TemaVisual.CrearRectanguloRedondeado(rectUser, 8);
                g.FillPath(TemaVisual.BrushFondoApp, pathU);
            }

            // Círculo de Avatar con iniciales
            string nombre = _sesionUsuario?.NombreUsuario ?? string.Empty;
            string iniciales = string.IsNullOrWhiteSpace(nombre) ? "U" : (nombre.Length >= 2 ? nombre[..2].ToUpperInvariant() : nombre.ToUpperInvariant());
            var rectAvatar = new Rectangle(rectUser.X + 4, rectUser.Y + 4, 32, 32);

            using (var brushAvatar = new SolidBrush(Color.FromArgb(37, 99, 235)))
            {
                g.FillEllipse(brushAvatar, rectAvatar);
            }

            g.DrawString(iniciales, FontAvatar, Brushes.White, rectAvatar, SfCentro);

            // Nombre y Rol del usuario
            string nombreCorto = nombre.Length > 12 ? string.Concat(nombre.AsSpan(0, 12), "...") : nombre;
            g.DrawString(nombreCorto, FontNom, TemaVisual.BrushTextoPrincipal, new PointF(rectUser.X + 40, rectUser.Y + 4));

            string rolTexto = (_sesionUsuario?.EsAdmin ?? false) ? "Administrador" : ((_sesionUsuario?.EsTecnico ?? false) ? "Técnico" : "Cliente");
            using (var brushRol = new SolidBrush(TemaVisual.TextoSecundario))
            {
                g.DrawString(rolTexto, FontRol, brushRol, new PointF(rectUser.X + 40, rectUser.Y + 21));
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

                var bmpSalir = _hoverSalir ? BmpSalirHover : BmpSalirNormal;
                g.DrawImage(bmpSalir, rectSalir.X + 11, rectSalir.Y + 10);

                using (var brushSalirTxt = new SolidBrush(fgSalir))
                {
                    g.DrawString("Salir", FontSalir, brushSalirTxt, new PointF(rectSalir.X + 31, rectSalir.Y + 9));
                }
            }

            // Línea divisoria inferior
            g.DrawLine(TemaVisual.PenBordeSutil, 0, Height - 1, Width, Height - 1);
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
