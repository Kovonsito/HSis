using HSis.Logic.DTOs;

namespace HSis.UI.Presenters
{
    public interface IDashboardTecnicoView
    {
        void MostrarTickets(List<TicketOperativoDto> tickets);
        void MostrarFeedbacks(List<FeedbackTecnicoDto> feedbacks);
        void MostrarIndicadores(int asignados, int disponibles, int cerrados, double promedioCalificacion);
        void MostrarCargando(bool cargando);
        void MostrarError(string mensaje);
    }
}

