#nullable enable
using System.Runtime.Versioning;
using HSis.Logic.DTOs;
using HSis.Logic.Services;
using HSis.UI.Factories;
using HSis.UI.Forms.Dashboards;
using HSis.UI.Helpers;

namespace HSis.UI.Forms.Auth
{
    [SupportedOSPlatform("windows")]
    public partial class IniciarSesionForm : Form
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ISessionCacheService _sessionCache;
        private readonly IFabricaFormularios _fabricaFormularios;
        private readonly INotificationClientService _notificationClient;

        public IniciarSesionForm(IUsuarioService usuarioService, ISessionCacheService sessionCache, IFabricaFormularios fabricaFormularios, INotificationClientService notificationClient)
        {
            InitializeComponent();
            _usuarioService = usuarioService;
            _sessionCache = sessionCache;
            _fabricaFormularios = fabricaFormularios;
            _notificationClient = notificationClient;
            InicializarLayoutLogin();
        }


        #region Login Actions
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

        private void ProcesarLoginExitoso(UsuarioDto usuario, string password)
        {
            SesionSistema.UsuarioActual = usuario;

            // Guardar credenciales en caché
            _sessionCache.SaveCredentials(usuario.Nombre ?? string.Empty, password);

            string roleName = SesionSistema.IdRolUsuario switch
            {
                1 => "Administrador",
                2 => "Técnico",
                3 => "Cliente",
                _ => "Usuario"
            };
            _ = _notificationClient.IniciarAsync(SesionSistema.IdUsuario, roleName);

            Form dashboardForm = SesionSistema.IdRolUsuario switch
            {
                1 => (Form)_fabricaFormularios.Crear<DashboardAdminForm>(),
                2 => (Form)_fabricaFormularios.Crear<DashboardTecnicoForm>(),
                3 => (Form)_fabricaFormularios.Crear<DashboardClienteForm>(),
                _ => (Form)_fabricaFormularios.Crear<DashboardAdminForm>()
            };

            // Suscribirse al evento FormClosed para cerrar la aplicación correctamente
            dashboardForm.FormClosed += (s, closedArgs) => Application.Exit();

            this.Hide();
            dashboardForm.Show();
        }

        private void frmIniciarSesion_Load(object? sender, EventArgs e)
        {
            var cached = _sessionCache.GetCredentials();
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

            btnIniciarSesion.Anchor = AnchorStyles.Left | AnchorStyles.Right;
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
        #endregion
    }
}
