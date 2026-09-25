using HSis.Contracts.DTOs;

namespace HSis.Contracts.Services;

public interface IRolUsuarioService
{
    Task<List<RolUsuarioDto>> ObtenerRolesUsuarioAsync();
    Task<RolUsuarioDto> CrearRolUsuarioAsync(RolUsuarioCatalogoRequestDto request);
    Task<RolUsuarioDto?> ActualizarRolUsuarioAsync(int idRol, RolUsuarioCatalogoRequestDto request);
    Task<bool> EliminarRolUsuarioAsync(int idRol);
}
