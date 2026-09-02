#nullable enable
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using FontAwesome.Sharp;
using HSis.Logic.Services;
using HSis.UI.Helpers;

namespace HSis.UI.Controls
{
    public class ItemSidebar
    {
        public string Clave { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public IconChar Icono { get; set; } = IconChar.None;
        public int BadgeCount { get; set; } = 0;
    }

    [SupportedOSPlatform("windows")]
    public class SidebarControl : UserControl
    {
        public event EventHandler<string>? ItemSeleccionado;
        public event EventHandler? CerrarSesionClic;

        private readonly List<ItemSidebar> _items = [];
        private string _itemActivoClave = string.Empty;
        private int _hoveredIndex = -1;
        private ISessionCacheService? _sessionCache;

        public SidebarControl()
        {
            Dock = DockStyle.Left;
            Width = 240;
            BackColor = TemaVisual.SidebarFondo;
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            MouseMove += SidebarControl_MouseMove;
            MouseLeave += (s, e) => { _hoveredIndex = -1; Invalidate(); };
            MouseClick += SidebarControl_MouseClick;
        }

        public void ConfigurarSesion(ISessionCacheService sessionCache)
        {
            _sessionCache = sessionCache;
            Invalidate();
        }

        public void ConfigurarItems(IEnumerable<ItemSidebar> items, string itemInicial = "")
        {
            _items.Clear();
            _items.AddRange(items);
            if (!string.IsNullOrEmpty(itemInicial))
            {
                _itemActivoClave = itemInicial;
            }
            else if (_items.Count > 0)
            {
                _itemActivoClave = _items[0].Clave;
            }
            Invalidate();
        }

        public void SeleccionarItem(string clave)
        {
            if (_itemActivoClave != clave)
            {
                _itemActivoClave = clave;
                Invalidate();
            }
        }

        public void ActualizarBadge(string clave, int badgeCount)
        {
            var item = _items.FirstOrDefault(i => i.Clave == clave);
            if (item != null && item.BadgeCount != badgeCount)
            {
                item.BadgeCount = badgeCount;
                Invalidate();
            }
        }

        private Rectangle ObtenerRectItem(int index)
        {
            int topOffset = 80;
            int itemHeight = 44;
            int marginX = 10;
            return new Rectangle(marginX, topOffset + (index * (itemHeight + 6)), Width - (marginX * 2), itemHeight);
        }

        private Rectangle ObtenerRectBotonSalir()
        {
            return new Rectangle(14, Height - 54, Width - 28, 38);
        }

        private void SidebarControl_MouseMove(object? sender, MouseEventArgs e)
        {
            int nuevoHover = -1;
            for (int i = 0; i < _items.Count; i++)
            {
                if (ObtenerRectItem(i).Contains(e.Location))
                {
                    nuevoHover = i;
                    break;
                }
            }

            if (ObtenerRectBotonSalir().Contains(e.Location))
            {
                nuevoHover = 999;
            }

            if (_hoveredIndex != nuevoHover)
            {
                _hoveredIndex = nuevoHover;
                Cursor = nuevoHover >= 0 ? Cursors.Hand : Cursors.Default;
                Invalidate();
            }
        }

        private void SidebarControl_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            for (int i = 0; i < _items.Count; i++)
            {
                if (ObtenerRectItem(i).Contains(e.Location))
                {
                    _itemActivoClave = _items[i].Clave;
                    Invalidate();
                    ItemSeleccionado?.Invoke(this, _itemActivoClave);
                    return;
                }
            }

