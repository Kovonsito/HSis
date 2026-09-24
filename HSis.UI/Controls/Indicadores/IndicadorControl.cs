#nullable enable
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using HSis.UI.Helpers;

namespace HSis.UI.Controls
{
    [SupportedOSPlatform("windows")]
    public partial class IndicadorControl : UserControl
    {
        public event EventHandler? IndicadorClic;
        private Color _colorAcento = Color.FromArgb(59, 130, 246);
        private bool _isHovered = false;

        public IndicadorControl()
        {
            InitializeComponent();
            DoubleBuffered = true;
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);

            MouseEnter += (s, e) => { _isHovered = true; Invalidate(); };
            MouseLeave += (s, e) => { _isHovered = false; Invalidate(); };
            lblTitulo.MouseEnter += (s, e) => { _isHovered = true; Invalidate(); };
            lblTitulo.MouseLeave += (s, e) => { _isHovered = false; Invalidate(); };
            lblCantidad.MouseEnter += (s, e) => { _isHovered = true; Invalidate(); };
            lblCantidad.MouseLeave += (s, e) => { _isHovered = false; Invalidate(); };
            pbxIcono.MouseEnter += (s, e) => { _isHovered = true; Invalidate(); };
            pbxIcono.MouseLeave += (s, e) => { _isHovered = false; Invalidate(); };
        }

        private string _titulo = "INDICADOR";
        private string _cantidad = "0";

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Titulo
        {
            get => _titulo;
            set
            {
                _titulo = value?.ToUpperInvariant() ?? string.Empty;
                lblTitulo.Text = _titulo;
                Invalidate();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Cantidad
        {
            get => _cantidad;
            set
            {
                _cantidad = value ?? "0";
                lblCantidad.Text = _cantidad;
                Invalidate();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color ColorFondo
        {
            get => _colorAcento;
            set
            {
                _colorAcento = value;
                Invalidate();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image? ImagenFondo
        {
            get => pbxIcono?.Image;
            set
            {
                if (pbxIcono != null)
                {
                    pbxIcono.Image = value;
                    pbxIcono.Visible = value != null;
                }
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Fondo del contenedor padre para bordes redondeados limpios
            Color colorPadre = (Parent?.BackColor != null && Parent.BackColor != Color.Transparent && Parent.BackColor.A > 0)
                ? Parent.BackColor
                : TemaVisual.FondoApp;

            using (var brushPadre = new SolidBrush(colorPadre))
            {
                g.FillRectangle(brushPadre, 0, 0, Width, Height);
            }

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using var path = TemaVisual.CrearRectanguloRedondeado(rect, 8);

            // Fondo blanco de tarjeta
            using (var brushFondo = new SolidBrush(Color.White))
            {
                g.FillPath(brushFondo, path);
            }

            // Barra lateral izquierda de acento
            var barraRect = new Rectangle(0, 0, 5, Height);
            g.SetClip(path);
            using (var brushAcento = new SolidBrush(_colorAcento))
            {
                g.FillRectangle(brushAcento, barraRect);
            }
            g.ResetClip();

            // Borde suave o destacado al hover
            using var penBorde = new Pen(_isHovered ? Color.FromArgb(59, 130, 246) : Color.FromArgb(226, 232, 240), _isHovered ? 1.5f : 1f);
            g.DrawPath(penBorde, path);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AjustarDisenoInterno();
        }

        private void AjustarDisenoInterno()
        {
            int width = ClientSize.Width;
            int height = ClientSize.Height;
            if (width <= 0 || height <= 0) return;

            // Posicionamiento de controles hijos
            int iconSide = width < 140 ? 24 : (width < 180 ? 28 : 34);
            int paddingDerecho = 14;

            if (pbxIcono != null)
            {
                pbxIcono.Size = new Size(iconSide, iconSide);
                pbxIcono.Location = new Point(Math.Max(10, width - iconSide - paddingDerecho), (height - iconSide) / 2);
            }

            int anchoDisponible = Math.Max(40, width - iconSide - paddingDerecho - 22);

            if (lblTitulo != null)
            {
                lblTitulo.Location = new Point(14, 14);
                lblTitulo.MaximumSize = new Size(anchoDisponible, 20);
                lblTitulo.AutoEllipsis = true;
            }

            if (lblCantidad != null)
            {
                lblCantidad.Location = new Point(12, 36);
                lblCantidad.MaximumSize = new Size(anchoDisponible, 40);
                lblCantidad.AutoEllipsis = true;
            }
        }

        private void Indicador_Click(object? sender, EventArgs e)
        {
            IndicadorClic?.Invoke(this, e);
        }
    }
}
