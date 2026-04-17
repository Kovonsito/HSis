using System;
using System.Collections.Generic;

namespace HSis.Data.Models;

public partial class Ingreso
{
    public int IdIngreso { get; set; }

    public DateTime? UltimoIngreso { get; set; }

    public DateTime? PenultimoIngreso { get; set; }

    public int IdMaterial { get; set; }

    public virtual Material IdMaterialNavigation { get; set; } = null!;
}
