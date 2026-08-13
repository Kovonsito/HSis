namespace HSis.Data.Models
{
    public partial class Usuario
    {
        public int IdUsuario { get; set; }

        public string? Nombre { get; set; }

        public int? IdDepartamento { get; set; }

        public int? IdPuesto { get; set; }

        public int? IdSucursal { get; set; }

        public int? IdRol { get; set; }

        public string? Contraseña { get; set; }

        public virtual ICollection<HistorialCambiosTicket> HistorialCambiosTickets { get; set; } = [];

        public virtual Departamento? Departamento { get; set; }

        public virtual Puesto? Puesto { get; set; }

        public virtual RolUsuario? Rol { get; set; }

        public virtual Sucursal? Sucursal { get; set; }

        public virtual ICollection<MovimientoMaterial> MovimientosMateriales { get; set; } = [];

        public virtual ICollection<Ticket> TicketsComoTecnico { get; set; } = [];

        public virtual ICollection<Ticket> TicketsComoUsuario { get; set; } = [];
    }
}
