using HSis.Logic.Services;

namespace HSis.UI
{
    public partial class frmIniciarSesion : Form
    {
        private readonly UsuarioService _usuarioService;

        public frmIniciarSesion(UsuarioService usuarioService)
        {
            InitializeComponent();
            _usuarioService = usuarioService;
        }

        private async void btnIniciarSesion_Click(object sender, EventArgs e)
        {
            var usuario = await _usuarioService.AutenticarAsync(txtUsuario.Text, txtContraseña.Text);
            if (usuario != null)
            {
                SesionSistema.UsuarioActual = usuario;

                Form dashboardForm = SesionSistema.IdRolUsuario switch
                {
                    1 => (Form)Program.ServiceProvider.GetService(typeof(frmDashboardAdmin))!,
                    2 => (Form)Program.ServiceProvider.GetService(typeof(frmDashboardTecnico))!,
                    3 => (Form)Program.ServiceProvider.GetService(typeof(frmDashboardCliente))!,
                    _ => (Form)Program.ServiceProvider.GetService(typeof(frmDashboardAdmin))!
                };

                // Suscribirse al evento FormClosed para cerrar la aplicación correctamente
                dashboardForm.FormClosed += (s, closedArgs) => Application.Exit();

                this.Hide();
                dashboardForm.Show();
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos", "Error al iniciar sesión", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsuario.Clear();
                txtContraseña.Clear();
            }
        }

        private async void frmIniciarSesion_Load(object sender, EventArgs e)
        {
            //await _usuarioService.RehashearContraseñasAsync();
        }
    }
}
