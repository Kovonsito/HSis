namespace HSis.Contracts.Services
{
    public interface IBusEventosNotificaciones
    {
        event EventHandler<NotificacionEventArgs>? OnNotificacionPublicada;
        event EventHandler<EstadoConexionEventArgs>? OnEstadoConexionCambiado;

        void PublicarNotificacion(int ticketId, string tipo, string mensaje);
        void PublicarEstadoConexion(bool conectado, string? mensajeEstado = null);
    }
}
