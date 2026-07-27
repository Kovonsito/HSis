using FluentAssertions;
using HSis.Data.Models;
using HSis.Logic.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace HSis.Tests.Services
{
    public class TicketDetalleServiceTests
    {
        private static IDbContextFactory<HSisDbContext> CreateFactory(DbContextOptions<HSisDbContext> options)
        {
            var mockFactory = new Mock<IDbContextFactory<HSisDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Returns(() => new HSisDbContext(options));
            return mockFactory.Object;
        }

        [Fact]
        public async Task AgregarMaterialATicketAsync_DebeAplicarCostoUnitarioActualDelMaterial()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<HSisDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var db = new HSisDbContext(options))
            {
                db.Materials.Add(new Material { IdMaterial = 100, Nombre = "Cable UTP Cat6", Costo = 15.50m, UnidadMedida = "Metro" });
                db.Tickets.Add(new Ticket { IdTicket = 1, IdUsuario = 1 });
                await db.SaveChangesAsync();
            }

            var service = new TicketDetalleService(CreateFactory(options));

            var detTicket = new DetTicket
            {
                IdTicket = 1,
                IdMaterial = 100,
                Cantidad = 10
            };

            // Act
            await service.AgregarMaterialATicketAsync(detTicket);

            // Assert
            using var dbVerification = new HSisDbContext(options);
            var guardado = await dbVerification.DetTickets.FirstOrDefaultAsync(dt => dt.IdTicket == 1 && dt.IdMaterial == 100);
            guardado.Should().NotBeNull();
            guardado!.CostoUnitarioAplicado.Should().Be(15.50m);
        }

        [Fact]
        public async Task ObtenerCostoTotalMaterialesTicketAsync_DebeSumarCostosPorCantidad()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<HSisDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var db = new HSisDbContext(options))
            {
                db.DetTickets.Add(new DetTicket { IdTicket = 2, IdMaterial = 1, Cantidad = 2, CostoUnitarioAplicado = 100.00m }); // 200
                db.DetTickets.Add(new DetTicket { IdTicket = 2, IdMaterial = 2, Cantidad = 5, CostoUnitarioAplicado = 50.00m });  // 250
                await db.SaveChangesAsync();
            }

            var service = new TicketDetalleService(CreateFactory(options));

            // Act
            decimal totalCosto = await service.ObtenerCostoTotalMaterialesTicketAsync(2);

            // Assert
            totalCosto.Should().Be(450.00m);
        }

        [Fact]
        public async Task EliminarMaterialDeTicketAsync_DebeRemoverDetalle()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<HSisDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var db = new HSisDbContext(options))
            {
                db.DetTickets.Add(new DetTicket { IdTicket = 3, IdMaterial = 10, Cantidad = 1, CostoUnitarioAplicado = 50m });
                await db.SaveChangesAsync();
            }

            var service = new TicketDetalleService(CreateFactory(options));

            // Act
            await service.EliminarMaterialDeTicketAsync(3, 10);

            // Assert
            using var dbVerification = new HSisDbContext(options);
            var det = await dbVerification.DetTickets.FindAsync(3, 10);
            det.Should().BeNull();
        }
    }
}
