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
        entity.Property(e => e.FechaAlta).HasDefaultValueSql("(getdate())", "DF__Ticket__Alta__6FE99F9F").HasColumnType("datetime").HasColumnName("FechaAlta");
        entity.Property(e => e.FechaAtencion).HasColumnType("datetime").HasColumnName("FechaAtencion");
        entity.Property(e => e.FechaCierre).HasColumnType("datetime").HasColumnName("FechaCierre");
        entity.Property(e => e.Descripcion).HasMaxLength(500).IsUnicode(false).HasColumnName("Descripcion");
        entity.Property(e => e.IdTecnico).HasColumnName("id_Tecnico");
        entity.Property(e => e.IdUsuario).HasColumnName("id_Usuario");
        entity.Property(e => e.Solucion).IsUnicode(false).HasColumnName("Solucion");
        entity.Property(e => e.Prioridad).HasMaxLength(20).IsUnicode(false);
        entity.Property(e => e.Estatus).HasMaxLength(15).IsUnicode(false).HasDefaultValue("Abierto", "DF__Ticket__Status__70DDC3D8").HasColumnName("Estatus");
        entity.Property(e => e.Calificacion).HasColumnName("Calificacion");
        entity.Property(e => e.ComentarioEvaluacion).HasMaxLength(1000).IsUnicode(false).HasColumnName("ComentarioEvaluacion");
        entity.Property(e => e.FechaEvaluacion).HasColumnType("datetime").HasColumnName("FechaEvaluacion");

        entity.HasOne(d => d.Tecnico).WithMany(p => p.TicketsComoTecnico)
            .HasForeignKey(d => d.IdTecnico)
            .HasConstraintName("FK_Ticket_Tecnico");

        entity.HasOne(d => d.Usuario).WithMany(p => p.TicketsComoUsuario)
            .HasForeignKey(d => d.IdUsuario)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Ticket_Usuario");
    }
}
