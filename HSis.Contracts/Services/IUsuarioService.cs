using HSis.Contracts.Constants;
using HSis.Contracts.DTOs;

namespace HSis.Contracts.Services
{
    public interface IUsuarioService
    {
        Task RehashearContraseñasAsync();
        Task<UsuarioDto?> AutenticarAsync(string nombreUsuario, string contraseña);
        Task<List<UsuarioDto>> ObtenerUsuariosPorRolAsync(int idRol);
        Task<List<UsuarioDto>> ObtenerUsuariosPorRolAsync(RolUsuarioEnum rol) => ObtenerUsuariosPorRolAsync((int)rol);
    }
}

