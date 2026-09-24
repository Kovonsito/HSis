#nullable enable
using System.Runtime.Versioning;
using HSis.Contracts.Services;
using HSis.UI.Controls;

namespace HSis.UI.Helpers
{
    /// <summary>
    /// Centraliza el ensamblado sidebar + topBar repetido en los 3 dashboards.
    /// Encapsula: ConfigurarSesion, ConfigurarItems, ItemSeleccionado, ConfigurarMenuHamburguesa.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static class ConfiguradorSidebarDashboard
    {
        /// <summary>
        /// Conecta el sidebar y el topBar de un dashboard con la sesión, los ítems de navegación
        /// y el callback de selección. Reemplaza los ~5 pasos manuales repetidos en cada dashboard.
        /// </summary>
        /// <param name="sidebar">Control de barra lateral del formulario.</param>
        /// <param name="topBar">Control de barra superior del formulario.</param>
        /// <param name="sessionCache">Caché de credenciales para datos de sesión.</param>
        /// <param name="contextoSesion">Contexto de usuario autenticado.</param>
        /// <param name="items">Ítems de navegación del sidebar.</param>
        /// <param name="claveDefault">Clave del ítem seleccionado por defecto.</param>
        /// <param name="alSeleccionar">Acción a ejecutar cuando el usuario selecciona un ítem.
        /// Recibe la clave del ítem. La lógica de negocio (qué vista cargar) es responsabilidad del dashboard.</param>
        public static void Configurar(
            SidebarControl sidebar,
            TopBarControl topBar,
            IAlmacenamientoCredencialesLocal sessionCache,
            IAdministradorSesionUsuario contextoSesion,
            ItemSidebar[] items,
            string claveDefault,
            Action<string> alSeleccionar)
        {
            // 1. Sesión y lista de ítems
            sidebar.ConfigurarSesion(sessionCache);
            sidebar.ConfigurarItems(items, claveDefault);

            // 2. Evento de selección desde el sidebar
            sidebar.ItemSeleccionado += (_, clave) => alSeleccionar(clave);

            // 3. Sesión del topBar + menú hamburguesa sincronizado con el sidebar
            topBar.ConfigurarSesion(sessionCache, contextoSesion);
            topBar.ConfigurarMenuHamburguesa(
                items,
                claveDefault,
                alSeleccionar,
                () => sidebar.Colapsado = !sidebar.Colapsado,
                () => !sidebar.Colapsado
            );
        }
    }
}
