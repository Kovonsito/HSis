using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HSis.Data.Models.Configurations
{
    public class SucursalConfiguration : IEntityTypeConfiguration<Sucursal>
    {
        public void Configure(EntityTypeBuilder<Sucursal> entity)
        {
            entity.HasKey(e => e.IdSucursal);
            entity.ToTable("Sucursal");
            entity.Property(e => e.IdSucursal).HasColumnName("id_Sucursal");
            entity.Property(e => e.Calle).HasMaxLength(30).IsUnicode(false);
            entity.Property(e => e.Colonia).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.IdEmpresa).HasColumnName("id_Empresa");
            entity.Property(e => e.Nombre).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.Numero).HasMaxLength(10).IsUnicode(false).HasColumnName("Numero");
            entity.Property(e => e.Telefono).HasMaxLength(15).IsUnicode(false);

            entity.HasOne(d => d.Empresa).WithMany(p => p.Sucursales)
                .HasForeignKey(d => d.IdEmpresa)
                .HasConstraintName("FK_Sucursal_Empresa");
        }
    }
}

