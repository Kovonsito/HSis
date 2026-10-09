using HSis.Contracts.Services;
using HSis.Contracts.Constants;
using HSis.Contracts.DTOs;
using HSis.Desktop.Infrastructure;

namespace HSis.UI.Services
{
    public class AdministradorSesionUsuario(PresenciaAplicacion presenciaAplicacion) : IAdministradorSesionUsuario, ICurrentUserService
    {
        private readonly PresenciaAplicacion _presenciaAplicacion = presenciaAplicacion;
        private UsuarioDto? _usuarioActual;

        public UsuarioDto? UsuarioActual
        {
            get => _usuarioActual;
            set
            {
                _usuarioActual = value;
                if (value is null)
                {
                    _presenciaAplicacion.Desactivar();
                }
                else
                {
                    _presenciaAplicacion.Activar();
                }
            }
        }
        public string TokenJWT { get; set; } = string.Empty;
        public int IdUsuario => UsuarioActual?.IdUsuario ?? 0;
        public string NombreUsuario => UsuarioActual?.Nombre ?? string.Empty;
        public int IdRolUsuario => UsuarioActual?.IdRol ?? 0;
        public bool EsAdmin => IdRolUsuario == (int)RolUsuarioEnum.Administrador;
        public bool EsTecnico => IdRolUsuario == (int)RolUsuarioEnum.Tecnico;

        public int GetCurrentUserId() => IdUsuario;

        public void IniciarSesion(UsuarioDto usuario, string token)
        {
            UsuarioActual = usuario;
            TokenJWT = token;
        }

        public void CerrarSesion()
        {
            UsuarioActual = null;
            TokenJWT = string.Empty;
        }
    }
}
