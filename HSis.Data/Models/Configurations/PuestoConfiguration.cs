using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HSis.Data.Models.Configurations;

public class PuestoConfiguration : IEntityTypeConfiguration<Puesto>
{
    public void Configure(EntityTypeBuilder<Puesto> entity)
    {
        entity.HasKey(e => e.IdPuesto);
        entity.ToTable("Puesto");
        entity.Property(e => e.IdPuesto).HasColumnName("id_Puesto");
        entity.Property(e => e.Descripcion).HasMaxLength(250).IsUnicode(false).HasColumnName("Descripcion");
        entity.Property(e => e.Nombre).HasMaxLength(20).IsUnicode(false);
    }
}
