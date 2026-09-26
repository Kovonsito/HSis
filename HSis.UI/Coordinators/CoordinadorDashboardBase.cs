#nullable enable
using System.Runtime.Versioning;
using System.Threading;
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
        IClienteSignalRNotificaciones notificationClient,
        INotificacionesApiClient notificacionesApiClient,
        IBusEventosNotificaciones eventBus)

    {
        protected readonly Form Formulario = formulario;
        protected readonly SidebarControl Sidebar = sidebar;
        protected readonly TopBarControl TopBar = topBar;
        protected readonly VistaTicketsDashboardControl VistaTickets = vistaTickets;
        protected readonly IAdministradorSesionUsuario ContextoSesion = contextoSesion;
        protected readonly IAlmacenamientoCredencialesLocal SessionCache = sessionCache;
        protected readonly IFabricaFormularios FormFactory = formFactory;
        protected readonly IClienteSignalRNotificaciones NotificationClient = notificationClient;
        protected readonly INotificacionesApiClient NotificacionesApiClient = notificacionesApiClient;
        protected readonly IBusEventosNotificaciones EventBus = eventBus;
        protected EstadoCargaAsync EstadoCarga => Formulario.ObtenerEstadoCarga();

        public virtual async Task IniciarAsync()
        {
            // 1. Configurar navegación
            ConfigurarNavegacion();

            // 2. Configurar vista de tickets
            ConfigurarVistaTickets();

            // Permite cargar catálogos dinámicos antes de conectar los eventos
            // públicos del filtro y de realizar la primera consulta.
            await PrepararFiltrosAsync();

            // Los filtros se configuran antes de suscribirnos para no provocar
            // consultas parciales durante la construcción de la vista.
            SuscribirRecargaAutomaticaDeFiltros();

            // 3. Integrar SignalR y notificaciones flotantes
            Formulario.IntegrarNotificacionesModerno(
                TopBar,
                FormFactory,
                ContextoSesion,
                NotificationClient,
                NotificacionesApiClient,
                EventBus,
                RecargarDatosAsync
            );

            // 4. Carga inicial de datos
            await RecargarDatosAsync();
        }

        protected abstract void ConfigurarNavegacion();
        protected abstract void ConfigurarVistaTickets();
        protected virtual Task PrepararFiltrosAsync() => Task.CompletedTask;
        public abstract Task RecargarDatosAsync();

        protected virtual void AlCambiarFiltros()
        {
            VistaTickets.ControladorPaginacion.ReiniciarAPrimeraPagina();
            _ = RecargarDatosAsync();
        }

        private void SuscribirRecargaAutomaticaDeFiltros()
        {
            VistaTickets.FiltroCambiado += (_, _) => AlCambiarFiltros();
            VistaTickets.LimpiarClic += (_, _) => AlCambiarFiltros();
            // El botón de recarga realiza una recarga completa desde el servidor.
            VistaTickets.RecargarClic += (_, _) =>
            {
                VistaTickets.ControladorPaginacion.ReiniciarAPrimeraPagina();
                _ = RecargarDatosAsync();
            };
        }
    }
}
