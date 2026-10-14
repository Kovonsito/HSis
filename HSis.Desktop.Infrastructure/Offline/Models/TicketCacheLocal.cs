using System;

namespace HSis.Desktop.Infrastructure.Offline.Models;

public class TicketCacheLocal
{
    public int IdTicket { get; set; }
    public int IdUsuario { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string DepartamentoUsuario { get; set; } = string.Empty;
    public int? IdTecnico { get; set; }
    public string NombreTecnico { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Solucion { get; set; }
    public string? Estatus { get; set; }
    public string? Prioridad { get; set; }
    public DateTime? FechaAlta { get; set; }
    public DateTime? FechaAtencion { get; set; }
    public DateTime? FechaCierre { get; set; }
    public int? Calificacion { get; set; }
    public string? ComentarioEvaluacion { get; set; }
    public DateTime? FechaEvaluacion { get; set; }
    public bool EsOfflineLocal { get; set; }
    public DateTime UltimaActualizacion { get; set; } = DateTime.UtcNow;
}
