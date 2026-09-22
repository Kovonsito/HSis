using FluentAssertions;
using HSis.Data.Models;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
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

        [Fact]
        public async Task ObtenerMaterialesAsyncDebeRetornarListaDeMaterialDto()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<HSisDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var db = new HSisDbContext(options))
            {
                db.Materials.AddRange(
                    new Material { IdMaterial = 1, Nombre = "Cable UTP Cat6", Costo = 12.0m, Inventario = 100, UnidadMedida = "Metro" },
                    new Material { IdMaterial = 2, Nombre = "Patchcord 1m", Costo = 25.0m, Inventario = 20, UnidadMedida = "Pieza" }
                );
                await db.SaveChangesAsync();
            }

            var service = new MaterialService(CreateFactory(options));

            // Act
            var resultado = await service.ObtenerMaterialesAsync();

            // Assert
            resultado.Should().HaveCount(2);
            resultado.First().Nombre.Should().Be("Cable UTP Cat6");
            resultado.First().StockActual.Should().Be(100);
        }

        [Fact]
        public async Task RegistrarMovimientoAsyncDebeActualizarInventarioYGuardarMovimiento()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<HSisDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var db = new HSisDbContext(options))
            {
                db.Materials.Add(new Material { IdMaterial = 1, Nombre = "Teclado USB", Costo = 150m, Inventario = 10, UnidadMedida = "Pieza" });
                await db.SaveChangesAsync();
            }

            var service = new MaterialService(CreateFactory(options));

            // Act
            await service.RegistrarMovimientoAsync(new KardexMovimientoDto
            {
                IdMaterial = 1,
                Cantidad = 5,
                CostoUnitario = 160m,
                IdUsuario = 1,
                Motivo = "Compra nueva"
            });

            // Assert
            using var dbVerification = new HSisDbContext(options);
            var mat = await dbVerification.Materials.FindAsync(1);
            mat.Should().NotBeNull();
            mat!.Inventario.Should().Be(15);
            mat.Costo.Should().Be(160m);

            var mov = await dbVerification.MovimientosMateriales.FirstOrDefaultAsync();
            mov.Should().NotBeNull();
            mov!.Cantidad.Should().Be(5);
        }
    }
}

