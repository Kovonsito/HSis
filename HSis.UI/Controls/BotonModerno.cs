#nullable enable
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using FontAwesome.Sharp;
using HSis.UI.Helpers;

namespace HSis.UI.Controls
{
    public enum EstiloBotonModerno
    {
        Primario,
        Secundario,
        Exito,
        Peligro,
        Advertencia,
        Ghost
    }

    [SupportedOSPlatform("windows")]
    public class BotonModerno : Button
    {
        private EstiloBotonModerno _estilo = EstiloBotonModerno.Primario;
        private int _radioBorde = 8;
        private IconChar _icono = IconChar.None;
        private int _iconoTamano = 16;
        private bool _isHovered = false;
        private bool _isPressed = false;

        [Category("Apariencia Moderna")]
        [DefaultValue(EstiloBotonModerno.Primario)]
        public EstiloBotonModerno Estilo
        {
            get => _estilo;
            set
            {
                _estilo = value;
                ActualizarColoresPorDefecto();
                Invalidate();
            }
        }

        [Category("Apariencia Moderna")]
        [DefaultValue(8)]
        public int RadioBorde
        {
            get => _radioBorde;
            set
            {
                _radioBorde = Math.Max(0, value);
                ActualizarRegion();
                Invalidate();
            }
        }

        [Category("Apariencia Moderna")]
        [DefaultValue(IconChar.None)]
        public IconChar Icono
        {
            get => _icono;
            set
            {
                _icono = value;
                Invalidate();
            }
        }

        [Category("Apariencia Moderna")]
        [DefaultValue(16)]
        public int IconoTamano
        {
            get => _iconoTamano;
            set
            {
                _iconoTamano = Math.Max(8, value);
                Invalidate();
            }
        }

        public BotonModerno()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Cursor = Cursors.Hand;
            Font = new Font("Segoe UI Semibold", 9.5f, FontStyle.Bold);
            Size = new Size(130, 38);
            BackColor = Color.Transparent;
            DoubleBuffered = true;
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            ActualizarColoresPorDefecto();
        }

        private void ActualizarColoresPorDefecto()
        {
            switch (_estilo)
            {
                case EstiloBotonModerno.Primario:
                    ForeColor = Color.White;
                    break;
                case EstiloBotonModerno.Secundario:
                    ForeColor = TemaVisual.TextoPrincipal;
                    break;
                case EstiloBotonModerno.Exito:
                    ForeColor = Color.White;
                    break;
                case EstiloBotonModerno.Peligro:
                    ForeColor = Color.White;
                    break;
                case EstiloBotonModerno.Advertencia:
                    ForeColor = Color.White;
                    break;
                case EstiloBotonModerno.Ghost:
                    ForeColor = TemaVisual.TextoSecundario;
                    break;
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _isHovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _isHovered = false;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            _isPressed = true;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            _isPressed = false;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // 1. Limpiar esquinas con el color del contenedor padre para evitar fondo negro
            Color colorPadre = Parent?.BackColor ?? (BackColor != Color.Transparent ? BackColor : Color.White);
            using (var brushPadre = new SolidBrush(colorPadre))
            {
                g.FillRectangle(brushPadre, 0, 0, Width, Height);
            }

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using var path = TemaVisual.CrearRectanguloRedondeado(rect, _radioBorde);

            // Obtener colores según estado y estilo
            Color colorFondo;
            Color colorBorde = Color.Transparent;
            Color colorTexto = ForeColor;

            if (!Enabled)
            {
                colorFondo = Color.FromArgb(241, 245, 249);
                colorTexto = Color.FromArgb(148, 163, 184);
                colorBorde = Color.FromArgb(226, 232, 240);
            }
            else
            {
                switch (_estilo)
                {
                    case EstiloBotonModerno.Primario:
                        colorFondo = _isPressed ? Color.FromArgb(29, 78, 216) : (_isHovered ? Color.FromArgb(37, 99, 235) : Color.FromArgb(59, 130, 246));
                        break;

                    case EstiloBotonModerno.Secundario:
                        colorFondo = _isPressed ? Color.FromArgb(241, 245, 249) : (_isHovered ? Color.FromArgb(248, 250, 252) : Color.White);
                        colorBorde = _isHovered ? TemaVisual.BordeHover : TemaVisual.BordeSutil;
                        break;

                    case EstiloBotonModerno.Exito:
                        colorFondo = _isPressed ? Color.FromArgb(5, 150, 105) : (_isHovered ? Color.FromArgb(16, 185, 129) : Color.FromArgb(34, 197, 94));
                        break;

                    case EstiloBotonModerno.Peligro:
                        colorFondo = _isPressed ? Color.FromArgb(185, 28, 28) : (_isHovered ? Color.FromArgb(220, 38, 38) : Color.FromArgb(239, 68, 68));
                        break;

                    case EstiloBotonModerno.Advertencia:
                        colorFondo = _isPressed ? Color.FromArgb(217, 119, 6) : (_isHovered ? Color.FromArgb(245, 158, 11) : Color.FromArgb(251, 191, 36));
                        break;

                    case EstiloBotonModerno.Ghost:
                        colorFondo = _isPressed ? Color.FromArgb(226, 232, 240) : (_isHovered ? Color.FromArgb(241, 245, 249) : Color.Transparent);
                        break;

                    default:
                        colorFondo = Color.FromArgb(59, 130, 246);
                        break;
                }
            }

            // Dibujar fondo
            using (var brushFondo = new SolidBrush(colorFondo))
            {
                g.FillPath(brushFondo, path);
            }

            // Dibujar borde si corresponde
            if (colorBorde != Color.Transparent)
            {
                using var penBorde = new Pen(colorBorde, 1.2f);
                g.DrawPath(penBorde, path);
            }

            // Dibujar icono FontAwesome o imagen si existe
            int textX = 0;
            if (_icono != IconChar.None)
            {
                using var bmpIcono = _icono.ToBitmap(colorTexto, _iconoTamano);
                if (string.IsNullOrWhiteSpace(Text))
                {
                    int imgX = (Width - bmpIcono.Width) / 2;
                    int imgY = (Height - bmpIcono.Height) / 2;
                    g.DrawImage(bmpIcono, imgX, imgY);
                }
                else
                {
                    int imgX = 14;
                    int imgY = (Height - bmpIcono.Height) / 2;
                    g.DrawImage(bmpIcono, imgX, imgY);
                    textX = imgX + bmpIcono.Width + 8;
                }
            }
            else if (Image != null)
            {
                int imgY = (Height - Image.Height) / 2;
                int imgX = 12;
                g.DrawImage(Image, imgX, imgY, Image.Width, Image.Height);
                textX = imgX + Image.Width + 6;
            }

            // Dibujar texto
            if (!string.IsNullOrWhiteSpace(Text))
            {
                using var brushTexto = new SolidBrush(colorTexto);
                using var sf = new StringFormat
                {
                    Alignment = textX > 0 ? StringAlignment.Near : StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                var textRect = textX > 0
                    ? new Rectangle(textX, 0, Width - textX - 8, Height)
                    : new Rectangle(0, 0, Width, Height);

                g.DrawString(Text, Font, brushTexto, textRect, sf);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ActualizarRegion();
        }

        private void ActualizarRegion()
        {
            if (Width > 0 && Height > 0)
            {
                var rect = new Rectangle(0, 0, Width, Height);
                using var path = TemaVisual.CrearRectanguloRedondeado(rect, _radioBorde);
                Region = new Region(path);
            }
        }
    }
}
