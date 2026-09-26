using System.Security.Claims;
using HSis.Contracts.Constants;
using HSis.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HSis.Server.Hubs
{
    [Authorize]
    public sealed class NotificationHub : Hub<INotificationClient>
    {
        public override async Task OnConnectedAsync()
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId) || userId <= 0)
            {
                Context.Abort();
                return;
            }

            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                GruposNotificaciones.Usuario(userId));

            var rol = ObtenerRol(Context.User);
            var grupoRol = ObtenerGrupoRol(rol);
            if (grupoRol is not null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, grupoRol);
            }

            await base.OnConnectedAsync();
        }

        private static RolUsuarioEnum? ObtenerRol(ClaimsPrincipal? user)
        {
            var roleClaim = user?.FindFirst(ClaimTypes.Role)?.Value;
            return int.TryParse(roleClaim, out var roleId) &&
                   Enum.IsDefined(typeof(RolUsuarioEnum), roleId)
                ? (RolUsuarioEnum)roleId
                : null;
        }

        private static string? ObtenerGrupoRol(RolUsuarioEnum? rol)
        {
            return rol switch
            {
                RolUsuarioEnum.Administrador => GruposNotificaciones.Administradores,
                RolUsuarioEnum.Tecnico => GruposNotificaciones.Tecnicos,
                RolUsuarioEnum.Cliente => GruposNotificaciones.Usuarios,
                _ => null
            };
        }
    }
}

