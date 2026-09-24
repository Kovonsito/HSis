#nullable enable
using System.Runtime.Versioning;
using HSis.Contracts.Services;
using HSis.UI.Controls;
using HSis.UI.Factories;
using HSis.UI.Helpers;

namespace HSis.UI.Coordinators
{
    /// <summary>
    /// Base para coordinadores unificados de Dashboard.
    /// Ensambla de forma declarativa la navegación (Sidebar + TopBar),
    /// la sesión de usuario y el panel de notificaciones en tiempo real.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public abstract class CoordinadorDashboardBase(
        Form formulario,
        SidebarControl sidebar,
        TopBarControl topBar,
        VistaTicketsDashboardControl vistaTickets,
        IAdministradorSesionUsuario contextoSesion,
        IAlmacenamientoCredencialesLocal sessionCache,
        IFabricaFormularios formFactory,
        IClienteSignalRNotificaciones notificationClient)

    {
        protected readonly Form Formulario = formulario;
        protected readonly SidebarControl Sidebar = sidebar;
        protected readonly TopBarControl TopBar = topBar;
        protected readonly VistaTicketsDashboardControl VistaTickets = vistaTickets;
        protected readonly IAdministradorSesionUsuario ContextoSesion = contextoSesion;
        protected readonly IAlmacenamientoCredencialesLocal SessionCache = sessionCache;
        protected readonly IFabricaFormularios FormFactory = formFactory;
        protected readonly IClienteSignalRNotificaciones NotificationClient = notificationClient;

        public virtual async Task IniciarAsync()
        {
            // 1. Configurar navegación
            ConfigurarNavegacion();

            // 2. Configurar vista de tickets
            ConfigurarVistaTickets();

            // 3. Integrar SignalR y notificaciones flotantes
            Formulario.IntegrarNotificacionesModerno(
                TopBar,
                FormFactory,
                ContextoSesion,
                NotificationClient,
                null,
                RecargarDatosAsync
            );

            // 4. Carga inicial de datos
            await RecargarDatosAsync();
        }

        protected abstract void ConfigurarNavegacion();
        protected abstract void ConfigurarVistaTickets();
        public abstract Task RecargarDatosAsync();
    }
}
