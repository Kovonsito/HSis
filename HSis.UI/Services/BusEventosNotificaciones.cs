using HSis.Contracts.Services;
using HSis.Contracts.DTOs;
namespace HSis.UI.Services
{
    public sealed class BusEventosNotificaciones : IBusEventosNotificaciones
    {
        public event EventHandler<NotificacionEventArgs>? OnNotificacionPublicada;
        public event EventHandler<EstadoConexionEventArgs>? OnEstadoConexionCambiado;

        public void PublicarNotificacion(NotificacionDto notificacion)
        {
            ArgumentNullException.ThrowIfNull(notificacion);
            OnNotificacionPublicada?.Invoke(this, new NotificacionEventArgs(notificacion));
        }

        public void PublicarEstadoConexion(bool conectado, string? mensajeEstado = null)
        {
            OnEstadoConexionCambiado?.Invoke(this, new EstadoConexionEventArgs(conectado, mensajeEstado));
        }
    }
}
