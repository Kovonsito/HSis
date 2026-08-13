#nullable enable
using System.Runtime.Versioning;

namespace HSis.UI.Controls
{
    [SupportedOSPlatform("windows")]
    public partial class BannerConexionControl : UserControl
    {
        public BannerConexionControl()
        {
            InitializeComponent();
        }

        public void MostrarEstado(bool conectado, string mensaje, Color colorFondo)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => MostrarEstado(conectado, mensaje, colorFondo)));
                return;
            }

            Visible = !conectado;
            lblMensaje.Text = mensaje;
            BackColor = colorFondo;
        }
    }
}

