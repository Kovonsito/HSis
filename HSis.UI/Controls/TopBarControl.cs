using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using FontAwesome.Sharp;
using HSis.UI.Helpers;

namespace HSis.UI.Controls
{
    [SupportedOSPlatform("windows")]
    public class TopBarControl : UserControl
    {
        public event EventHandler? NotificacionesClic;

        private string _titulo = "Panel Principal";
        private string _subtitulo = "Mesa de Servicio";
        private bool _conectado = true;
        private string _textoConexion = "En Línea";
        private int _notificacionesNoLeidas = 0;
        private bool _hoverCampana = false;

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
            Height = 64;
            BackColor = Color.White;
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            MouseMove += TopBarControl_MouseMove;
            MouseLeave += (s, e) => { _hoverCampana = false; Invalidate(); };
            MouseClick += TopBarControl_MouseClick;
        }

        public void ActualizarConexion(bool conectado, string? mensaje = null)
        {
            _conectado = conectado;
            _textoConexion = conectado ? "En Línea" : (!string.IsNullOrEmpty(mensaje) ? mensaje : "Reconectando...");
            Invalidate();
        }

        private Rectangle ObtenerRectCampana()
        {
            return new Rectangle(Width - 55, 14, 38, 38);
        }

        private void TopBarControl_MouseMove(object? sender, MouseEventArgs e)
        {
            bool hover = ObtenerRectCampana().Contains(e.Location);
            if (_hoverCampana != hover)
            {
                _hoverCampana = hover;
                Cursor = hover ? Cursors.Hand : Cursors.Default;
                Invalidate();
            }
        }

        private void TopBarControl_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && ObtenerRectCampana().Contains(e.Location))
            {
                NotificacionesClic?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // 1. Título y Subtítulo / Breadcrumb
            using (var brushTitulo = new SolidBrush(TemaVisual.TextoPrincipal))
            using (var fontTitulo = new Font("Segoe UI", 13.5f, FontStyle.Bold))
            {
                g.DrawString(_titulo, fontTitulo, brushTitulo, new PointF(18, 11));
            }

            using (var brushSub = new SolidBrush(TemaVisual.TextoSecundario))
            using (var fontSub = new Font("Segoe UI", 8.5f, FontStyle.Regular))
            {
                g.DrawString(_subtitulo, fontSub, brushSub, new PointF(18, 36));
            }

            // 2. Indicador de Estado SignalR en tiempo real (Pill moderno)
            int pillWidth = 118;
            int pillX = Width - 60 - pillWidth - 10;
            var rectPill = new Rectangle(pillX, 18, pillWidth, 28);
            using var pathPill = TemaVisual.CrearRectanguloRedondeado(rectPill, 14);
            using (var brushPill = new SolidBrush(_conectado ? Color.FromArgb(236, 253, 245) : Color.FromArgb(254, 243, 199)))
            {
                g.FillPath(brushPill, pathPill);
            }

            using (var penPill = new Pen(_conectado ? Color.FromArgb(167, 243, 208) : Color.FromArgb(253, 230, 138), 1f))
            {
                g.DrawPath(penPill, pathPill);
            }

            // Punto verde/amarillo vectorial
            using (var brushDot = new SolidBrush(_conectado ? Color.FromArgb(16, 185, 129) : Color.FromArgb(245, 158, 11)))
            {
                g.FillEllipse(brushDot, rectPill.X + 12, rectPill.Y + (rectPill.Height - 8) / 2, 8, 8);
            }

            // Texto de estado
            using (var brushPillTxt = new SolidBrush(_conectado ? Color.FromArgb(6, 95, 70) : Color.FromArgb(146, 64, 14)))
            using (var fontPill = new Font("Segoe UI Semibold", 8.5f, FontStyle.Bold))
            {
                var rectTxt = new Rectangle(rectPill.X + 24, rectPill.Y, rectPill.Width - 26, rectPill.Height);
                using var sfPill = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(_textoConexion, fontPill, brushPillTxt, rectTxt, sfPill);
            }

            // 3. Botón Campana Notificaciones
            var rectCampana = ObtenerRectCampana();
            using var pathCampana = TemaVisual.CrearRectanguloRedondeado(rectCampana, 8);
            if (_hoverCampana)
            {
                using var brushH = new SolidBrush(Color.FromArgb(241, 245, 249));
                g.FillPath(brushH, pathCampana);
            }

            using (var bmpCampana = FontAwesome.Sharp.IconChar.Bell.ToBitmap(TemaVisual.TextoPrincipal, 18))
            {
                int campX = rectCampana.X + (rectCampana.Width - bmpCampana.Width) / 2;
                int campY = rectCampana.Y + (rectCampana.Height - bmpCampana.Height) / 2;
                g.DrawImage(bmpCampana, campX, campY);
            }

            // Badge rojo de notificaciones no leídas
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

            // Línea divisoria inferior sutil
            using var penBorde = new Pen(TemaVisual.BordeSutil, 1f);
            g.DrawLine(penBorde, 0, Height - 1, Width, Height - 1);
        }
    }
}
