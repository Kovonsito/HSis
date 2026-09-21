namespace HSis.Logic.Services
{
    public class NotificationEventBus : INotificationEventBus
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

