#nullable enable
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using System.Windows.Forms;
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
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            MouseEnter += (s, e) => { _isHovered = true; Invalidate(); };
            MouseLeave += (s, e) => { _isHovered = false; Invalidate(); };
            lblTitulo.MouseEnter += (s, e) => { _isHovered = true; Invalidate(); };
            lblTitulo.MouseLeave += (s, e) => { _isHovered = false; Invalidate(); };
            lblCantidad.MouseEnter += (s, e) => { _isHovered = true; Invalidate(); };
            lblCantidad.MouseLeave += (s, e) => { _isHovered = false; Invalidate(); };
            pbxIcono.MouseEnter += (s, e) => { _isHovered = true; Invalidate(); };
            pbxIcono.MouseLeave += (s, e) => { _isHovered = false; Invalidate(); };
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Titulo
        {
            get => lblTitulo.Text;
            set
            {
                lblTitulo.Text = value.ToUpperInvariant();
                AjustarDisenoInterno();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Cantidad
        {
            get => lblCantidad.Text;
            set
            {
                lblCantidad.Text = value;
                AjustarDisenoInterno();
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Limpiar fondo con el color del padre
            Color colorPadre = Parent?.BackColor ?? Color.FromArgb(248, 250, 252);
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

            // Barra lateral izquierda de acento de color
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

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AjustarDisenoInterno();
            Invalidate();
        }

        private void AjustarDisenoInterno()
        {
            if (lblTitulo == null || lblCantidad == null || pbxIcono == null)
                return;

            int width = ClientSize.Width;
            int height = ClientSize.Height;
            if (width <= 0 || height <= 0) return;

            // 1. Título arriba a la izquierda
            lblTitulo.Location = new Point(18, 14);
            lblTitulo.MaximumSize = new Size(Math.Max(10, width - 68), 18);

            // 2. Ícono arriba a la derecha
            int iconSide = 36;
            pbxIcono.Size = new Size(iconSide, iconSide);
            pbxIcono.Location = new Point(width - iconSide - 16, 14);

            // 3. Cantidad grande abajo a la izquierda
            lblCantidad.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            lblCantidad.Location = new Point(18, 40);
        }

        private void Indicador_Click(object? sender, EventArgs e)
        {
            IndicadorClic?.Invoke(this, e);
        }
    }
}
