using HSis.Logic.Services;

namespace HSis.UI.Presenters
{
    public interface INotificacionesView
    {
        void ActualizarInsigniaCampana(int noLeidas);
        void MostrarNotificaciones(IEnumerable<NotificacionLocal> notificaciones);
        void ActualizarEstadoConexion(bool conectado, string mensaje, System.Drawing.Color colorFondo);
        Task RecargarDatosHostAsync();
        void AbrirDetalleTicket(int ticketId);
    }
}

