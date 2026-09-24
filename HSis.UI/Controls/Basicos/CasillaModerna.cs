#nullable enable
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using FontAwesome.Sharp;
using HSis.UI.Helpers;

namespace HSis.UI.Controls
{
    [SupportedOSPlatform("windows")]
    [DefaultEvent("CheckedChanged")]
    public class CasillaModerna : Control
    {
        private bool _checked = false;
        private bool _isHovered = false;
        private int _tamanoCasilla = 18;
        private int _radioBorde = 4;

        [Category("Comportamiento")]
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Checked
        {
            get => _checked;
            set
            {
                if (_checked != value)
                {
                    _checked = value;
                    CheckedChanged?.Invoke(this, EventArgs.Empty);
                    Invalidate();
                }
            }
        }

        [Category("Apariencia Moderna")]
        [DefaultValue(18)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int TamanoCasilla
        {
            get => _tamanoCasilla;
            set { _tamanoCasilla = Math.Max(12, value); Invalidate(); }
        }

        [Category("Apariencia Moderna")]
        [DefaultValue(4)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int RadioBorde
        {
            get => _radioBorde;
            set { _radioBorde = Math.Max(0, value); Invalidate(); }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [System.Diagnostics.CodeAnalysis.AllowNull]
        public override string Text
        {
            get => base.Text;
            set
            {
                base.Text = value ?? string.Empty;
                Invalidate();
            }
        }

        public event EventHandler? CheckedChanged;

        public CasillaModerna()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.Selectable |
                     ControlStyles.ResizeRedraw, true);

            DoubleBuffered = true;
            Cursor = Cursors.Hand;
            Font = TemaVisual.FuenteNormal;
            ForeColor = TemaVisual.TextoPrincipal;
            BackColor = Color.Transparent;
            Size = new Size(200, 24);
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

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            Focus();
            Checked = !Checked;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Space)
            {
                Checked = !Checked;
                e.Handled = true;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int boxY = Math.Max(0, (Height - _tamanoCasilla) / 2);
            var boxRect = new Rectangle(1, boxY, _tamanoCasilla, _tamanoCasilla);

            using var path = TemaVisual.CrearRectanguloRedondeado(boxRect, _radioBorde);

            if (_checked)
            {
                // Caja marcada: Fondo primario con checkmark blanco
                using var brushFondo = new SolidBrush(TemaVisual.Primario);
                g.FillPath(brushFondo, path);

                using var penBorde = new Pen(TemaVisual.PrimarioHover, 1f);
                g.DrawPath(penBorde, path);

                // Dibujar Checkmark FontAwesome
                using var bmpCheck = IconChar.Check.ToBitmap(Color.White, _tamanoCasilla - 4);
                int checkX = boxRect.X + (boxRect.Width - bmpCheck.Width) / 2;
                int checkY = boxRect.Y + (boxRect.Height - bmpCheck.Height) / 2;
                g.DrawImage(bmpCheck, checkX, checkY);
            }
            else
            {
                // Caja desmarcada: Fondo blanco o transparente con borde sutil / hover
                using var brushFondo = new SolidBrush(Color.White);
                g.FillPath(brushFondo, path);

                Color colorBorde = _isHovered ? TemaVisual.Primario : TemaVisual.BordeSutil;
                float grosorBorde = _isHovered ? 1.5f : 1f;
                using var penBorde = new Pen(colorBorde, grosorBorde);
                g.DrawPath(penBorde, path);
            }

            // Texto de la casilla
            if (!string.IsNullOrEmpty(Text))
            {
                int textX = boxRect.Right + 8;
                var textRect = new Rectangle(textX, 0, Math.Max(0, Width - textX), Height);
                Color colorTexto = Enabled ? ForeColor : TemaVisual.TextoMuted;
                using var brushTexto = new SolidBrush(colorTexto);
                using var sf = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                g.DrawString(Text, Font, brushTexto, textRect, sf);
            }
        }
    }
}
