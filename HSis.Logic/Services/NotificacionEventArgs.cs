namespace HSis.Logic.Services
{
    public class NotificacionEventArgs : EventArgs
    {
        public int TicketId { get; }
        public string Tipo { get; }
        public string Mensaje { get; }
        public DateTime Fecha { get; }

        public NotificacionEventArgs(int ticketId, string tipo, string mensaje)
        {
            TicketId = ticketId;
            Tipo = tipo;
            Mensaje = mensaje;
            Fecha = DateTime.Now;
        }
    }

    public class EstadoConexionEventArgs : EventArgs
    {
        public bool Conectado { get; }
        public string? MensajeEstado { get; }

        public EstadoConexionEventArgs(bool conectado, string? mensajeEstado = null)
        {
            Conectado = conectado;
            MensajeEstado = mensajeEstado;
        }
    }
}
