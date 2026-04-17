using System;
using System.Collections.Generic;

namespace HSis.Data.Models;

public partial class Puesto
{
    public int IdPuesto { get; set; }

    public string? Nombre { get; set; }

    public string? Descripción { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
