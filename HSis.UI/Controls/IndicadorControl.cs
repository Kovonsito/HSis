#nullable enable
using System.ComponentModel;
using System.Runtime.Versioning;

namespace HSis.UI.Controls
{
    [SupportedOSPlatform("windows")]
    public partial class IndicadorControl : UserControl
    {
        public event EventHandler? IndicadorClic;

        public IndicadorControl()
        {
            InitializeComponent();
            if (pnlPrincipal != null)
            {
                pnlPrincipal.SizeChanged += (s, e) => AjustarDisenoInterno();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        // Propiedad para cambiar el título desde afuera
        public string Titulo
        {
            get => lblTitulo.Text;
            set
            {
                lblTitulo.Text = value;
                AjustarDisenoInterno();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        // Propiedad para cambiar el número
        public string Cantidad
        {
            get => lblCantidad.Text;
            set
            {
                lblCantidad.Text = value;
                AjustarDisenoInterno();
            }
        }

        // Propiedad para cambiar el color de fondo
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color ColorFondo
        {
            get => pnlPrincipal.BackColor;
            set => pnlPrincipal.BackColor = value;
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
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AjustarDisenoInterno();
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            AjustarDisenoInterno();
        }

        private void AjustarDisenoInterno()
        {
            if (pnlPrincipal == null || lblTitulo == null || lblCantidad == null || pbxIcono == null)
                return;

            int width = pnlPrincipal.ClientSize.Width;
            int height = pnlPrincipal.ClientSize.Height;

            if (width <= 0 || height <= 0)
                return;

            pnlPrincipal.SuspendLayout();

            // 1. Título
            lblTitulo.Location = new Point(8, 6);
            float titleFontSize = Math.Clamp(height * 0.11f, 9f, 12f);
            if (Math.Abs(lblTitulo.Font.Size - titleFontSize) > 0.5f)
            {
                lblTitulo.Font = new Font(lblTitulo.Font.FontFamily, titleFontSize, lblTitulo.Font.Style);
            }
            lblTitulo.MaximumSize = new Size(width - 16, 0);

            // 2. Ícono (Esquina inferior derecha con margen seguro)
            int marginX = Math.Clamp((int)(width * 0.05f), 8, 14);
            int marginY = Math.Clamp((int)(height * 0.10f), 8, 14);
            int iconSide = Math.Clamp((int)(Math.Min(width, height) * 0.35f), 20, 38);

            pbxIcono.Size = new Size(iconSide, iconSide);
            pbxIcono.Location = new Point(width - iconSide - marginX, height - iconSide - marginY);
            pbxIcono.BringToFront();

            // 3. Cantidad (Centrado en la tarjeta)
            float numFontSize = Math.Clamp(height * 0.26f, 14f, 26f);
            if (Math.Abs(lblCantidad.Font.Size - numFontSize) > 0.5f)
            {
                lblCantidad.Font = new Font(lblCantidad.Font.FontFamily, numFontSize, FontStyle.Bold);
            }

            int cantX = Math.Max(4, (width - lblCantidad.Width) / 2);
            int cantY = Math.Max(lblTitulo.Bottom + 2, (height - lblCantidad.Height) / 2);
            lblCantidad.Location = new Point(cantX, cantY);

            pnlPrincipal.ResumeLayout(false);
            pnlPrincipal.PerformLayout();
        }

        private void Indicador_Click(object sender, EventArgs e)
        {
            // Si alguien se suscribió al evento, le avisamos
            IndicadorClic?.Invoke(this, e);
        }
    }
}
