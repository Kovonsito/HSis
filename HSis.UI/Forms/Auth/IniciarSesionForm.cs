#nullable enable
using System.Runtime.Versioning;
using HSis.Contracts.Constants;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.UI.Factories;
using HSis.UI.Forms.Dashboards;
using HSis.UI.Helpers;

namespace HSis.UI.Forms.Auth
{
    [SupportedOSPlatform("windows")]
    public partial class IniciarSesionForm : Form
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IAdministradorSesionUsuario _contextoSesion;
        private readonly IAlmacenamientoCredencialesLocal _sessionCache;
        private readonly IFabricaFormularios _formFactory;
        private readonly IClienteSignalRNotificaciones _notificationClient;

        public IniciarSesionForm(
            IUsuarioService usuarioService,
            IAdministradorSesionUsuario contextoSesion,
            IAlmacenamientoCredencialesLocal sessionCache,
            IFabricaFormularios formFactory,
            IClienteSignalRNotificaciones notificationClient)
        {
            InitializeComponent();
            _usuarioService = usuarioService;
            _contextoSesion = contextoSesion;
            _sessionCache = sessionCache;
            _formFactory = formFactory;
            _notificationClient = notificationClient;

            InicializarLayoutLogin();
        }

        public void LimpiarCredenciales()
        {
            txtUsuario.Clear();
            txtContraseña.Clear();
        }

        public void CargarCredencialesGuardadas(string usuario, string contraseña)
        {
            txtUsuario.Text = usuario;
            txtContraseña.Text = contraseña;
        }

        public void NavegarADashboard(UsuarioDto usuario, string rolNombre)
        {
            Form dashboardForm = (RolUsuarioEnum)(usuario.IdRol ?? (int)RolUsuarioEnum.Cliente) switch
            {
                RolUsuarioEnum.Administrador => _formFactory.Crear<DashboardAdminForm>(),
                RolUsuarioEnum.Tecnico => _formFactory.Crear<DashboardTecnicoForm>(),
                RolUsuarioEnum.Cliente => _formFactory.Crear<DashboardClienteForm>(),
                _ => _formFactory.Crear<DashboardClienteForm>()
            };

            dashboardForm.FormClosed += (s, closedArgs) => Application.Exit();
            this.Hide();
            dashboardForm.Show();
        }

        #region Form Events
        private async void BtnIniciarSesion_Click(object? sender, EventArgs e)
        {
            string usuarioInput = txtUsuario.Text;
            string passwordInput = txtContraseña.Text;

            if (string.IsNullOrWhiteSpace(usuarioInput) || string.IsNullOrWhiteSpace(passwordInput))
            {
                DialogoUIHelper.MostrarAdvertencia("Por favor, ingrese usuario y contraseña.", "Campos requeridos");
                return;
            }

            await this.EjecutarOperacionAsync(async () =>
            {
                var usuario = await _usuarioService.AutenticarAsync(usuarioInput, passwordInput);
                if (usuario != null)
                {
                    _contextoSesion.UsuarioActual = usuario;
                    _sessionCache.SaveCredentials(usuario.Nombre ?? string.Empty, passwordInput);

                    string roleName = (RolUsuarioEnum)_contextoSesion.IdRolUsuario switch
                    {
                        RolUsuarioEnum.Administrador => "Administrador",
                        RolUsuarioEnum.Tecnico => "Técnico",
                        RolUsuarioEnum.Cliente => "Cliente",
                        _ => "Usuario"
                    };

                    _ = _notificationClient.IniciarAsync(_contextoSesion.IdUsuario, roleName);

                    NavegarADashboard(usuario, roleName);
                }
                else
                {
                    DialogoUIHelper.MostrarAdvertencia("Usuario o contraseña incorrectos", "Credenciales Inválidas");
                    LimpiarCredenciales();
                }
            }, "Error al iniciar sesión", btnIniciarSesion);
        }

        private void FrmIniciarSesion_Load(object? sender, EventArgs e)
        {
            var cached = _sessionCache.GetCredentials();
            if (cached.HasValue)
            {
                CargarCredencialesGuardadas(cached.Value.Username, cached.Value.Password);
            }
        }
        #endregion
    }
}
