using System;

namespace HSis.Data.Models;

public partial class OperacionIdempotente
{
    public Guid IdOperacion { get; set; }

    public int IdUsuario { get; set; }

    public string RutaEndpoint { get; set; } = string.Empty;

    public string MetodoHttp { get; set; } = string.Empty;

    public int CodigoEstadoHttp { get; set; }

    public string? RespuestaJson { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
