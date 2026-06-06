#nullable enable
using HSis.Logic.Services;
using System.Runtime.Versioning;
using HSis.Data.Models;

namespace HSis.UI
{
    [SupportedOSPlatform("windows")]
    public partial class frmIniciarSesion : Form
    {
        private readonly UsuarioService _usuarioService;

        public frmIniciarSesion(UsuarioService usuarioService)
        {
            InitializeComponent();
            _usuarioService = usuarioService;
        }

        private async void btnIniciarSesion_Click(object? sender, EventArgs e)
        {
            var usuario = await _usuarioService.AutenticarAsync(txtUsuario.Text, txtContraseña.Text);
            if (usuario != null)
            {
                ProcesarLoginExitoso(usuario, txtContraseña.Text);
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos", "Error al iniciar sesión", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsuario.Clear();
                txtContraseña.Clear();
            }
        }

        private void ProcesarLoginExitoso(Usuario usuario, string password)
        {
            SesionSistema.UsuarioActual = usuario;

            // Guardar credenciales en caché
            SessionCacheService.SaveCredentials(usuario.Nombre ?? string.Empty, password);

            // Iniciar servicio cliente de SignalR en segundo plano
            var notificationClient = (NotificationClientService)Program.ServiceProvider.GetService(typeof(NotificationClientService))!;
            string roleName = SesionSistema.IdRolUsuario switch
            {
                1 => "Admin",
                2 => "Tecnico",
                3 => "Cliente",
                _ => "Usuario"
            };
            _ = notificationClient.IniciarAsync(SesionSistema.IdUsuario, roleName);

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

        private async void frmIniciarSesion_Load(object? sender, EventArgs e)
        {
            var cached = SessionCacheService.GetCredentials();
            if (cached.HasValue)
            {
                txtUsuario.Text = cached.Value.Username;
                txtContraseña.Text = cached.Value.Password;

                var usuario = await _usuarioService.AutenticarAsync(cached.Value.Username, cached.Value.Password);
                if (usuario != null)
                {
                    ProcesarLoginExitoso(usuario, cached.Value.Password);
                }
            }
        }
    }
}
