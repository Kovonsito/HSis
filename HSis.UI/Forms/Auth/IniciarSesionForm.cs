#nullable enable
using System.Runtime.Versioning;
using HSis.Contracts.Constants;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.UI.Factories;
using HSis.UI.Forms.Dashboards;
using HSis.UI.Helpers;
using HSis.UI.Services;

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
        private readonly AgenteNotificacionesLauncher _notificationAgentLauncher;
        private readonly AperturaTicketService _aperturaTicketService;

        public IniciarSesionForm(
            IUsuarioService usuarioService,
            IAdministradorSesionUsuario contextoSesion,
            IAlmacenamientoCredencialesLocal sessionCache,
            IFabricaFormularios formFactory,
            IClienteSignalRNotificaciones notificationClient,
            AgenteNotificacionesLauncher notificationAgentLauncher,
            AperturaTicketService aperturaTicketService)
        {
            InitializeComponent();
            _usuarioService = usuarioService;
            _contextoSesion = contextoSesion;
            _sessionCache = sessionCache;
            _formFactory = formFactory;
            _notificationClient = notificationClient;
            _notificationAgentLauncher = notificationAgentLauncher;
            _aperturaTicketService = aperturaTicketService;

            InicializarLayoutLogin();
            ConfigurarEventosValidacion();
        }

        private void ConfigurarEventosValidacion()
        {
            txtUsuario.TextChanged += (_, _) => LimpiarErrorCredenciales();
            txtContraseña.TextChanged += (_, _) => LimpiarErrorCredenciales();
        }

        private void MostrarErrorCredenciales(string mensaje)
        {
            txtUsuario.TieneError = true;
            txtContraseña.TieneError = true;
            lblErrorCredenciales.Text = mensaje;
            lblErrorCredenciales.Visible = true;
        }

        private void LimpiarErrorCredenciales()
        {
            if (lblErrorCredenciales.Visible || txtUsuario.TieneError || txtContraseña.TieneError)
            {
                txtUsuario.TieneError = false;
                txtContraseña.TieneError = false;
                lblErrorCredenciales.Visible = false;
            }
        }

        public void LimpiarCredenciales()
        {
            txtUsuario.Clear();
            txtContraseña.Clear();
            LimpiarErrorCredenciales();
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
            dashboardForm.Shown += (_, _) =>
                _aperturaTicketService.AbrirPendiente(dashboardForm);
            this.Hide();
            dashboardForm.Show();
        }

        #region Form Events
        private async void BtnIniciarSesion_Click(object? sender, EventArgs e)
        {
            string usuarioInput = txtUsuario.Text.Trim();
            string passwordInput = txtContraseña.Text;

            if (string.IsNullOrWhiteSpace(usuarioInput) || string.IsNullOrWhiteSpace(passwordInput))
            {
                MostrarErrorCredenciales("Por favor, ingrese usuario y contraseña.");
                if (string.IsNullOrWhiteSpace(usuarioInput)) txtUsuario.Focus();
                else txtContraseña.Focus();
                return;
            }

            await this.EjecutarOperacionAsync(async () =>
            {
                var usuario = await _usuarioService.AutenticarAsync(usuarioInput, passwordInput);
                if (usuario != null)
                {
                    LimpiarErrorCredenciales();
                    _contextoSesion.UsuarioActual = usuario;
                    _sessionCache.SaveCredentials(usuario.Nombre ?? string.Empty, passwordInput);

                    string roleName = (RolUsuarioEnum)_contextoSesion.IdRolUsuario switch
                    {
                        RolUsuarioEnum.Administrador => "Administrador",
                        RolUsuarioEnum.Tecnico => "Técnico",
                        RolUsuarioEnum.Cliente => "Cliente",
                        _ => "Usuario"
                    };

                    await _notificationClient.IniciarAsync(_contextoSesion.TokenJWT);
                    _notificationAgentLauncher.Iniciar();

                    NavegarADashboard(usuario, roleName);
                }
                else
                {
                    MostrarErrorCredenciales("El usuario o contraseña es incorrecta");
                    txtContraseña.SelectAll();
                    txtContraseña.Focus();
                }
            }, "Error al iniciar sesión", "iniciar-sesion", btnIniciarSesion);
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
