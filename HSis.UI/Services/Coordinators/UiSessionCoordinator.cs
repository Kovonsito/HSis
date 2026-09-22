#nullable enable
using System.Runtime.Versioning;
using HSis.Logic.Services;
using HSis.UI.Factories;

namespace HSis.UI.Services.Coordinators
{
    [SupportedOSPlatform("windows")]
    public class UiSessionCoordinator : IUiSessionCoordinator
    {
        public IContextoSesion ContextoSesion { get; }
        public ISessionCacheService SessionCache { get; }
        public IFabricaFormularios FabricaFormularios { get; }
        public INotificationClientService NotificationClient { get; }
        public INotificacionStorageService NotificacionStorage { get; }
        public INotificationEventBus? NotificationEventBus { get; }

        public UiSessionCoordinator(
            IContextoSesion contextoSesion,
            ISessionCacheService sessionCache,
            IFabricaFormularios fabricaFormularios,
            INotificationClientService notificationClient,
            INotificacionStorageService notificacionStorage,
            INotificationEventBus? notificationEventBus = null)
        {
            ContextoSesion = contextoSesion;
            SessionCache = sessionCache;
            FabricaFormularios = fabricaFormularios;
            NotificationClient = notificationClient;
            NotificacionStorage = notificacionStorage;
            NotificationEventBus = notificationEventBus;
        }
    }
}
