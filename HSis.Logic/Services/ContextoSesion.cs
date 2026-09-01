using HSis.Logic.Constants;
using HSis.Logic.DTOs;

namespace HSis.Logic.Services
{
    public class ContextoSesion : IContextoSesion, ICurrentUserService
    {
        public UsuarioDto? UsuarioActual { get; set; }
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

