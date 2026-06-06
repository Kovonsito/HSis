using Xunit;
using Moq;
using HSis.Logic.Services;
using HSis.Logic.DTOs;
using HSis.Data.Models;
using Microsoft.EntityFrameworkCore;
using Mapster;
using MapsterMapper;
using FluentValidation;
using HSis.Logic.Validators;
using FluentAssertions;

namespace HSis.Tests.Services
{
    public class TicketServiceTests
    {
        private readonly IMapper _mapper;
        private readonly TicketCreateValidator _createValidator;
        private readonly TicketUpdateValidator _updateValidator;

        public TicketServiceTests()
        {
            // Configuración real de Mapster para las pruebas
            var config = new TypeAdapterConfig();
            new HSis.Logic.Profiles.TicketProfile().Register(config);
            _mapper = new Mapper(config);
            _createValidator = new TicketCreateValidator();
            _updateValidator = new TicketUpdateValidator();
        }

        private static IDbContextFactory<HSisDbContext> CreateFactory(DbContextOptions<HSisDbContext> options)
        {
            var mockFactory = new Mock<IDbContextFactory<HSisDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Returns(() => new HSisDbContext(options));
            return mockFactory.Object;
        }

        [Fact]
        public async Task CrearTicketAsync_ConDatosValidos_DebeGuardarYRetornarDto()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<HSisDbContext>()
                .UseInMemoryDatabase(databaseName: "HSis_Test_Crear")
                .Options;

            var service = new TicketService(CreateFactory(options), _mapper, _createValidator, _updateValidator);

            var nuevoTicketDto = new TicketCreateDto
            {
                IdUsuario = 1,
                Descripcion = "Problema con la impresora de etiquetas."
            };

            // Act
            var resultado = await service.CrearTicketAsync(nuevoTicketDto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.IdTicket.Should().BeGreaterThan(0);
            resultado.Status.Should().Be("Abierto");

            using var contextVerification = new HSisDbContext(options);
            var ticketEnBd = await contextVerification.Tickets.FindAsync(resultado.IdTicket);
            ticketEnBd.Should().NotBeNull();
            ticketEnBd!.Descripción.Should().Be(nuevoTicketDto.Descripcion);
        }

        [Fact]
        public async Task CrearTicketAsync_ConDescripcionCorta_DebeLanzarValidationException()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<HSisDbContext>()
                .UseInMemoryDatabase(databaseName: "HSis_Test_Validacion")
                .Options;

            var service = new TicketService(CreateFactory(options), _mapper, _createValidator, _updateValidator);

            var ticketInvalido = new TicketCreateDto
            {
                IdUsuario = 1,
                Descripcion = "Corto" // Menos de 10 caracteres
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => service.CrearTicketAsync(ticketInvalido));
        }

        [Fact]
        public void ObtenerEstatusPermitidos_ParaAdmin_DebeRetornarTodosLosEstatus()
        {
            // Arrange
            var mockFactory = new Mock<IDbContextFactory<HSisDbContext>>();
            var service = new TicketService(mockFactory.Object, _mapper, _createValidator, _updateValidator);

            // Act
            var resultado = TicketService.ObtenerEstatusPermitidos(1, "Abierto"); // Rol 1 = Admin

            // Assert
            resultado.Should().Contain("Cerrado");
            resultado.Should().Contain("Reabierto");
            resultado.Should().HaveCount(4);
        }

        [Fact]
        public async Task ObtenerTicketsFiltradosAsync_ConFiltrosVarios_DebeFiltrarCorrectamente()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<HSisDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Nombre único para evitar conflictos de BD en memoria
                .Options;

            var factory = CreateFactory(options);
            var service = new TicketService(factory, _mapper, _createValidator, _updateValidator);

            // Poblar base de datos de prueba
            using (var db = new HSisDbContext(options))
            {
                db.Usuarios.Add(new Usuario { IdUsuario = 10, Nombre = "Juan Perez" });
                db.Usuarios.Add(new Usuario { IdUsuario = 11, Nombre = "Pedro Gomez" });

                db.Tickets.Add(new Ticket { IdTicket = 101, IdUsuario = 10, Status = "Abierto", Prioridad = "Alta", Alta = DateTime.Today });
                db.Tickets.Add(new Ticket { IdTicket = 102, IdUsuario = 10, Status = "En Proceso", Prioridad = "Media", Alta = DateTime.Today });
                db.Tickets.Add(new Ticket { IdTicket = 103, IdUsuario = 11, Status = "Cerrado", Prioridad = "Baja", Alta = DateTime.Today.AddDays(-5) });

                await db.SaveChangesAsync();
            }

            // Act 1: Filtrar por Estatus
            var filtroEstatus = new TicketFilterDto { Estatus = "Abierto" };
            var resultadoEstatus = await service.ObtenerTicketsFiltradosAsync(filtroEstatus);

            // Assert 1
            resultadoEstatus.Should().HaveCount(1);
            resultadoEstatus.First().IdTicket.Should().Be(101);

            // Act 2: Filtrar por Usuario Emisor
            var filtroUsuario = new TicketFilterDto { UsuarioEmisor = "Juan" };
            var resultadoUsuario = await service.ObtenerTicketsFiltradosAsync(filtroUsuario);

            // Assert 2
            resultadoUsuario.Should().HaveCount(2);

            // Act 3: Filtrar por Prioridad y Rango Temporal
            var filtroComplejo = new TicketFilterDto { Prioridad = "Media", RangoTemporal = VistaTemporal.Semana };
            var resultadoComplejo = await service.ObtenerTicketsFiltradosAsync(filtroComplejo);

            // Assert 3
            resultadoComplejo.Should().HaveCount(1);
            resultadoComplejo.First().IdTicket.Should().Be(102);
        }
    }
}
