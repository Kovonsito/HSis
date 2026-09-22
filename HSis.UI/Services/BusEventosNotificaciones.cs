using HSis.Contracts.Services;
using HSis.Contracts.DTOs;
namespace HSis.UI.Services
{
    public class BusEventosNotificaciones : IBusEventosNotificaciones
    {
        public event EventHandler<NotificacionEventArgs>? OnNotificacionPublicada;
        public event EventHandler<EstadoConexionEventArgs>? OnEstadoConexionCambiado;

        public void PublicarNotificacion(int ticketId, string tipo, string mensaje)
        {
            OnNotificacionPublicada?.Invoke(this, new NotificacionEventArgs(ticketId, tipo, mensaje));
        }

        public void PublicarEstadoConexion(bool conectado, string? mensajeEstado = null)
        {
            OnEstadoConexionCambiado?.Invoke(this, new EstadoConexionEventArgs(conectado, mensajeEstado));
        }
    }
}
