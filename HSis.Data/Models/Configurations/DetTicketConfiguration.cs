using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HSis.Data.Models.Configurations;

public class DetTicketConfiguration : IEntityTypeConfiguration<DetTicket>
{
    public void Configure(EntityTypeBuilder<DetTicket> entity)
    {
        entity.HasKey(e => new { e.IdTicket, e.IdMaterial });
        entity.ToTable("Det_Ticket", tb => tb.HasTrigger("TR_Material_ActualizarStock_Egreso"));
        entity.Property(e => e.IdTicket).HasColumnName("id_Ticket");
        entity.Property(e => e.IdMaterial).HasColumnName("id_Material");
        entity.Property(e => e.CostoUnitarioAplicado).HasColumnType("smallmoney").HasColumnName("Costo_Unitario_Aplicado");

        entity.HasOne(d => d.IdMaterialNavigation).WithMany(p => p.DetTickets)
            .HasForeignKey(d => d.IdMaterial)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Det_Ticket_Material");

        entity.HasOne(d => d.IdTicketNavigation).WithMany(p => p.DetTickets)
            .HasForeignKey(d => d.IdTicket)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Det_Ticket_Ticket");
    }
}
