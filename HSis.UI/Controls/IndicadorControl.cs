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
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]

        // Propiedad para cambiar el título desde afuera
        public string Titulo
        {
            get => lblTitulo.Text;
            set => lblTitulo.Text = value;
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]

        // Propiedad para cambiar el número
        public string Cantidad
        {
            get => lblCantidad.Text;
            set => lblCantidad.Text = value;
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

        private void Indicador_Click(object sender, EventArgs e)
        {
            // Si alguien se suscribió al evento, le avisamos
            IndicadorClic?.Invoke(this, e);
        }

    }
}