            if (ObtenerRectBotonSalir().Contains(e.Location))
            {
                var confirmResult = MessageBox.Show("¿Estás seguro de que deseas cerrar sesión?", "Cerrar Sesión", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirmResult == DialogResult.Yes)
                {
                    _sessionCache?.ClearCredentials();
                    CerrarSesionClic?.Invoke(this, EventArgs.Empty);
                    Application.Restart();
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // 1. Logotipo y Título de cabecera
            using (var brushLogo = new SolidBrush(Color.FromArgb(37, 99, 235)))
            {
                g.FillEllipse(brushLogo, 16, 18, 38, 38);
            }

            using (var bmpLogo = IconChar.Headset.ToBitmap(Color.White, 20))
            {
                g.DrawImage(bmpLogo, 25, 27);
            }

            using (var brushTitulo = new SolidBrush(Color.White))
            using (var fontTitulo = new Font("Segoe UI", 12.5f, FontStyle.Bold))
            {
                g.DrawString("HSis Support", fontTitulo, brushTitulo, new PointF(62, 17));
            }

            using (var brushSub = new SolidBrush(Color.FromArgb(148, 163, 184)))
            using (var fontSub = new Font("Segoe UI", 8.5f, FontStyle.Regular))
            {
                g.DrawString("Mesa de Servicio", fontSub, brushSub, new PointF(62, 37));
            }

            // Separador superior sutil
            using (var penDiv = new Pen(Color.FromArgb(30, 41, 59), 1f))
            {
                g.DrawLine(penDiv, 14, 68, Width - 14, 68);
            }

            // 2. Lista de Ítems de navegación
            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                var rect = ObtenerRectItem(i);
                bool esActivo = item.Clave == _itemActivoClave;
                bool esHover = _hoveredIndex == i;

                using var path = TemaVisual.CrearRectanguloRedondeado(rect, 8);

                if (esActivo)
                {
                    using var brushActivo = new SolidBrush(TemaVisual.SidebarItemActivo);
                    g.FillPath(brushActivo, path);
                }
                else if (esHover)
                {
                    using var brushHover = new SolidBrush(TemaVisual.SidebarItemHover);
                    g.FillPath(brushHover, path);
                }

                Color colorItem = esActivo ? Color.White : (esHover ? Color.White : Color.FromArgb(203, 213, 225));

                // Ícono FontAwesome vectorial nítido
                if (item.Icono != IconChar.None)
                {
                    using var bmpIcon = item.Icono.ToBitmap(colorItem, 20);
                    int iconY = rect.Y + (rect.Height - bmpIcon.Height) / 2;
                    g.DrawImage(bmpIcon, rect.X + 14, iconY);
                }

                // Título
                using (var brushTexto = new SolidBrush(colorItem))
                using (var fontTexto = new Font("Segoe UI", 9.5f, esActivo ? FontStyle.Bold : FontStyle.Regular))
                {
                    g.DrawString(item.Titulo, fontTexto, brushTexto, new PointF(rect.X + 44, rect.Y + 12));
                }

                // Badge de conteo si aplica
                if (item.BadgeCount > 0)
                {
                    var badgeRect = new Rectangle(rect.Right - 36, rect.Y + 10, 26, 22);
                    using var pathBadge = TemaVisual.CrearRectanguloRedondeado(badgeRect, 6);
                    using var brushBadge = new SolidBrush(esActivo ? Color.FromArgb(29, 78, 216) : Color.FromArgb(239, 68, 68));
                    using var brushBadgeTxt = new SolidBrush(Color.White);
                    using var sfBadge = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    using var fontBadge = new Font("Segoe UI", 8f, FontStyle.Bold);

                    g.FillPath(brushBadge, pathBadge);
                    g.DrawString(item.BadgeCount > 99 ? "99+" : item.BadgeCount.ToString(), fontBadge, brushBadgeTxt, badgeRect, sfBadge);
                }
            }

            // 3. Perfil de Usuario y Sesión en el pie
            int pieY = Height - 120;
            using (var penPie = new Pen(Color.FromArgb(30, 41, 59), 1f))
            {
                g.DrawLine(penPie, 14, pieY, Width - 14, pieY);
            }

            // Círculo de Avatar iniciales
            string nombre = SesionSistema.NombreUsuario;
            string iniciales = string.IsNullOrWhiteSpace(nombre) ? "U" : (nombre.Length >= 2 ? nombre[..2].ToUpperInvariant() : nombre.ToUpperInvariant());
            var rectAvatar = new Rectangle(16, pieY + 12, 34, 34);

            using (var brushAvatar = new SolidBrush(Color.FromArgb(51, 65, 85)))
            {
                g.FillEllipse(brushAvatar, rectAvatar);
            }

            using (var sfAv = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            using (var brushAvTxt = new SolidBrush(Color.White))
            using (var fontAv = new Font("Segoe UI", 9.5f, FontStyle.Bold))
            {
                g.DrawString(iniciales, fontAv, brushAvTxt, rectAvatar, sfAv);
            }

            // Nombre y Rol
            using (var brushNombre = new SolidBrush(Color.White))
            using (var fontNombre = new Font("Segoe UI", 9f, FontStyle.Bold))
            {
                string nombreCorto = nombre.Length > 15 ? string.Concat(nombre.AsSpan(0, 15), "...") : nombre;
                g.DrawString(nombreCorto, fontNombre, brushNombre, new PointF(56, pieY + 10));
            }

            string rolTexto = SesionSistema.EsAdmin ? "Administrador" : (SesionSistema.EsTecnico ? "Técnico" : "Cliente");
            using (var brushRol = new SolidBrush(Color.FromArgb(148, 163, 184)))
            using (var fontRol = new Font("Segoe UI", 8f, FontStyle.Regular))
            {
                g.DrawString(rolTexto, fontRol, brushRol, new PointF(56, pieY + 28));
            }

            // Botón "Cerrar Sesión" moderno con FontAwesome
            var rectSalir = ObtenerRectBotonSalir();
            bool esHoverSalir = _hoveredIndex == 999;
            using var pathSalir = TemaVisual.CrearRectanguloRedondeado(rectSalir, 6);
            using (var brushSalir = new SolidBrush(esHoverSalir ? Color.FromArgb(220, 38, 38) : Color.FromArgb(30, 41, 59)))
            {
                g.FillPath(brushSalir, pathSalir);
            }

            using (var bmpSalir = IconChar.SignOutAlt.ToBitmap(Color.White, 15))
            {
                g.DrawImage(bmpSalir, rectSalir.X + 16, rectSalir.Y + 11);
            }

            using (var brushSalirTxt = new SolidBrush(Color.White))
            using (var fontSalir = new Font("Segoe UI", 8.5f, FontStyle.Bold))
            {
                g.DrawString("Cerrar Sesión", fontSalir, brushSalirTxt, new PointF(rectSalir.X + 40, rectSalir.Y + 11));
            }
        }
    }
}
