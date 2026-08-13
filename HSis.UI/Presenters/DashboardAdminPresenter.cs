using HSis.Data.Models;
using HSis.Logic.DTOs;
using HSis.Logic.Services;

namespace HSis.UI.Presenters
{
    public class DashboardAdminPresenter(
        ITicketQueryService queryService,
        ITicketKpiService kpiService,
        ICatalogoService catalogoService)
    {
        private IDashboardAdminView? _view;

        public void SetView(IDashboardAdminView view)
        {
            _view = view;
        }


        public async Task CargarKPIsAsync(DateTime inicio, DateTime fin)
        {
            if (_view == null) return;
            try
            {
                var kpis = await kpiService.ObtenerReporteKpisAsync(inicio, fin);
                _view.MostrarKPIs(kpis);
            }
            catch (Exception ex)
            {
                _view.MostrarError($"Error al cargar KPIs: {ex.Message}");
            }
        }

        public async Task CargarCombosFiltrosAsync()
        {
            if (_view == null) return;
            try
            {
                var todosUsuarios = await catalogoService.ObtenerTodosAsync<Usuario>();
                var tecnicos = todosUsuarios.Where(u => u.IdRol == 2 || u.IdRol == 1).ToList();
                _view.CargarCombosFiltros(tecnicos);
            }
            catch (Exception ex)
            {
                _view.MostrarError($"Error al cargar lista de técnicos: {ex.Message}");
            }
        }

        public async Task CargarGridAsync(TicketFilterDto filtros, int pageNumber, int pageSize)
        {
            if (_view == null) return;
            try
            {
                _view.MostrarCargando(true);
                var paginado = await queryService.ObtenerTicketsFiltradosPaginadosAsync(filtros, pageNumber, pageSize);
                _view.MostrarTickets(paginado.Items, paginado.TotalCount, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                _view.MostrarError($"Error al cargar listado de tickets: {ex.Message}");
            }
            finally
            {
                _view.MostrarCargando(false);
            }
        }
    }
}
