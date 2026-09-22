using HSis.Contracts.DTOs;

namespace HSis.Contracts.Services
{
    public interface IAdministradorSesionUsuario
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
