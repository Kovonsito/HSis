using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HSis.Data.Models.Configurations;

public class MaterialConfiguration : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> entity)
    {
        entity.HasKey(e => e.IdMaterial);
        entity.ToTable("Material");
        entity.Property(e => e.IdMaterial).HasColumnName("id_Material");
        entity.Property(e => e.Costo).HasColumnType("smallmoney");
        entity.Property(e => e.Nombre).HasMaxLength(50).IsUnicode(false);
        entity.Property(e => e.UnidadMedida).HasMaxLength(20).IsUnicode(false).HasDefaultValue("Pieza", "DF_Material_Unidad").HasColumnName("Unidad_Medida");
    }
}
