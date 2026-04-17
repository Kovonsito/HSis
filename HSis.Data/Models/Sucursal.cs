using System;
using System.Collections.Generic;

namespace HSis.Data.Models;

public partial class Sucursal
{
    public int IdSucursal { get; set; }

    public string? Nombre { get; set; }

    public string? Calle { get; set; }

    public string? Número { get; set; }

    public string? Colonia { get; set; }

    public string? Telefono { get; set; }

    public int? IdEmpresa { get; set; }

    public virtual Empresa? IdEmpresaNavigation { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
