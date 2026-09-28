using Microsoft.EntityFrameworkCore;

namespace HSis.Data.Models
{
    public partial class HSisDbContext : DbContext
    {
        public HSisDbContext(DbContextOptions<HSisDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Departamento> Departamentos { get; set; } = null!;

        public virtual DbSet<DetTicket> DetTickets { get; set; } = null!;

        public virtual DbSet<Empresa> Empresas { get; set; } = null!;

        public virtual DbSet<HistorialCambiosTicket> HistorialCambiosTickets { get; set; } = null!;

        public virtual DbSet<MovimientoMaterial> MovimientosMateriales { get; set; } = null!;

        public virtual DbSet<Material> Materials { get; set; } = null!;

        public virtual DbSet<Puesto> Puestos { get; set; } = null!;

        public virtual DbSet<RolUsuario> RolesUsuarios { get; set; } = null!;

        public virtual DbSet<Sucursal> Sucursales { get; set; } = null!;

        public virtual DbSet<Ticket> Tickets { get; set; } = null!;

        public virtual DbSet<Usuario> Usuarios { get; set; } = null!;

        public virtual DbSet<Notificacion> Notificaciones { get; set; } = null!;

        public virtual DbSet<VHistorialInventario> VHistorialInventarios { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(HSisDbContext).Assembly);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
