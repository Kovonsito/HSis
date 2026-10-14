using System;

namespace HSis.Contracts.DTOs;

public class OperacionOutboxDto
{
    public Guid IdOperacion { get; set; }
    public string TipoOperacion { get; set; } = string.Empty;
    public string RutaEndpoint { get; set; } = string.Empty;
    public string MetodoHttp { get; set; } = "POST";
    public string PayloadJson { get; set; } = string.Empty;
    public string? EntidadTempId { get; set; }
    public int? EntidadIdReal { get; set; }
    public DateTime FechaCreacion { get; set; }
    public int Intentos { get; set; }
    public int Estado { get; set; }
    public string? UltimoError { get; set; }
}
