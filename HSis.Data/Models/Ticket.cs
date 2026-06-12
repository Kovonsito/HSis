namespace HSis.Data.Models;

public partial class Ticket
{
    public int IdTicket { get; set; }

    public int IdUsuario { get; set; }

    public DateTime? Alta { get; set; }

    public DateTime? Cierre { get; set; }

    public DateTime? Atención { get; set; }

    public string? Status { get; set; }

    public string? Descripción { get; set; }

    public string? Solución { get; set; }

    public int? IdTecnico { get; set; }

    public string? Prioridad { get; set; }

    public virtual ICollection<DetTicket> DetTickets { get; set; } = [];

    public virtual ICollection<HistorialCambiosTicket> HistorialCambiosTickets { get; set; } = [];

    public virtual Usuario? IdTecnicoNavigation { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public int? Calificacion { get; set; }

    public string? ComentarioFeedback { get; set; }

    public DateTime? FechaFeedback { get; set; }
}
