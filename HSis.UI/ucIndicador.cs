using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace HSis.UI
{
    public partial class ucIndicador : UserControl
    {
        public event EventHandler ucIndicadorEvent;

        public ucIndicador()
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
        public Image ImagenFondo
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

        private void ucIndicador_Click(object sender, EventArgs e)
        {
            // Si alguien se suscribió al evento, le avisamos
            ucIndicadorEvent?.Invoke(this, e);
        }

    }
}
