#nullable enable
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using FontAwesome.Sharp;
using HSis.UI.Helpers;

namespace HSis.UI.Controls
{
    [SupportedOSPlatform("windows")]
    public class SelectorFechaModerno : UserControl
    {
        private readonly DateTimePicker _dtp;
        private int _radioBorde = 6;
        private Color _colorBorde = TemaVisual.BordeSutil;
        private Color _colorBordeFoco = TemaVisual.Primario;
        private bool _tieneFoco = false;

        [Category("Apariencia Moderna")]
        [DefaultValue(6)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int RadioBorde
        {
            get => _radioBorde;
            set { _radioBorde = Math.Max(0, value); Invalidate(); }
        }

        [Category("Comportamiento")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public DateTime Value
        {
            get => _dtp.Value;
            set
            {
                _dtp.Value = value;
                Invalidate();
            }
        }

        [Category("Comportamiento")]
        [DefaultValue(DateTimePickerFormat.Long)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public DateTimePickerFormat Format
        {
            get => _dtp.Format;
            set => _dtp.Format = value;
        }

        [Category("Comportamiento")]
        [DefaultValue(null)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string? CustomFormat
        {
            get => _dtp.CustomFormat;
            set => _dtp.CustomFormat = value;
        }

        public event EventHandler? ValueChanged;

        public SelectorFechaModerno()
        {
            _dtp = new DateTimePicker
            {
                Font = TemaVisual.FuenteNormal,
                CalendarFont = TemaVisual.FuenteNormal,
                CalendarForeColor = TemaVisual.TextoPrincipal,
                CalendarTitleBackColor = TemaVisual.Primario,
                CalendarTitleForeColor = Color.White
            };

            Padding = new Padding(24, 4, 4, 4);
            BackColor = Color.White;
            DoubleBuffered = true;
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            _dtp.Enter += (s, e) => { _tieneFoco = true; Invalidate(); };
            _dtp.Leave += (s, e) => { _tieneFoco = false; Invalidate(); };
            _dtp.ValueChanged += (s, e) => ValueChanged?.Invoke(this, e);

            Controls.Add(_dtp);
            AlinearDateTimePicker();
            Resize += (s, e) => AlinearDateTimePicker();
        }

        private void AlinearDateTimePicker()
        {
            _dtp.Location = new Point(Padding.Left, (Height - _dtp.PreferredHeight) / 2);
            _dtp.Width = Math.Max(20, Width - Padding.Left - Padding.Right);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using var path = TemaVisual.CrearRectanguloRedondeado(rect, _radioBorde);

            // Fondo
            using (var brushFondo = new SolidBrush(BackColor))
            {
                g.FillPath(brushFondo, path);
            }

            // Icono FontAwesome de calendario
            using (var bmpIcono = IconChar.CalendarDays.ToBitmap(TemaVisual.Primario, 15))
            {
                int yIcon = (Height - bmpIcono.Height) / 2;
                g.DrawImage(bmpIcono, 8, yIcon);
            }

            // Borde
            Color bordeActual = _tieneFoco ? _colorBordeFoco : _colorBorde;
            float grosor = _tieneFoco ? 1.6f : 1.2f;
            using (var penBorde = new Pen(bordeActual, grosor))
            {
                g.DrawPath(penBorde, path);
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            _dtp.Focus();
        }
    }
}
