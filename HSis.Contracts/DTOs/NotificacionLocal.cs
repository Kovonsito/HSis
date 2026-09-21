namespace HSis.Logic.Services
{
    public class NotificacionLocal
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int? DbId { get; set; }
        public int TicketId { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.Now;
        public bool Leido { get; set; }
    }
}
