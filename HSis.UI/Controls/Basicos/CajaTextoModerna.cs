#nullable enable
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using HSis.UI.Helpers;

namespace HSis.UI.Controls
{
    [SupportedOSPlatform("windows")]
    public class CajaTextoModerna : UserControl
    {
        private readonly TextBox _textBox;
        private int _radioBorde = 6;
        private Color _colorBorde = TemaVisual.BordeSutil;
        private Color _colorBordeFoco = TemaVisual.Primario;
        private bool _tieneFoco = false;
        private string _placeholder = string.Empty;

        [Category("Apariencia Moderna")]
        [DefaultValue(6)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int RadioBorde
        {
            get => _radioBorde;
            set { _radioBorde = Math.Max(0, value); Invalidate(); }
        }

        [Category("Apariencia Moderna")]
        [DefaultValue("")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Placeholder
        {
            get => _placeholder;
            set { _placeholder = value; Invalidate(); }
        }

        [Category("Comportamiento")]
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ReadOnly
        {
            get => _textBox.ReadOnly;
            set
            {
                _textBox.ReadOnly = value;
                _textBox.BackColor = value ? Color.FromArgb(248, 250, 252) : Color.White;
                BackColor = _textBox.BackColor;
                Invalidate();
            }
        }

        [Category("Comportamiento")]
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Multiline
        {
            get => _textBox.Multiline;
            set
            {
                _textBox.Multiline = value;
                if (value)
                {
                    _textBox.Dock = DockStyle.Fill;
                }
                else
                {
                    _textBox.Dock = DockStyle.None;
                    AlinearTextBoxVertical();
                }
            }
        }

        [Category("Comportamiento")]
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool UseSystemPasswordChar
        {
            get => _textBox.UseSystemPasswordChar;
            set => _textBox.UseSystemPasswordChar = value;
        }

        [Category("Comportamiento")]
        [DefaultValue('\0')]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public char PasswordChar
        {
            get => _textBox.PasswordChar;
            set => _textBox.PasswordChar = value;
        }

        public void Clear()
        {
            _textBox.Clear();
            Invalidate();
        }

        public new bool Focus()
        {
            return _textBox.Focus();
        }

        public void SelectAll()
        {
            _textBox.SelectAll();
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [System.Diagnostics.CodeAnalysis.AllowNull]
        public override string Text
        {
            get => _textBox.Text;
            set
            {
                _textBox.Text = value ?? string.Empty;
                Invalidate();
            }
        }

        public CajaTextoModerna()
        {
            _textBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Font = TemaVisual.FuenteNormal,
                ForeColor = TemaVisual.TextoPrincipal,
                BackColor = Color.White
            };

            Padding = new Padding(10, 8, 10, 8);
            BackColor = Color.White;
            DoubleBuffered = true;
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            _textBox.Enter += (s, e) => { _tieneFoco = true; Invalidate(); };
            _textBox.Leave += (s, e) => { _tieneFoco = false; Invalidate(); };
            _textBox.TextChanged += (s, e) =>
            {
                OnTextChanged(e);
                Invalidate();
            };

            Controls.Add(_textBox);
            AlinearTextBoxVertical();
            Resize += (s, e) => AlinearTextBoxVertical();
        }

        private void AlinearTextBoxVertical()
        {
            if (!_textBox.Multiline)
            {
                _textBox.Location = new Point(Padding.Left, (Height - _textBox.PreferredHeight) / 2);
                _textBox.Width = Math.Max(20, Width - Padding.Left - Padding.Right);
            }
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

            // Placeholder
            if (string.IsNullOrEmpty(_textBox.Text) && !string.IsNullOrEmpty(_placeholder) && !_tieneFoco)
            {
                using var brushPlaceholder = new SolidBrush(TemaVisual.TextoMuted);
                int yPos = _textBox.Multiline ? Padding.Top : (Height - (int)g.MeasureString(_placeholder, _textBox.Font).Height) / 2;
                g.DrawString(_placeholder, _textBox.Font, brushPlaceholder, new PointF(Padding.Left, yPos));
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
            _textBox.Focus();
        }
    }
}
