using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HSis.Data.Models.Configurations;

public class VHistorialInventarioConfiguration : IEntityTypeConfiguration<VHistorialInventario>
{
    public void Configure(EntityTypeBuilder<VHistorialInventario> entity)
    {
        entity.HasNoKey().ToView("v_Historial_Inventario");
        entity.Property(e => e.CostoUnitario).HasColumnType("smallmoney").HasColumnName("Costo_Unitario");
        entity.Property(e => e.Fecha).HasColumnType("datetime");
        entity.Property(e => e.FolioTicket).HasColumnName("Folio_Ticket");
        entity.Property(e => e.IdMaterial).HasColumnName("Id_Material");
        entity.Property(e => e.IdMovimientoUnico).HasColumnName("Id_Movimiento_Unico");
        entity.Property(e => e.Material).HasMaxLength(50).IsUnicode(false);
        entity.Property(e => e.TipoMovimiento).HasMaxLength(7).IsUnicode(false).HasColumnName("Tipo_Movimiento");
        entity.Property(e => e.UsuarioResponsable).HasMaxLength(50).IsUnicode(false).HasColumnName("Usuario_Responsable");
        entity.Property(e => e.ValorTotalMovimiento).HasColumnType("smallmoney").HasColumnName("Valor_Total_Movimiento");
    }
}
