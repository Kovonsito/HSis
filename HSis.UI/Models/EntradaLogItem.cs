#nullable enable
using System;

namespace HSis.UI.Models;

/// <summary>
/// Representa una entrada estructurada de evento de log en el visor de sucesos.
/// </summary>
public sealed class EntradaLogItem
{
    public DateTime FechaHora { get; set; }

    public string FechaHoraTexto => FechaHora.ToString("yyyy-MM-dd HH:mm:ss.fff");

    public string Nivel { get; set; } = "Información";

    public string? TraceId { get; set; }

    public string MensajeCorto { get; set; } = string.Empty;

    public string DetalleCompleto { get; set; } = string.Empty;

    public string ArchivoOrigen { get; set; } = string.Empty;

    public bool EsError => string.Equals(Nivel, "Error", StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(Nivel, "Fatal", StringComparison.OrdinalIgnoreCase);

    public bool EsAdvertencia => string.Equals(Nivel, "Advertencia", StringComparison.OrdinalIgnoreCase);
}
