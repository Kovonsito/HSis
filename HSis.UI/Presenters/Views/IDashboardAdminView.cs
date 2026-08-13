using HSis.Logic.DTOs;

namespace HSis.UI.Presenters
{
    public interface IDashboardAdminView
    {
        void MostrarKPIs(int nuevos, int urgentes, int enProceso, int cerrados, int reabiertos, double calificacion);
        void MostrarTickets(List<TicketDto> tickets, int totalCount);
        void CargarCombosFiltros(List<UsuarioDto> admins, List<UsuarioDto> tecnicos);
        void MostrarCargando(bool cargando);
        void MostrarError(string mensaje);
        void MostrarInformacion(string mensaje, string titulo);
    }
}

