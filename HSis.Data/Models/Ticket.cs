namespace HSis.Data.Models;

public partial class Ticket
{
    public int IdTicket { get; set; }

    public int IdUsuario { get; set; }

    public DateTime? FechaAlta { get; set; }

    public DateTime? FechaCierre { get; set; }

    public DateTime? FechaAtencion { get; set; }

    public string? Estatus { get; set; }

    public string? Descripcion { get; set; }

    public string? Solucion { get; set; }

    public int? IdTecnico { get; set; }

    public string? Prioridad { get; set; }

    public virtual ICollection<DetTicket> DetTickets { get; set; } = [];

    public virtual ICollection<HistorialCambiosTicket> HistorialCambiosTickets { get; set; } = [];

    public virtual Usuario? Tecnico { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;

    public int? Calificacion { get; set; }

    public string? ComentarioEvaluacion { get; set; }

    public DateTime? FechaEvaluacion { get; set; }
}
