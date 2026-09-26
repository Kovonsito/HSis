using HSis.Contracts.DTOs;

namespace HSis.Contracts.Services
{
    public sealed class NotificacionEventArgs(NotificacionDto notificacion) : EventArgs
    {
        public NotificacionDto Notificacion { get; } = notificacion;
        public int IdNotificacion => Notificacion.IdNotificacion;
        public int? TicketId => Notificacion.TicketId;
        public int? MaterialId => Notificacion.MaterialId;
        public string Tipo => Notificacion.Tipo;
        public string Mensaje => Notificacion.Mensaje;
        public DateTimeOffset Fecha => Notificacion.FechaCreacion;
    }

    public sealed class EstadoConexionEventArgs(bool conectado, string? mensajeEstado = null) : EventArgs
    {
        public bool Conectado { get; } = conectado;
        public string? MensajeEstado { get; } = mensajeEstado;
    }
}

