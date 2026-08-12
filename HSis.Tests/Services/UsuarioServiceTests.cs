using FluentAssertions;
using HSis.Data.Models;
using HSis.Logic.Services;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace HSis.Tests.Services
{
    public class UsuarioServiceTests
    {
        private readonly IMapper _mapper;

        public UsuarioServiceTests()
        {
            var config = new TypeAdapterConfig();
            new Logic.Profiles.TicketProfile().Register(config);
            _mapper = new Mapper(config);
        }

        private static IDbContextFactory<HSisDbContext> CreateFactory(DbContextOptions<HSisDbContext> options)
        {
            var mockFactory = new Mock<IDbContextFactory<HSisDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Returns(() => new HSisDbContext(options));
            return mockFactory.Object;
        }

        [Fact]
        public void HashPasswordConPasswordValidoDebeGenerarHashBCrypt()
        {
            // Arrange
            string passwordOriginal = "Password123!";

            // Act
            string hashed = UsuarioService.HashPassword(passwordOriginal);

            // Assert
            hashed.Should().NotBeNullOrEmpty();
            hashed.Should().NotBe(passwordOriginal);
            BCrypt.Net.BCrypt.Verify(passwordOriginal, hashed).Should().BeTrue();
        }

        [Fact]
        public async Task AutenticarAsyncConCredencialesCorrectasDebeRetornarUsuarioDto()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<HSisDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var db = new HSisDbContext(options))
            {
                db.Usuarios.Add(new Usuario
                {
                    IdUsuario = 1,
                    Nombre = "JuanPerez",
                    Contraseña = UsuarioService.HashPassword("Secret123"),
                    IdRol = 1
                });
                await db.SaveChangesAsync();
            }

            var service = new UsuarioService(CreateFactory(options), _mapper);

            // Act
            var usuarioDto = await service.AutenticarAsync("JuanPerez", "Secret123");

            // Assert
            usuarioDto.Should().NotBeNull();
            usuarioDto!.Nombre.Should().Be("JuanPerez");
            usuarioDto.IdRol.Should().Be(1);
        }

        [Fact]
        public async Task AutenticarAsyncConContraseñaIncorrectaDebeRetornarNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<HSisDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var db = new HSisDbContext(options))
            {
                db.Usuarios.Add(new Usuario
                {
                    IdUsuario = 2,
                    Nombre = "MariaGomez",
                    Contraseña = UsuarioService.HashPassword("Correcta123"),
                    IdRol = 2
                });
                await db.SaveChangesAsync();
            }

            var service = new UsuarioService(CreateFactory(options), _mapper);

            // Act
            var resultado = await service.AutenticarAsync("MariaGomez", "Incorrecta999");

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task ObtenerUsuariosPorRolAsyncDebeFiltrarPorRol()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<HSisDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var db = new HSisDbContext(options))
            {
                db.Usuarios.Add(new Usuario { IdUsuario = 10, Nombre = "UserAdmin", IdRol = 1 });
                db.Usuarios.Add(new Usuario { IdUsuario = 11, Nombre = "UserTecnico", IdRol = 2 });
                db.Usuarios.Add(new Usuario { IdUsuario = 12, Nombre = "UserCliente", IdRol = 3 });
                await db.SaveChangesAsync();
            }

            var service = new UsuarioService(CreateFactory(options), _mapper);

            // Act
            var tecnicos = await service.ObtenerUsuariosPorRolAsync(2);

            // Assert
            tecnicos.Should().HaveCount(1);
            tecnicos.First().Nombre.Should().Be("UserTecnico");
        }
    }
}
