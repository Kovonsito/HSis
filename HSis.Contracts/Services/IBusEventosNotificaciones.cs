using HSis.Contracts.DTOs;

namespace HSis.Contracts.Services
{
    public interface IBusEventosNotificaciones
    {
        event EventHandler<NotificacionEventArgs>? OnNotificacionPublicada;
        event EventHandler<EstadoConexionEventArgs>? OnEstadoConexionCambiado;

        void PublicarNotificacion(NotificacionDto notificacion);
        void PublicarEstadoConexion(bool conectado, string? mensajeEstado = null);
    }
}
