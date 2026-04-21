using System;
using System.Collections.Generic;

namespace HSis.Data.Models;

public partial class VHistorialInventario
{
    public Guid? IdMovimientoUnico { get; set; }

    public int IdMaterial { get; set; }

    public string Material { get; set; } = null!;

    public DateTime? Fecha { get; set; }

    public string TipoMovimiento { get; set; } = null!;

    public int? Cantidad { get; set; }

    public decimal CostoUnitario { get; set; }

    public decimal? ValorTotalMovimiento { get; set; }

    public string? UsuarioResponsable { get; set; }

    public int? FolioTicket { get; set; }
}
