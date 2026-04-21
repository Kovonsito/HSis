using System;
using System.Collections.Generic;

namespace HSis.Data.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string? Nombre { get; set; }

    public int? IdDepartamento { get; set; }

    public int? IdPuesto { get; set; }

    public int? IdSucursal { get; set; }

    public int? IdRol { get; set; }

    public string? Contraseña { get; set; }

    public virtual ICollection<HistorialCambiosTicket> HistorialCambiosTickets { get; set; } = new List<HistorialCambiosTicket>();

    public virtual Departamento? IdDepartamentoNavigation { get; set; }

    public virtual Puesto? IdPuestoNavigation { get; set; }

    public virtual RolUsuario? IdRolNavigation { get; set; }

    public virtual Sucursal? IdSucursalNavigation { get; set; }

    public virtual ICollection<IngresosMaterial> IngresosMaterials { get; set; } = new List<IngresosMaterial>();

    public virtual ICollection<Ticket> TicketIdTecnicoNavigations { get; set; } = new List<Ticket>();

    public virtual ICollection<Ticket> TicketIdUsuarioNavigations { get; set; } = new List<Ticket>();
}
