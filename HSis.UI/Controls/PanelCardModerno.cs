#nullable enable
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using FontAwesome.Sharp;
using HSis.UI.Helpers;

namespace HSis.UI.Controls
{
    [SupportedOSPlatform("windows")]
    public class PanelCardModerno : Panel
    {
        private int _radioBorde = 8;
        private Color _colorBorde = TemaVisual.BordeSutil;
        private string _titulo = string.Empty;
        private IconChar _icono = IconChar.None;
        private int _iconoTamano = 16;
        private Color _colorIcono = TemaVisual.Primario;

        [Category("Apariencia Moderna")]
        [DefaultValue(8)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int RadioBorde
        {
            get => _radioBorde;
            set { _radioBorde = Math.Max(0, value); Invalidate(); }
        }

        [Category("Apariencia Moderna")]
        [DefaultValue("")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Titulo
        {
            get => _titulo;
            set
            {
                _titulo = value;
                ActualizarPaddingInterno();
                Invalidate();
            }
        }

        [Category("Apariencia Moderna")]
        [DefaultValue(IconChar.None)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public IconChar Icono
        {
            get => _icono;
            set { _icono = value; Invalidate(); }
        }

        [Category("Apariencia Moderna")]
        [DefaultValue(16)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int IconoTamano
        {
            get => _iconoTamano;
            set { _iconoTamano = Math.Max(8, value); Invalidate(); }
        }

        public PanelCardModerno()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);

            DoubleBuffered = true;
            BackColor = TemaVisual.FondoTarjeta;
            ActualizarPaddingInterno();
        }

        private void ActualizarPaddingInterno()
        {
            int top = string.IsNullOrWhiteSpace(_titulo) ? 14 : 44;
            Padding = new Padding(14, top, 14, 14);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using var path = TemaVisual.CrearRectanguloRedondeado(rect, _radioBorde);

            // Fondo de la tarjeta
            using (var brushFondo = new SolidBrush(BackColor))
            {
                g.FillPath(brushFondo, path);
            }

            // Borde sutil
            using (var penBorde = new Pen(_colorBorde, 1f))
            {
                g.DrawPath(penBorde, path);
            }

            // Cabecera con título e icono opcionales
            if (!string.IsNullOrWhiteSpace(_titulo))
            {
                int startX = 14;
                if (_icono != IconChar.None)
                {
                    using var bmp = _icono.ToBitmap(_colorIcono, _iconoTamano);
                    int iconY = (38 - bmp.Height) / 2;
                    g.DrawImage(bmp, startX, iconY);
                    startX += bmp.Width + 8;
                }

                using var brushTitle = new SolidBrush(TemaVisual.TextoPrincipal);
                using var fontTitle = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                using var sf = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Near
                };
                g.DrawString(_titulo, fontTitle, brushTitle, new RectangleF(startX, 0, Width - startX - 14, 38), sf);

                // Línea divisoria suave bajo el encabezado
                using var penDiv = new Pen(_colorBorde, 1f);
                g.DrawLine(penDiv, 1, 38, Width - 2, 38);
            }
        }
    }
}
