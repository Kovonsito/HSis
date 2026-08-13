using HSis.Data.Models;
using HSis.Logic.DTOs;

namespace HSis.UI.Presenters
{
    public interface IDashboardAdminView
    {
        void MostrarKPIs(ReporteKpisDto kpis);
        void MostrarTickets(List<TicketDto> tickets, int totalCount, int pageNumber, int pageSize);
        void MostrarCargando(bool cargando);
        void MostrarError(string mensaje);
        void CargarCombosFiltros(List<Usuario> tecnicos);
    }
}
