using HSis.Data.Models;
using HSis.Logic.DTOs;

namespace HSis.UI.Presenters
{
    public interface ITicketDetalleView
    {
        void MostrarTicket(TicketDto ticket);
        void CargarTecnicos(List<Usuario> tecnicos, int? idTecnicoActual, bool esAdmin);
        void CargarEstatusPermitidos(List<string> estatusPermitidos, string estatusActual);
        void CargarHistorial(List<HistorialCambiosDto> historial);
        void CargarDetallesMaterial(List<TicketDetalleDto> detalles);
        void MostrarError(string mensaje);
        void MostrarExito(string mensaje);
        void CerrarFormulario();
        void MostrarCargando(bool cargando);
    }
}
