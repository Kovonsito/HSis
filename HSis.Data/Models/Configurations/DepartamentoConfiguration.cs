using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HSis.Data.Models.Configurations
{
    public class DepartamentoConfiguration : IEntityTypeConfiguration<Departamento>
    {
        public void Configure(EntityTypeBuilder<Departamento> entity)
        {
            entity.HasKey(e => e.IdDepartamento);
            entity.ToTable("Departamento");
            entity.Property(e => e.IdDepartamento).HasColumnName("id_Departamento");
            entity.Property(e => e.Descripcion).HasMaxLength(250).IsUnicode(false).HasColumnName("Descripcion");
            entity.Property(e => e.Nombre).HasMaxLength(30).IsUnicode(false);
        }
    }
}

