using System.Collections.Generic;
using System.Threading.Tasks;
using HSis.Logic.DTOs;

namespace HSis.Logic.Services
{
    public interface IUsuarioService
    {
        Task RehashearContraseñasAsync();
        Task<UsuarioDto?> AutenticarAsync(string nombreUsuario, string contraseña);
        Task<List<UsuarioDto>> ObtenerUsuariosPorRolAsync(int idRol);
    }
}
