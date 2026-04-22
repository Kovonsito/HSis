using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HSis.Data.Models.Configurations;

public class EmpresaConfiguration : IEntityTypeConfiguration<Empresa>
{
    public void Configure(EntityTypeBuilder<Empresa> entity)
    {
        entity.HasKey(e => e.IdEmpresa);
        entity.ToTable("Empresa");
        entity.Property(e => e.IdEmpresa).HasColumnName("id_Empresa");
        entity.Property(e => e.Calle).HasMaxLength(50).IsUnicode(false);
        entity.Property(e => e.Colonia).HasMaxLength(50).IsUnicode(false);
        entity.Property(e => e.Nombre).HasMaxLength(50).IsUnicode(false);
        entity.Property(e => e.Número).HasMaxLength(10).IsUnicode(false);
        entity.Property(e => e.Telefono).HasMaxLength(15).IsUnicode(false);
    }
}
