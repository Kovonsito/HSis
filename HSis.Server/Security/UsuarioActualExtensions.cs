using System.Security.Claims;
using HSis.Contracts.Constants;

namespace HSis.Server.Security;

internal static class UsuarioActualExtensions
{
    public static int ObtenerIdUsuario(this ClaimsPrincipal usuario)
        => int.TryParse(usuario.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    public static RolUsuarioEnum? ObtenerRol(this ClaimsPrincipal usuario)
        => int.TryParse(usuario.FindFirstValue(ClaimTypes.Role), out var rol) && Enum.IsDefined(typeof(RolUsuarioEnum), rol)
            ? (RolUsuarioEnum)rol
            : null;

    public static bool EsAdministrador(this ClaimsPrincipal usuario)
        => usuario.ObtenerRol() == RolUsuarioEnum.Administrador;

    // Administrador o el propio usuario; evita que un usuario consulte datos de otro.
    public static bool EsAdministradorOMismo(this ClaimsPrincipal usuario, int idUsuario)
        => usuario.EsAdministrador() || (idUsuario > 0 && usuario.ObtenerIdUsuario() == idUsuario);
}