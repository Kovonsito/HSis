using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HSis.Data.Models.Configurations;

public class NotificacionConfiguration : IEntityTypeConfiguration<Notificacion>
{
    public void Configure(EntityTypeBuilder<Notificacion> entity)
    {
        entity.HasKey(e => e.IdNotificacion);
        entity.ToTable("Notificacion");

        entity.Property(e => e.IdNotificacion).HasColumnName("id_Notificacion");
        entity.Property(e => e.UsuarioDestinoId).HasColumnName("usuario_Destino_Id");

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

        entity.HasOne(d => d.UsuarioDestino)
            .WithMany()
            .HasForeignKey(d => d.UsuarioDestinoId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Notificacion_Usuario");
    }
}
