using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HSis.Data.Models.Configurations
{
    public class NotificacionConfiguration : IEntityTypeConfiguration<Notificacion>
    {
        public void Configure(EntityTypeBuilder<Notificacion> entity)
        {
            entity.HasKey(e => e.IdNotificacion);
            entity.ToTable("Notificacion");

            entity.Property(e => e.IdNotificacion).HasColumnName("id_Notificacion");
            entity.Property(e => e.UsuarioDestinoId).HasColumnName("usuario_Destino_Id");
            entity.Property(e => e.TicketId).HasColumnName("id_Ticket");
            entity.Property(e => e.MaterialId).HasColumnName("id_Material");

            entity.Property(e => e.Mensaje)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.Leido)
                .HasDefaultValue(false);

            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("GETDATE()");

            entity.HasIndex(e => new { e.UsuarioDestinoId, e.Leido, e.FechaCreacion })
                .HasDatabaseName("IX_Notificacion_Usuario_Leido_Fecha");

            entity.HasOne(d => d.Ticket)
                .WithMany()
                .HasForeignKey(d => d.TicketId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Notificacion_Ticket");

            entity.HasOne(d => d.Material)
                .WithMany()
                .HasForeignKey(d => d.MaterialId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Notificacion_Material");

            entity.HasOne(d => d.UsuarioDestino)
                .WithMany()
                .HasForeignKey(d => d.UsuarioDestinoId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Notificacion_Usuario");
        }
    }
}

