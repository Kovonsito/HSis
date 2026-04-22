using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HSis.Data.Models.Configurations;

public class IngresosMaterialConfiguration : IEntityTypeConfiguration<IngresosMaterial>
{
    public void Configure(EntityTypeBuilder<IngresosMaterial> entity)
    {
        entity.HasKey(e => e.IdIngreso).HasName("PK_Ingreso");
        entity.ToTable("Ingresos_Material", tb => tb.HasTrigger("TR_Material_ActualizarStock_Ingreso"));
        entity.Property(e => e.IdIngreso).HasColumnName("id_Ingreso");
        entity.Property(e => e.CostoCompra).HasColumnType("smallmoney").HasColumnName("Costo_Compra");
        entity.Property(e => e.FechaIngreso).HasDefaultValueSql("(getdate())", "DF_Ingreso_Fecha").HasColumnType("datetime").HasColumnName("Fecha_Ingreso");
        entity.Property(e => e.IdMaterial).HasColumnName("Id_Material");
        entity.Property(e => e.IdUsuario).HasColumnName("id_Usuario");

        entity.HasOne(d => d.IdMaterialNavigation).WithMany(p => p.IngresosMaterials)
            .HasForeignKey(d => d.IdMaterial)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Ingreso_Material");

        entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.IngresosMaterials)
            .HasForeignKey(d => d.IdUsuario)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Ingreso_Usuario");
    }
}
