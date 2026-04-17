using System;
using System.Collections.Generic;

namespace HSis.Data.Models;

public partial class Material
{
    public int IdMaterial { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal Costo { get; set; }

    public short Inventario { get; set; }

    public decimal? CostoAnterior { get; set; }

    public virtual ICollection<DetTicket> DetTickets { get; set; } = new List<DetTicket>();
}
