namespace HSis.UI.Models.Notificaciones;

public sealed class NotificacionLocal
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int? DbId { get; set; }
    public int? TicketId { get; set; }
    public int? MaterialId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.Now;
    public bool Leido { get; set; }
}
