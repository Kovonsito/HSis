#nullable enable
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using HSis.UI.Helpers;

namespace HSis.UI.Controls
{
    [SupportedOSPlatform("windows")]
    public class TarjetaContenedor : Panel
    {
        private int _radioBorde = 10;
        private Color _colorBorde = TemaVisual.BordeSutil;
        private Color _colorFondo = TemaVisual.FondoTarjeta;
        private string _titulo = string.Empty;
        private string _subtitulo = string.Empty;
        private int _alturaCabecera = 0;

        [Category("Apariencia Moderna")]
        [DefaultValue(10)]
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
                _alturaCabecera = string.IsNullOrWhiteSpace(_titulo) ? 0 : 42;
                Invalidate();
            }
        }

        [Category("Apariencia Moderna")]
        [DefaultValue("")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Subtitulo
        {
            get => _subtitulo;
            set { _subtitulo = value; Invalidate(); }
        }

        public TarjetaContenedor()
        {
            BackColor = Color.Transparent;
            Padding = new Padding(12);
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using var path = TemaVisual.CrearRectanguloRedondeado(rect, _radioBorde);

            // Fondo de la tarjeta
            using (var brushFondo = new SolidBrush(_colorFondo))
            {
                g.FillPath(brushFondo, path);
            }

            // Cabecera si tiene título
            if (!string.IsNullOrWhiteSpace(_titulo))
            {
                var rectCabecera = new Rectangle(0, 0, Width - 1, _alturaCabecera);
                using (var brushTexto = new SolidBrush(TemaVisual.TextoPrincipal))
                {
                    g.DrawString(_titulo, TemaVisual.FuenteSubtitulo, brushTexto, new PointF(14, 12));
                }

                if (!string.IsNullOrWhiteSpace(_subtitulo))
                {
                    using var brushSub = new SolidBrush(TemaVisual.TextoSecundario);
                    g.DrawString(_subtitulo, TemaVisual.FuentePequena, brushSub, new PointF(14, 28));
                }

                // Línea divisoria
                using var penDiv = new Pen(TemaVisual.BordeSutil, 1f);
                g.DrawLine(penDiv, 14, _alturaCabecera, Width - 15, _alturaCabecera);
            }

            // Borde redondeado sutil
            using (var penBorde = new Pen(_colorBorde, 1.2f))
            {
                g.DrawPath(penBorde, path);
            }
        }
    }
}
