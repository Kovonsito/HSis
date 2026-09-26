#nullable enable
using System.Runtime.Versioning;
using HSis.Contracts.Constants;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.UI.Controls;
using HSis.UI.Factories;
using HSis.UI.Helpers;

namespace HSis.UI.Coordinators
{
    /// <summary>
    /// Coordinador especializado en dashboards con filtrado y paginación en memoria (Cliente y Técnico).
    /// Centraliza el filtrado LINQ por texto, fechas y estados para no repetirlo en los formularios.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public abstract class CoordinadorDashboardMemoria(
        Form formulario,
        SidebarControl sidebar,
        TopBarControl topBar,
        VistaTicketsDashboardControl vistaTickets,
        IAdministradorSesionUsuario contextoSesion,
        IAlmacenamientoCredencialesLocal sessionCache,
        IFabricaFormularios formFactory,
        IClienteSignalRNotificaciones notificationClient,
        INotificacionesApiClient notificacionesApiClient,
        IBusEventosNotificaciones eventBus) : CoordinadorDashboardBase(formulario, sidebar, topBar, vistaTickets, contextoSesion, sessionCache, formFactory, notificationClient, notificacionesApiClient, eventBus)
    {
        protected List<TicketDto> TodosLosTickets = [];

        protected override void AlCambiarFiltros()
        {
            if (EstadoCarga.EstaCargando)
            {
                return;
            }

            VistaTickets.ControladorPaginacion.ReiniciarAPrimeraPagina();
            MostrarPaginaActual();
        }

        public void MostrarPaginaActual()
        {
            var query = TodosLosTickets.AsEnumerable();

            // 1. Filtrado personalizado por vista o estado del rol
            query = AplicarFiltroPorEstado(query);

            // 2. Filtros comunes (búsqueda por texto en folio/descripción/técnico y rango de fechas)
            var (txt, dtInicio, dtFin, _, _) = VistaTickets.Filtro.ObtenerValoresFiltros().ExtraerFiltrosComunes();

            if (!string.IsNullOrEmpty(txt))
            {
                query = query.Where(t =>
                    t.Folio.ToString().Contains(txt) ||
                    (t.Descripcion?.ToLowerInvariant().Contains(txt) ?? false) ||
                    (t.TecnicoAsignado?.ToLowerInvariant().Contains(txt) ?? false));
            }

            if (dtInicio.HasValue) query = query.Where(t => t.FechaAlta >= dtInicio.Value);
            if (dtFin.HasValue) query = query.Where(t => t.FechaAlta <= dtFin.Value);

            // 3. Filtros adicionales específicos de cada rol
            query = AplicarFiltrosAdicionales(query);

            var ticketsFiltrados = query.ToList();

            // 4. Paginar y asignar al Grid
            var paginaTickets = VistaTickets.ControladorPaginacion.ObtenerPagina(ticketsFiltrados).ToList();
            VistaTickets.Grid.DataSource = new ListaVinculableOrdenable<TicketDto>(paginaTickets);

            // 5. Aplicar columnas del rol y actualizar conteo de página
            AplicarPerfilColumnas(VistaTickets.Grid);
            VistaTickets.ControladorPaginacion.Actualizar(ticketsFiltrados.Count);
        }

        protected virtual IEnumerable<TicketDto> AplicarFiltroPorEstado(IEnumerable<TicketDto> tickets) => tickets;
        protected virtual IEnumerable<TicketDto> AplicarFiltrosAdicionales(IEnumerable<TicketDto> tickets) => tickets;
        protected abstract void AplicarPerfilColumnas(DataGridView grid);
    }
}
