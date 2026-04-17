using System;
using System.Collections.Generic;

namespace HSis.Data.Models;

public partial class Empresa
{
    public int IdEmpresa { get; set; }

    public string? Nombre { get; set; }

    public string? Calle { get; set; }

    public string? Número { get; set; }

    public string? Colonia { get; set; }

    public string? Telefono { get; set; }

    public virtual ICollection<Sucursal> Sucursals { get; set; } = new List<Sucursal>();
}
