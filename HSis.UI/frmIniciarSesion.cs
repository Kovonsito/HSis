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
            InicializarLayoutLogin();
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

        private void frmIniciarSesion_Load(object? sender, EventArgs e)
        {
            var cached = SessionCacheService.GetCredentials();
            if (cached.HasValue)
            {
                txtUsuario.Text = cached.Value.Username;
                txtContraseña.Text = cached.Value.Password;
            }
        }

        private void InicializarLayoutLogin()
        {
            var tblPrincipal = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 5,
                ColumnCount = 1,
                Padding = new Padding(30, 20, 30, 20),
                Name = "tblPrincipal"
            };

            for (int i = 0; i < 5; i++)
            {
                tblPrincipal.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }

            lblUsuario.Dock = DockStyle.Fill;
            lblUsuario.Margin = new Padding(0, 0, 0, 5);
            txtUsuario.Dock = DockStyle.Fill;
            txtUsuario.Margin = new Padding(0, 0, 0, 15);

            lblContraseña.Dock = DockStyle.Fill;
            lblContraseña.Margin = new Padding(0, 0, 0, 5);
            txtContraseña.Dock = DockStyle.Fill;
            txtContraseña.Margin = new Padding(0, 0, 0, 20);

            btnIniciarSesion.Dock = DockStyle.Fill;
            btnIniciarSesion.Margin = new Padding(0);
            btnIniciarSesion.Height = 35;

            tblPrincipal.Controls.Add(lblUsuario, 0, 0);
            tblPrincipal.Controls.Add(txtUsuario, 0, 1);
            tblPrincipal.Controls.Add(lblContraseña, 0, 2);
            tblPrincipal.Controls.Add(txtContraseña, 0, 3);
            tblPrincipal.Controls.Add(btnIniciarSesion, 0, 4);

            this.Controls.Clear();
            this.Controls.Add(tblPrincipal);
        }
    }
}
