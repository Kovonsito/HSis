using FluentAssertions;
using HSis.Data.Models;
using HSis.Logic.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace HSis.Tests.Services
{
    public class MaterialServiceTests
    {
        private static IDbContextFactory<HSisDbContext> CreateFactory(DbContextOptions<HSisDbContext> options)
        {
            var mockFactory = new Mock<IDbContextFactory<HSisDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Returns(() => new HSisDbContext(options));
            return mockFactory.Object;
        }

        [Fact]
        public async Task ActualizarCostoMaterialAsyncConIdValidoDebeActualizarCostoEnBD()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<HSisDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var db = new HSisDbContext(options))
            {
                db.Materials.Add(new Material { IdMaterial = 1, Nombre = "Conector RJ45", Costo = 5.00m, UnidadMedida = "Pieza" });
                await db.SaveChangesAsync();
            }

            var service = new MaterialService(CreateFactory(options));

            // Act
            await service.ActualizarCostoMaterialAsync(1, 7.50m);

            // Assert
            using var dbVerification = new HSisDbContext(options);
            var mat = await dbVerification.Materials.FindAsync(1);
            mat.Should().NotBeNull();
            mat!.Costo.Should().Be(7.50m);
        }
    }
}
