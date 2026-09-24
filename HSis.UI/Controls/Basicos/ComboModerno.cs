#nullable enable
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using FontAwesome.Sharp;
using HSis.UI.Helpers;

namespace HSis.UI.Controls
{
    [SupportedOSPlatform("windows")]
    [DefaultEvent("SelectedIndexChanged")]
    public class ComboModerno : UserControl
    {
        private readonly ComboBox _comboBox;
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

        [Category("Datos")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ComboBox.ObjectCollection Items => _comboBox.Items;

        [Category("Datos")]
        [DefaultValue(null)]
        [AttributeProvider(typeof(IListSource))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public object? DataSource
        {
            get => _comboBox.DataSource;
            set => _comboBox.DataSource = value;
        }

        [Category("Datos")]
        [DefaultValue("")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string DisplayMember
        {
            get => _comboBox.DisplayMember;
            set => _comboBox.DisplayMember = value;
        }

        [Category("Datos")]
        [DefaultValue("")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ValueMember
        {
            get => _comboBox.ValueMember;
            set => _comboBox.ValueMember = value;
        }

        [Category("Comportamiento")]
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool FormattingEnabled
        {
            get => _comboBox.FormattingEnabled;
            set => _comboBox.FormattingEnabled = value;
        }

        [Category("Comportamiento")]
        [DefaultValue(ComboBoxStyle.DropDownList)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ComboBoxStyle DropDownStyle
        {
            get => _comboBox.DropDownStyle;
            set => _comboBox.DropDownStyle = value;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedIndex
        {
            get => _comboBox.SelectedIndex;
            set => _comboBox.SelectedIndex = value;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object? SelectedItem
        {
            get => _comboBox.SelectedItem;
            set => _comboBox.SelectedItem = value;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object? SelectedValue
        {
            get => _comboBox.SelectedValue;
            set => _comboBox.SelectedValue = value!;
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [System.Diagnostics.CodeAnalysis.AllowNull]
        public override string Text
        {
            get => _comboBox.Text;
            set => _comboBox.Text = value ?? string.Empty;
        }

        public event EventHandler? SelectedIndexChanged;

        public ComboModerno()
        {
            _comboBox = new ComboBox
            {
                FlatStyle = FlatStyle.Flat,
                Font = TemaVisual.FuenteNormal,
                ForeColor = TemaVisual.TextoPrincipal,
                BackColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            Padding = new Padding(10, 6, 26, 6);
            BackColor = Color.White;
            DoubleBuffered = true;
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            _comboBox.Enter += (s, e) => { _tieneFoco = true; Invalidate(); };
            _comboBox.Leave += (s, e) => { _tieneFoco = false; Invalidate(); };
            _comboBox.SelectedIndexChanged += (s, e) => SelectedIndexChanged?.Invoke(this, e);

            Controls.Add(_comboBox);
            AlinearComboVertical();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AlinearComboVertical();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            _comboBox.Font = Font;
            AlinearComboVertical();
        }

        private void AlinearComboVertical()
        {
            if (_comboBox == null) return;
            _comboBox.Left = Padding.Left;
            _comboBox.Width = Math.Max(10, Width - Padding.Left - Padding.Right);
            _comboBox.Top = Math.Max(0, (Height - _comboBox.Height) / 2);
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

            // Borde reactivo
            Color colorActualBorde = _tieneFoco ? _colorBordeFoco : _colorBorde;
            float grosor = _tieneFoco ? 1.8f : 1f;
            using (var penBorde = new Pen(colorActualBorde, grosor))
            {
                g.DrawPath(penBorde, path);
            }

            // Icono Chevron desplegable estilizado
            using var bmpChevron = IconChar.ChevronDown.ToBitmap(_tieneFoco ? TemaVisual.Primario : TemaVisual.TextoMuted, 10);
            int chevronX = Width - Padding.Right + 8;
            int chevronY = (Height - bmpChevron.Height) / 2;
            g.DrawImage(bmpChevron, chevronX, chevronY);
        }
    }
}
