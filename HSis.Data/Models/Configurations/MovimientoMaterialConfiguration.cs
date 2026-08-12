using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HSis.Data.Models.Configurations;

public class MovimientoMaterialConfiguration : IEntityTypeConfiguration<MovimientoMaterial>
{
    public void Configure(EntityTypeBuilder<MovimientoMaterial> entity)
    {
        entity.HasKey(e => e.IdMovimiento).HasName("PK_Movimientos_Material");
        entity.ToTable("Movimientos_Material", tb => tb.HasTrigger("TR_Material_ActualizarStock_Movimiento"));

        entity.Property(e => e.IdMovimiento).HasColumnName("id_Movimiento");
        entity.Property(e => e.CostoUnitario).HasColumnType("smallmoney").HasColumnName("Costo_Unitario");
        entity.Property(e => e.FechaMovimiento).HasColumnType("datetime").HasColumnName("Fecha_Movimiento");
        entity.Property(e => e.IdMaterial).HasColumnName("Id_Material");
        entity.Property(e => e.IdUsuario).HasColumnName("id_Usuario");
        entity.Property(e => e.Motivo).HasMaxLength(100).IsUnicode(false).HasColumnName("Motivo");

        entity.HasOne(d => d.Material).WithMany(p => p.MovimientosMateriales)
            .HasForeignKey(d => d.IdMaterial)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Ingreso_Material");

        entity.HasOne(d => d.Usuario).WithMany(p => p.MovimientosMateriales)
            .HasForeignKey(d => d.IdUsuario)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Ingreso_Usuario");
    }
}
