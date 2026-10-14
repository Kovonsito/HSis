using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HSis.Data.Models.Configurations;

public class OperacionIdempotenteConfiguration : IEntityTypeConfiguration<OperacionIdempotente>
{
    public void Configure(EntityTypeBuilder<OperacionIdempotente> builder)
    {
        builder.ToTable("OperacionesIdempotentes");
        builder.HasKey(e => e.IdOperacion);
        builder.Property(e => e.IdOperacion).ValueGeneratedNever();
        builder.Property(e => e.RutaEndpoint).HasMaxLength(250).IsRequired();
        builder.Property(e => e.MetodoHttp).HasMaxLength(10).IsRequired();
        builder.Property(e => e.CodigoEstadoHttp).IsRequired();
        builder.Property(e => e.FechaCreacion).HasDefaultValueSql("SYSUTCDATETIME()");
        builder.HasIndex(e => e.FechaCreacion);
    }
}
