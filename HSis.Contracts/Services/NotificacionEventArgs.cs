namespace HSis.Logic.Services
{
    public class NotificacionEventArgs(int ticketId, string tipo, string mensaje) : EventArgs
    {
        public int TicketId { get; } = ticketId;
        public string Tipo { get; } = tipo;
        public string Mensaje { get; } = mensaje;
        public DateTime Fecha { get; } = DateTime.Now;
    }

    public class EstadoConexionEventArgs(bool conectado, string? mensajeEstado = null) : EventArgs
    {
        public bool Conectado { get; } = conectado;
        public string? MensajeEstado { get; } = mensajeEstado;
    }
}

