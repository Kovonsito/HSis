#nullable enable
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using FontAwesome.Sharp;
using HSis.Contracts.Services;
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

        // Recursos GDI estáticos reutilizables para máximo rendimiento
        private static readonly Font FontTitulo = new("Segoe UI", 12f, FontStyle.Bold);
        private static readonly Font FontSub = new("Segoe UI", 8.5f, FontStyle.Regular);
        private static readonly Font FontItemActivo = new("Segoe UI", 9.5f, FontStyle.Bold);
        private static readonly Font FontItemNormal = new("Segoe UI", 9.5f, FontStyle.Regular);
        private static readonly Font FontBadge = new("Segoe UI", 7.5f, FontStyle.Bold);

        private static readonly Bitmap BmpLogo = IconChar.Headset.ToBitmap(Color.White, 20);
        private static readonly SolidBrush BrushLogoBg = new(Color.FromArgb(37, 99, 235));
        private static readonly SolidBrush BrushTextoBlanco = new(Color.White);
        private static readonly SolidBrush BrushTextoSub = new(Color.FromArgb(148, 163, 184));
        private static readonly SolidBrush BrushTextoItemInactivo = new(Color.FromArgb(203, 213, 225));
        private static readonly SolidBrush BrushItemActivo = new(TemaVisual.SidebarItemActivo);
        private static readonly SolidBrush BrushItemHover = new(TemaVisual.SidebarItemHover);
        private static readonly Pen PenSeparador = new(Color.FromArgb(30, 41, 59), 1f);

        private static readonly Dictionary<(IconChar Icono, Color Color), Bitmap> CacheIconosSidebar = [];
        private static readonly object LockIconos = new();

        private static Bitmap ObtenerIconoSidebar(IconChar icono, Color color)
        {
            lock (LockIconos)
            {
                var clave = (icono, color);
                if (!CacheIconosSidebar.TryGetValue(clave, out var bmp))
                {
                    bmp = icono.ToBitmap(color, 20);
                    CacheIconosSidebar[clave] = bmp;
                }
                return bmp;
            }
        }

        private readonly List<ItemSidebar> _items = [];
        private string _itemActivoClave = string.Empty;
        private int _hoveredIndex = -1;
        private bool _colapsado = false;
        private IAlmacenamientoCredencialesLocal? _sessionCache;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Colapsado
        {
            get => _colapsado;
            set
            {
                _colapsado = value;
                Visible = !_colapsado;
                Invalidate();
            }
        }

        public SidebarControl()
        {
            Dock = DockStyle.Left;
            Width = 240;
            BackColor = TemaVisual.SidebarFondo;
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);

            MouseMove += SidebarControl_MouseMove;
            MouseLeave += (s, e) =>
            {
                _hoveredIndex = -1;
                Cursor = Cursors.Default;
                Invalidate();
            };
            MouseClick += SidebarControl_MouseClick;
        }

        public void ConfigurarSesion(IAlmacenamientoCredencialesLocal sessionCache)
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

            if (_hoveredIndex != nuevoHover)
            {
                _hoveredIndex = nuevoHover;
                Cursor = (nuevoHover >= 0) ? Cursors.Hand : Cursors.Default;
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
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // 1. Logotipo y Título de cabecera
            g.FillEllipse(BrushLogoBg, 16, 18, 38, 38);
            g.DrawImage(BmpLogo, 25, 27);
            g.DrawString("HSis Support", FontTitulo, BrushTextoBlanco, new PointF(60, 17));
            g.DrawString("Mesa de Servicio", FontSub, BrushTextoSub, new PointF(60, 37));

            // Separador superior sutil
            g.DrawLine(PenSeparador, 14, 68, Width - 14, 68);

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
                    g.FillPath(BrushItemActivo, path);
                }
                else if (esHover)
                {
                    g.FillPath(BrushItemHover, path);
                }

                Color colorItem = esActivo ? Color.White : (esHover ? Color.White : Color.FromArgb(203, 213, 225));
                var brushTexto = (esActivo || esHover) ? BrushTextoBlanco : BrushTextoItemInactivo;

                // Ícono FontAwesome vectorial en caché
                if (item.Icono != IconChar.None)
                {
                    var bmpIcon = ObtenerIconoSidebar(item.Icono, colorItem);
                    int iconY = rect.Y + (rect.Height - bmpIcon.Height) / 2;
                    g.DrawImage(bmpIcon, rect.X + 14, iconY);
                }

                // Título
                var fontTexto = esActivo ? FontItemActivo : FontItemNormal;
                g.DrawString(item.Titulo, fontTexto, brushTexto, new PointF(rect.X + 44, rect.Y + 12));

                // Badge de conteo si aplica
                if (item.BadgeCount > 0)
                {
                    var badgeRect = new Rectangle(rect.Right - 36, rect.Y + 10, 26, 22);
                    using var pathBadge = TemaVisual.CrearRectanguloRedondeado(badgeRect, 6);
                    using var brushBadge = new SolidBrush(esActivo ? Color.FromArgb(29, 78, 216) : Color.FromArgb(239, 68, 68));

                    using var sfBadge = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.FillPath(brushBadge, pathBadge);
                    g.DrawString(item.BadgeCount > 99 ? "99+" : item.BadgeCount.ToString(), FontBadge, BrushTextoBlanco, badgeRect, sfBadge);
                }
            }
        }
    }
}
