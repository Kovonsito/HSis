using FluentAssertions;
using HSis.Data.Models;
using HSis.Logic.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace HSis.Tests.Services
{
    public class CatalogoServiceTests
    {
        private static IDbContextFactory<HSisDbContext> CreateFactory(DbContextOptions<HSisDbContext> options)
        {
            var mockFactory = new Mock<IDbContextFactory<HSisDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Returns(() => new HSisDbContext(options));
            return mockFactory.Object;
        }

        private static IServiceProvider CreateMockServiceProvider()
        {
            var mockProvider = new Mock<IServiceProvider>();
            return mockProvider.Object;
        }

        [Fact]
        public async Task CrearAsyncConEntidadUsuarioDebeHashearContraseñaYGuardar()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<HSisDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var service = new CatalogoService(CreateFactory(options), CreateMockServiceProvider());

            var nuevoUsuario = new Usuario
            {
                Nombre = "NuevoUser",
                Contraseña = "PlainTextPassword123",
                IdRol = 3
            };

            // Act
            await service.CrearAsync(nuevoUsuario);

            // Assert
            using var dbVerification = new HSisDbContext(options);
            var uEnBd = await dbVerification.Usuarios.FirstOrDefaultAsync(u => u.Nombre == "NuevoUser");
            uEnBd.Should().NotBeNull();
            uEnBd!.Contraseña.Should().NotBe("PlainTextPassword123");
            BCrypt.Net.BCrypt.Verify("PlainTextPassword123", uEnBd.Contraseña).Should().BeTrue();
        }

        [Fact]
        public async Task ObtenerTodosAsyncDebeRetornarListaDeEntidades()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<HSisDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var db = new HSisDbContext(options))
            {
                db.Departamentos.Add(new Departamento { IdDepartamento = 1, Nombre = "Sistemas" });
                db.Departamentos.Add(new Departamento { IdDepartamento = 2, Nombre = "Recursos Humanos" });
                await db.SaveChangesAsync();
            }

            var service = new CatalogoService(CreateFactory(options), CreateMockServiceProvider());

            // Act
            var resultado = await service.ObtenerTodosAsync<Departamento>();

            // Assert
            resultado.Should().HaveCount(2);
        }

        [Fact]
        public async Task ObtenerFiltradoAsyncConPredicadoDebeFiltrarResultados()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<HSisDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var db = new HSisDbContext(options))
            {
                db.Sucursales.Add(new Sucursal { IdSucursal = 1, Nombre = "Matriz" });
                db.Sucursales.Add(new Sucursal { IdSucursal = 2, Nombre = "Norte" });
                await db.SaveChangesAsync();
            }

            var service = new CatalogoService(CreateFactory(options), CreateMockServiceProvider());

            // Act
            var resultado = await service.ObtenerFiltradoAsync<Sucursal>(s => s.Nombre == "Matriz");

            // Assert
            resultado.Should().HaveCount(1);
            resultado.First().IdSucursal.Should().Be(1);
        }

        [Fact]
        public async Task EliminarAsyncConIdValidoDebeRemoverEntidad()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<HSisDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var db = new HSisDbContext(options))
            {
                db.Puestos.Add(new Puesto { IdPuesto = 5, Nombre = "Analista" });
                await db.SaveChangesAsync();
            }

            var service = new CatalogoService(CreateFactory(options), CreateMockServiceProvider());

            // Act
            await service.EliminarAsync<Puesto>(5);

            // Assert
            using var dbVerification = new HSisDbContext(options);
            var puesto = await dbVerification.Puestos.FindAsync(5);
            puesto.Should().BeNull();
        }

        [Fact]
        public async Task ObtenerSiguienteIdAsyncConRegistrosExistentesDebeCalcularIdSiguiente()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<HSisDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var db = new HSisDbContext(options))
            {
                db.Puestos.Add(new Puesto { IdPuesto = 10, Nombre = "Puesto 10" });
                db.Puestos.Add(new Puesto { IdPuesto = 25, Nombre = "Puesto 25" });
                await db.SaveChangesAsync();
            }

            var service = new CatalogoService(CreateFactory(options), CreateMockServiceProvider());

            // Act
            int siguienteId = await service.ObtenerSiguienteIdAsync(typeof(Puesto), nameof(Puesto.IdPuesto));

            // Assert
            siguienteId.Should().Be(26);
        }
    }
}

