using HSis.Logic.Constants;
using HSis.Logic.Services;
using HSis.UI.Helpers;

namespace HSis.UI.Presenters
{
    public class IniciarSesionPresenter(
        IUsuarioService usuarioService,
        ISessionCacheService sessionCache,
        INotificationClientService notificationClient,
        IContextoSesion contextoSesion)
    {
        private IIniciarSesionView? _view;

        public void SetView(IIniciarSesionView view)
        {
            _view = view;
        }


        public void CargarCredencialesEnCache()
        {
            if (_view == null) return;
            var cached = sessionCache.GetCredentials();
            if (cached.HasValue)
            {
                _view.CargarCredencialesGuardadas(cached.Value.Username, cached.Value.Password);
            }
        }

        public async Task IniciarSesionAsync()
        {
            if (_view == null) return;

            string usuarioInput = _view.NombreUsuario;
            string passwordInput = _view.Contraseña;

            if (string.IsNullOrWhiteSpace(usuarioInput) || string.IsNullOrWhiteSpace(passwordInput))
            {
                _view.MostrarError("Por favor, ingrese usuario y contraseña.");
                return;
            }

            try
            {
                _view.MostrarCargando(true);
                var usuario = await usuarioService.AutenticarAsync(usuarioInput, passwordInput);
                if (usuario != null)
                {
                    contextoSesion.UsuarioActual = usuario;
                    SesionSistema.UsuarioActual = usuario;
                    sessionCache.SaveCredentials(usuario.Nombre ?? string.Empty, passwordInput);

                    string roleName = (RolUsuarioEnum)SesionSistema.IdRolUsuario switch
                    {
                        RolUsuarioEnum.Administrador => "Administrador",
                        RolUsuarioEnum.Tecnico => "Técnico",
                        RolUsuarioEnum.Cliente => "Cliente",
                        _ => "Usuario"
                    };

                    _ = notificationClient.IniciarAsync(SesionSistema.IdUsuario, roleName);

                    _view.NavegarADashboard(usuario, roleName);
                }
                else
                {
                    _view.MostrarError("Usuario o contraseña incorrectos");
                    _view.LimpiarCredenciales();
                }
            }
            catch (Exception ex)
            {
                _view.MostrarError($"Error al iniciar sesión: {ex.Message}");
            }
            finally
            {
                _view.MostrarCargando(false);
            }
        }
    }
}
