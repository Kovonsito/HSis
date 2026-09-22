#nullable enable
using System.Runtime.Versioning;
using HSis.Logic.Services;
using HSis.UI.Factories;

namespace HSis.UI.Services.Coordinators
{
    /// <summary>
    /// Fachada que agrupa y coordina los servicios transversales de sesión,
    /// caché de credenciales, navegación/fábrica de modales y notificaciones en tiempo real para la UI.
    /// Resuelve la sobreinyección de dependencias en los formularios principales.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public interface IUiSessionCoordinator
    {
        IContextoSesion ContextoSesion { get; }
        ISessionCacheService SessionCache { get; }
        IFabricaFormularios FabricaFormularios { get; }
        INotificationClientService NotificationClient { get; }
        INotificacionStorageService NotificacionStorage { get; }
        INotificationEventBus? NotificationEventBus { get; }

        int IdUsuarioActual => ContextoSesion.UsuarioActual?.IdUsuario ?? 0;
        string NombreUsuarioActual => ContextoSesion.UsuarioActual?.Nombre ?? string.Empty;
    }
}
