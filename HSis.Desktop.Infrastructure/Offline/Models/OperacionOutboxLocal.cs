using System;

namespace HSis.Desktop.Infrastructure.Offline.Models;

public enum EstadoOperacionOutbox
{
    Pendiente = 0,
    EnProceso = 1,
    Sincronizado = 2,
    Fallido = 3
}

public class OperacionOutboxLocal
{
    public Guid IdOperacion { get; set; } = Guid.NewGuid();
    public string TipoOperacion { get; set; } = string.Empty;
    public string RutaEndpoint { get; set; } = string.Empty;
    public string MetodoHttp { get; set; } = "POST";
    public string PayloadJson { get; set; } = string.Empty;
    public string? EntidadTempId { get; set; }
    public int? EntidadIdReal { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public int Intentos { get; set; }
    public EstadoOperacionOutbox Estado { get; set; } = EstadoOperacionOutbox.Pendiente;
    public string? UltimoError { get; set; }
}
