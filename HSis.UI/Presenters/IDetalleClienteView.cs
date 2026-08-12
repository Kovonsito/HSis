using HSis.Logic.DTOs;

namespace HSis.UI.Presenters
{
    public interface IDetalleClienteView
    {
        void MostrarTicket(TicketDto ticket);
        void MostrarError(string mensaje);
        void MostrarExito(string mensaje);
        void CerrarFormulario();
        void MostrarCargando(bool cargando);
    }
}
