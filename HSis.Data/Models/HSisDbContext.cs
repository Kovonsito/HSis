using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HSis.Data.Models;

public partial class HSisDbContext : DbContext
{
    public HSisDbContext()
    {
    }

    public HSisDbContext(DbContextOptions<HSisDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Departamento> Departamentos { get; set; }

    public virtual DbSet<DetTicket> DetTickets { get; set; }

    public virtual DbSet<Empresa> Empresas { get; set; }

    public virtual DbSet<HistorialCambiosTicket> HistorialCambiosTickets { get; set; }

    public virtual DbSet<Ingreso> Ingresos { get; set; }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<Puesto> Puestos { get; set; }

    public virtual DbSet<RolUsuario> RolUsuarios { get; set; }

    public virtual DbSet<Sucursal> Sucursals { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=EJIPC-SISTEMA03\\SQLEXPRESS;Initial Catalog=HSIS;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Departamento>(entity =>
        {
            entity.HasKey(e => e.IdDepartamento);

            entity.ToTable("Departamento");

            entity.Property(e => e.IdDepartamento).HasColumnName("id_Departamento");
            entity.Property(e => e.Descripción)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DetTicket>(entity =>
        {
            entity.HasKey(e => new { e.IdTicket, e.IdMaterial });

            entity.ToTable("Det_Ticket");

            entity.Property(e => e.IdTicket).HasColumnName("id_Ticket");
            entity.Property(e => e.IdMaterial).HasColumnName("id_Material");
            entity.Property(e => e.Cantidad).HasDefaultValue(1, "DF__Det_Ticke__Canti__6C190EBB");
            entity.Property(e => e.CostoUnitarioAplicado)
                .HasColumnType("smallmoney")
                .HasColumnName("Costo_Unitario_Aplicado");

            entity.HasOne(d => d.IdMaterialNavigation).WithMany(p => p.DetTickets)
                .HasForeignKey(d => d.IdMaterial)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Det_Ticket_Material");

            entity.HasOne(d => d.IdTicketNavigation).WithMany(p => p.DetTickets)
                .HasForeignKey(d => d.IdTicket)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Det_Ticket_Ticket");
        });

        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.HasKey(e => e.IdEmpresa);

            entity.ToTable("Empresa");

            entity.Property(e => e.IdEmpresa).HasColumnName("id_Empresa");
            entity.Property(e => e.Calle)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Colonia)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Número)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        modelBuilder.Entity<HistorialCambiosTicket>(entity =>
        {
            entity.HasKey(e => e.IdHistorial).HasName("PK_Historial_Ticket");

            entity.ToTable("Historial_Cambios_Ticket");

            entity.Property(e => e.IdHistorial).HasColumnName("id_Historial");
            entity.Property(e => e.CampoModificado)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Campo_Modificado");
            entity.Property(e => e.FechaMovimiento)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Fecha_Movimiento");
            entity.Property(e => e.IdTicket).HasColumnName("id_Ticket");
            entity.Property(e => e.IdUsuarioCambio).HasColumnName("id_Usuario_Cambio");
            entity.Property(e => e.ValorAnterior)
                .IsUnicode(false)
                .HasColumnName("Valor_Anterior");
            entity.Property(e => e.ValorNuevo)
                .IsUnicode(false)
                .HasColumnName("Valor_Nuevo");

            entity.HasOne(d => d.IdTicketNavigation).WithMany(p => p.HistorialCambiosTickets)
                .HasForeignKey(d => d.IdTicket)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Historial_Ticket_Ticket");

            entity.HasOne(d => d.IdUsuarioCambioNavigation).WithMany(p => p.HistorialCambiosTickets)
                .HasForeignKey(d => d.IdUsuarioCambio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Historial_Usuario");
        });

        modelBuilder.Entity<Ingreso>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.IdIngreso)
                .ValueGeneratedOnAdd()
                .HasColumnName("id_Ingreso");
            entity.Property(e => e.IdMaterial).HasColumnName("Id_Material");
            entity.Property(e => e.PenultimoIngreso)
                .HasColumnType("datetime")
                .HasColumnName("Penultimo_Ingreso");
            entity.Property(e => e.UltimoIngreso)
                .HasColumnType("datetime")
                .HasColumnName("Ultimo_Ingreso");

            entity.HasOne(d => d.IdMaterialNavigation).WithMany()
                .HasForeignKey(d => d.IdMaterial)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ingresos_Material");
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.IdMaterial);

            entity.ToTable("Material");

            entity.Property(e => e.IdMaterial).HasColumnName("id_Material");
            entity.Property(e => e.Costo).HasColumnType("smallmoney");
            entity.Property(e => e.CostoAnterior)
                .HasDefaultValue(0m, "DF_Material_Costo_Anterior")
                .HasColumnType("smallmoney")
                .HasColumnName("Costo_Anterior");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Puesto>(entity =>
        {
            entity.HasKey(e => e.IdPuesto);

            entity.ToTable("Puesto");

            entity.Property(e => e.IdPuesto).HasColumnName("id_Puesto");
            entity.Property(e => e.Descripción)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<RolUsuario>(entity =>
        {
            entity.HasKey(e => e.IdRol);

            entity.ToTable("Rol_Usuario");

            entity.Property(e => e.IdRol).HasColumnName("id_Rol");
            entity.Property(e => e.Descripción)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Sucursal>(entity =>
        {
            entity.HasKey(e => e.IdSucursal);

            entity.ToTable("Sucursal");

            entity.Property(e => e.IdSucursal).HasColumnName("id_Sucursal");
            entity.Property(e => e.Calle)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Colonia)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IdEmpresa).HasColumnName("id_Empresa");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Número)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.Sucursals)
                .HasForeignKey(d => d.IdEmpresa)
                .HasConstraintName("FK_Sucursal_Empresa");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.IdTicket).HasName("PK__Ticket__81109ADA1E0C898B");

            entity.ToTable("Ticket");

            entity.Property(e => e.IdTicket).HasColumnName("id_Ticket");
            entity.Property(e => e.Alta)
                .HasDefaultValueSql("(getdate())", "DF__Ticket__Alta__6FE99F9F")
                .HasColumnType("datetime");
            entity.Property(e => e.Atención).HasColumnType("datetime");
            entity.Property(e => e.Cierre).HasColumnType("datetime");
            entity.Property(e => e.Descripción)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.IdTecnico).HasColumnName("id_Tecnico");
            entity.Property(e => e.IdUsuario).HasColumnName("id_Usuario");
            entity.Property(e => e.Solución).IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValue("Abierto", "DF__Ticket__Status__70DDC3D8");

            entity.HasOne(d => d.IdTecnicoNavigation).WithMany(p => p.TicketIdTecnicoNavigations)
                .HasForeignKey(d => d.IdTecnico)
                .HasConstraintName("FK_Ticket_Tecnico");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.TicketIdUsuarioNavigations)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_Usuario");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario);

            entity.ToTable("Usuario");

            entity.Property(e => e.IdUsuario).HasColumnName("id_Usuario");
            entity.Property(e => e.Contraseña)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.IdDepartamento).HasColumnName("id_Departamento");
            entity.Property(e => e.IdPuesto).HasColumnName("id_Puesto");
            entity.Property(e => e.IdRol).HasColumnName("id_Rol");
            entity.Property(e => e.IdSucursal).HasColumnName("id_Sucursal");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdDepartamentoNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdDepartamento)
                .HasConstraintName("FK_Usuario_Departamento");

            entity.HasOne(d => d.IdPuestoNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdPuesto)
                .HasConstraintName("FK_Usuario_Puesto");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK_Usuario_Rol_Usuario");

            entity.HasOne(d => d.IdSucursalNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdSucursal)
                .HasConstraintName("FK_Usuario_Sucursal");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
