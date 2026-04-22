using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HSis.Data.Models.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> entity)
    {
        entity.HasKey(e => e.IdTicket).HasName("PK__Ticket__81109ADA1E0C898B");
        entity.ToTable("Ticket");
        entity.Property(e => e.IdTicket).HasColumnName("id_Ticket");
        entity.Property(e => e.Alta).HasDefaultValueSql("(getdate())", "DF__Ticket__Alta__6FE99F9F").HasColumnType("datetime");
        entity.Property(e => e.Atención).HasColumnType("datetime");
        entity.Property(e => e.Cierre).HasColumnType("datetime");
        entity.Property(e => e.Descripción).HasMaxLength(500).IsUnicode(false);
        entity.Property(e => e.IdTecnico).HasColumnName("id_Tecnico");
        entity.Property(e => e.IdUsuario).HasColumnName("id_Usuario");
        entity.Property(e => e.Solución).IsUnicode(false);
        entity.Property(e => e.Status).HasMaxLength(15).IsUnicode(false).HasDefaultValue("Abierto", "DF__Ticket__Status__70DDC3D8");

        entity.HasOne(d => d.IdTecnicoNavigation).WithMany(p => p.TicketIdTecnicoNavigations)
            .HasForeignKey(d => d.IdTecnico)
            .HasConstraintName("FK_Ticket_Tecnico");

        entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.TicketIdUsuarioNavigations)
            .HasForeignKey(d => d.IdUsuario)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Ticket_Usuario");
    }
}
