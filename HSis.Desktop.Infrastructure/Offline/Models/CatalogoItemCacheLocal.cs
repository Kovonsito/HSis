using System;

namespace HSis.Desktop.Infrastructure.Offline.Models;

public class CatalogoItemCacheLocal
{
    public int Id { get; set; }
    public string TipoCatalogo { get; set; } = string.Empty;
    public int IdItem { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? DatosJson { get; set; }
    public DateTime UltimaActualizacion { get; set; } = DateTime.UtcNow;
}
