using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HSis.Data.Models.Configurations;

public class DepartamentoConfiguration : IEntityTypeConfiguration<Departamento>
{
    public void Configure(EntityTypeBuilder<Departamento> entity)
    {
        entity.HasKey(e => e.IdDepartamento);
        entity.ToTable("Departamento");
        entity.Property(e => e.IdDepartamento).HasColumnName("id_Departamento");
        entity.Property(e => e.Descripción).HasMaxLength(250).IsUnicode(false);
        entity.Property(e => e.Nombre).HasMaxLength(30).IsUnicode(false);
    }
}
