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
        private Color _colorBordeError = TemaVisual.Peligro;
        private bool _tieneError = false;
        private string _placeholder = string.Empty;
        private bool _esFormatoTelefono = false;
        private bool _aplicandoFormato = false;

        [Category("Apariencia Moderna")]
        [DefaultValue(6)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int RadioBorde
        {
            get => _radioBorde;
            set { _radioBorde = Math.Max(0, value); Invalidate(); }
        }

        [Category("Apariencia Moderna")]
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool TieneError
        {
            get => _tieneError;
            set
            {
                if (_tieneError != value)
                {
                    _tieneError = value;
                    Invalidate();
                }
            }
        }

        [Category("Apariencia Moderna")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ColorBordeError
        {
            get => _colorBordeError;
            set
            {
                if (_colorBordeError != value)
                {
                    _colorBordeError = value;
                    Invalidate();
                }
            }
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

        [Category("Comportamiento")]
        [DefaultValue(32767)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int MaxLength
        {
            get => _textBox.MaxLength;
            set => _textBox.MaxLength = value;
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

        [Category("Comportamiento")]
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool EsFormatoTelefono
        {
            get => _esFormatoTelefono;
            set
            {
                _esFormatoTelefono = value;
                if (value)
                {
                    MaxLength = 14;
                    if (!string.IsNullOrEmpty(_textBox.Text))
                    {
                        FormatearTextoTelefono();
                    }
                }
            }
        }

        [Browsable(false)]
        public bool EsTelefonoValido
        {
            get
            {
                if (!_esFormatoTelefono)
                {
                    return true;
                }

                var digitos = _textBox.Text.Count(char.IsDigit);
                return digitos == 0 || digitos == 10;
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
                if (_aplicandoFormato)
                {
                    return;
                }

                if (_tieneError)
                {
                    _tieneError = false;
                }

                if (_esFormatoTelefono)
                {
                    FormatearTextoTelefono();
                }

                OnTextChanged(e);
                Invalidate();
            };

            Controls.Add(_textBox);
            AlinearTextBoxVertical();
            Resize += (s, e) => AlinearTextBoxVertical();
        }

        private void FormatearTextoTelefono()
        {
            _aplicandoFormato = true;
            try
            {
                string raw = _textBox.Text;
                int rawSelStart = _textBox.SelectionStart;

                // Contar cuántos dígitos hay antes de la posición actual del cursor
                int digitsBeforeCursor = 0;
                for (int i = 0; i < Math.Min(rawSelStart, raw.Length); i++)
                {
                    if (char.IsDigit(raw[i]))
                    {
                        digitsBeforeCursor++;
                    }
                }

                // Extraer solo dígitos (máximo 10)
                var digits = new string(raw.Where(char.IsDigit).Take(10).ToArray());

                string formatted;
                if (digits.Length == 0)
                {
                    formatted = string.Empty;
                }
                else if (digits.Length <= 3)
                {
                    formatted = $"({digits}";
                }
                else if (digits.Length <= 6)
                {
                    formatted = $"({digits[..3]}) {digits[3..]}";
                }
                else
                {
                    formatted = $"({digits[..3]}) {digits[3..6]}-{digits[6..]}";
                }

                _textBox.Text = formatted;

                // Reposicionar el cursor después de los mismos dígitos
                if (digitsBeforeCursor == 0)
                {
                    _textBox.SelectionStart = formatted.Length > 0 && formatted.StartsWith("(") ? 1 : 0;
                }
                else
                {
                    int currentDigits = 0;
                    int newPos = formatted.Length;
                    for (int i = 0; i < formatted.Length; i++)
                    {
                        if (char.IsDigit(formatted[i]))
                        {
                            currentDigits++;
                            if (currentDigits == digitsBeforeCursor)
                            {
                                newPos = i + 1;
                                break;
                            }
                        }
                    }
                    _textBox.SelectionStart = Math.Min(newPos, formatted.Length);
                }
            }
            finally
            {
                _aplicandoFormato = false;
            }
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

            // Borde: Si tiene error, se dibuja rojo; si tiene foco, color foco; de lo contrario color borde sutil
            Color bordeActual = _tieneError
                ? _colorBordeError
                : (_tieneFoco ? _colorBordeFoco : _colorBorde);
            float grosor = (_tieneError || _tieneFoco) ? 1.6f : 1.2f;
            using var penBorde = new Pen(bordeActual, grosor);
            g.DrawPath(penBorde, path);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            _textBox.Focus();
        }
    }
}
