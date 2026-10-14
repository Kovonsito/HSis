using System.IO;
using HSis.Desktop.Infrastructure.Offline.Models;
using Microsoft.EntityFrameworkCore;

namespace HSis.Desktop.Infrastructure.Offline;

public class HSisLocalDbContext : DbContext
{
    public DbSet<OperacionOutboxLocal> OperacionesOutbox => Set<OperacionOutboxLocal>();
    public DbSet<TicketCacheLocal> TicketsCache => Set<TicketCacheLocal>();
    public DbSet<CatalogoItemCacheLocal> CatalogosCache => Set<CatalogoItemCacheLocal>();

    private readonly string _dbPath;

    public HSisLocalDbContext(string? dbPath = null)
    {
        _dbPath = string.IsNullOrWhiteSpace(dbPath)
            ? ConfiguracionEscritorio.ObtenerRutaDatos(ConfiguracionEscritorio.ArchivoBaseDatosOffline)
            : dbPath;
    }

    public HSisLocalDbContext(DbContextOptions<HSisLocalDbContext> options) : base(options)
    {
        _dbPath = ConfiguracionEscritorio.ObtenerRutaDatos(ConfiguracionEscritorio.ArchivoBaseDatosOffline);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var directory = Path.GetDirectoryName(_dbPath);
            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            optionsBuilder.UseSqlite($"Data Source={_dbPath}");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OperacionOutboxLocal>(b =>
        {
            b.ToTable("OperacionesOutbox");
            b.HasKey(o => o.IdOperacion);
            b.Property(o => o.IdOperacion).ValueGeneratedNever();
            b.Property(o => o.TipoOperacion).HasMaxLength(50).IsRequired();
            b.Property(o => o.RutaEndpoint).HasMaxLength(250).IsRequired();
            b.Property(o => o.MetodoHttp).HasMaxLength(10).IsRequired();
            b.Property(o => o.PayloadJson).IsRequired();
            b.HasIndex(o => o.Estado);
            b.HasIndex(o => o.FechaCreacion);
        });

        modelBuilder.Entity<TicketCacheLocal>(b =>
        {
            b.ToTable("TicketsCache");
            b.HasKey(t => t.IdTicket);
            b.Property(t => t.IdTicket).ValueGeneratedNever();
            b.Property(t => t.Estatus).HasMaxLength(50).IsRequired();
            b.HasIndex(t => t.IdUsuario);
            b.HasIndex(t => t.IdTecnico);
        });

        modelBuilder.Entity<CatalogoItemCacheLocal>(b =>
        {
            b.ToTable("CatalogosCache");
            b.HasKey(c => c.Id);
            b.HasIndex(c => new { c.TipoCatalogo, c.IdItem });
        });
    }
}
