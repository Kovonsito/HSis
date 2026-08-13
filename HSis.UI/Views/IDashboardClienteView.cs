using HSis.Logic.DTOs;

namespace HSis.UI.Presenters
{
    public interface IDashboardClienteView
    {
        void MostrarTickets(List<TicketClienteDto> tickets);
        void MostrarIndicadores(int activos, int cerrados);
        void MostrarCargando(bool cargando);
        void MostrarError(string mensaje);
    }
}
