using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HSis.Data.Models.Configurations;

public class RolUsuarioConfiguration : IEntityTypeConfiguration<RolUsuario>
{
    public void Configure(EntityTypeBuilder<RolUsuario> entity)
    {
        entity.HasKey(e => e.IdRol);
        entity.ToTable("Rol_Usuario");
        entity.Property(e => e.IdRol).HasColumnName("id_Rol");
        entity.Property(e => e.Descripción).HasMaxLength(50).IsUnicode(false);
    }
}
