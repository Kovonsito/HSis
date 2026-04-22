using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HSis.Data.Models.Configurations;

public class HistorialCambiosTicketConfiguration : IEntityTypeConfiguration<HistorialCambiosTicket>
{
    public void Configure(EntityTypeBuilder<HistorialCambiosTicket> entity)
    {
        entity.HasKey(e => e.IdHistorial).HasName("PK_Historial_Ticket");
        entity.ToTable("Historial_Cambios_Ticket");
        entity.Property(e => e.IdHistorial).HasColumnName("id_Historial");
        entity.Property(e => e.CampoModificado).HasMaxLength(50).IsUnicode(false).HasColumnName("Campo_Modificado");
        entity.Property(e => e.FechaMovimiento).HasDefaultValueSql("(getdate())").HasColumnType("datetime").HasColumnName("Fecha_Movimiento");
        entity.Property(e => e.IdTicket).HasColumnName("id_Ticket");
        entity.Property(e => e.IdUsuarioCambio).HasColumnName("id_Usuario_Cambio");
        entity.Property(e => e.ValorAnterior).IsUnicode(false).HasColumnName("Valor_Anterior");
        entity.Property(e => e.ValorNuevo).IsUnicode(false).HasColumnName("Valor_Nuevo");

        entity.HasOne(d => d.IdTicketNavigation).WithMany(p => p.HistorialCambiosTickets)
            .HasForeignKey(d => d.IdTicket)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Historial_Ticket_Ticket");

        entity.HasOne(d => d.IdUsuarioCambioNavigation).WithMany(p => p.HistorialCambiosTickets)
            .HasForeignKey(d => d.IdUsuarioCambio)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Historial_Usuario");
    }
}
