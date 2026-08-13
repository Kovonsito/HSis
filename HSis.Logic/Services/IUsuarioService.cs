using HSis.Logic.Constants;
using HSis.Logic.DTOs;

namespace HSis.Logic.Services
{
    public interface IUsuarioService
    {
        Task RehashearContraseñasAsync();
        Task<UsuarioDto?> AutenticarAsync(string nombreUsuario, string contraseña);
        Task<List<UsuarioDto>> ObtenerUsuariosPorRolAsync(int idRol);
        Task<List<UsuarioDto>> ObtenerUsuariosPorRolAsync(RolUsuarioEnum rol)
        {
            return ObtenerUsuariosPorRolAsync((int)rol);
        }

    }
}
