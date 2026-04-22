using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HSis.Data.Models.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> entity)
    {
        entity.HasKey(e => e.IdUsuario);
        entity.ToTable("Usuario");
        entity.Property(e => e.IdUsuario).HasColumnName("id_Usuario");
        entity.Property(e => e.Contraseña).HasMaxLength(255).IsUnicode(false);
        entity.Property(e => e.IdDepartamento).HasColumnName("id_Departamento");
        entity.Property(e => e.IdPuesto).HasColumnName("id_Puesto");
        entity.Property(e => e.IdRol).HasColumnName("id_Rol");
        entity.Property(e => e.IdSucursal).HasColumnName("id_Sucursal");
        entity.Property(e => e.Nombre).HasMaxLength(50).IsUnicode(false);

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
    }
}
