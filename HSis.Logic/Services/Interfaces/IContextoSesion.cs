using HSis.Logic.DTOs;

namespace HSis.Logic.Services
{
    public interface IContextoSesion
    {
        UsuarioDto? UsuarioActual { get; set; }
        string TokenJWT { get; set; }
        int IdUsuario { get; }
        string NombreUsuario { get; }
        int IdRolUsuario { get; }
        bool EsAdmin { get; }
        bool EsTecnico { get; }
        void IniciarSesion(UsuarioDto usuario, string token);
        void CerrarSesion();
    }
}

