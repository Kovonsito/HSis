#nullable enable
using System.Runtime.Versioning;
using HSis.Logic.Constants;
using HSis.Logic.DTOs;
using HSis.Logic.Services;
using HSis.UI.Factories;
using HSis.UI.Forms.Dashboards;
using HSis.UI.Helpers;

using HSis.UI.Services.Coordinators;

namespace HSis.UI.Forms.Auth
{
    [SupportedOSPlatform("windows")]
    public partial class IniciarSesionForm : Form
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IUiSessionCoordinator _sessionCoordinator;

        public IniciarSesionForm(
            IUsuarioService usuarioService,
            IUiSessionCoordinator sessionCoordinator)
        {
            InitializeComponent();
            _usuarioService = usuarioService;
            _sessionCoordinator = sessionCoordinator;

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
                RolUsuarioEnum.Administrador => _sessionCoordinator.FabricaFormularios.Crear<DashboardAdminForm>(),
                RolUsuarioEnum.Tecnico => _sessionCoordinator.FabricaFormularios.Crear<DashboardTecnicoForm>(),
                RolUsuarioEnum.Cliente => _sessionCoordinator.FabricaFormularios.Crear<DashboardClienteForm>(),
                _ => _sessionCoordinator.FabricaFormularios.Crear<DashboardClienteForm>()
            };

            dashboardForm.FormClosed += (s, closedArgs) => Application.Exit();
            this.Hide();
            dashboardForm.Show();
        }

        public void MostrarError(string mensaje)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => MostrarError(mensaje)));
                return;
            }
            MessageBox.Show(mensaje, "Error al iniciar sesión", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public void MostrarCargando(bool cargando)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => MostrarCargando(cargando)));
                return;
            }
            btnIniciarSesion.Enabled = !cargando;
            this.UseWaitCursor = cargando;
        }

        #region Form Events
        private async void BtnIniciarSesion_Click(object? sender, EventArgs e)
        {
            string usuarioInput = txtUsuario.Text;
            string passwordInput = txtContraseña.Text;

            if (string.IsNullOrWhiteSpace(usuarioInput) || string.IsNullOrWhiteSpace(passwordInput))
            {
                MostrarError("Por favor, ingrese usuario y contraseña.");
                return;
            }

            await this.EjecutarOperacionAsync(async () =>
            {
                var usuario = await _usuarioService.AutenticarAsync(usuarioInput, passwordInput);
                if (usuario != null)
                {
                    _sessionCoordinator.ContextoSesion.UsuarioActual = usuario;
                    SesionSistema.UsuarioActual = usuario;
                    _sessionCoordinator.SessionCache.SaveCredentials(usuario.Nombre ?? string.Empty, passwordInput);

                    string roleName = (RolUsuarioEnum)SesionSistema.IdRolUsuario switch
                    {
                        RolUsuarioEnum.Administrador => "Administrador",
                        RolUsuarioEnum.Tecnico => "Técnico",
                        RolUsuarioEnum.Cliente => "Cliente",
                        _ => "Usuario"
                    };

                    _ = _sessionCoordinator.NotificationClient.IniciarAsync(SesionSistema.IdUsuario, roleName);

                    NavegarADashboard(usuario, roleName);
                }
                else
                {
                    MostrarError("Usuario o contraseña incorrectos");
                    LimpiarCredenciales();
                }
            }, "Error al iniciar sesión", btnIniciarSesion);
        }

        private void FrmIniciarSesion_Load(object? sender, EventArgs e)
        {
            var cached = _sessionCoordinator.SessionCache.GetCredentials();
            if (cached.HasValue)
            {
                CargarCredencialesGuardadas(cached.Value.Username, cached.Value.Password);
            }
        }
        #endregion
    }
}
