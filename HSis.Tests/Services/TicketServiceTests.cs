using Xunit;
using Moq;
using HSis.Logic.Services;
using HSis.Logic.DTOs;
using HSis.Data.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
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
            // Configuración real de AutoMapper para las pruebas
            var config = new MapperConfiguration(cfg => cfg.AddProfile<HSis.Logic.Profiles.TicketProfile>());
            _mapper = config.CreateMapper();
            _createValidator = new TicketCreateValidator();
            _updateValidator = new TicketUpdateValidator();
        }

        private IDbContextFactory<HSisDbContext> CreateFactory(HSisDbContext context)
        {
            var mockFactory = new Mock<IDbContextFactory<HSisDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Returns(context);
            return mockFactory.Object;
        }

        [Fact]
        public async Task CrearTicketAsync_ConDatosValidos_DebeGuardarYRetornarDto()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<HSisDbContext>()
                .UseInMemoryDatabase(databaseName: "HSis_Test_Crear")
                .Options;

            using var context = new HSisDbContext(options);
            var service = new TicketService(CreateFactory(context), _mapper, _createValidator, _updateValidator);

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
            
            var ticketEnBd = await context.Tickets.FindAsync(resultado.IdTicket);
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

            using var context = new HSisDbContext(options);
            var service = new TicketService(CreateFactory(context), _mapper, _createValidator, _updateValidator);

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
            var resultado = service.ObtenerEstatusPermitidos(1, "Abierto"); // Rol 1 = Admin

            // Assert
            resultado.Should().Contain("Cerrado");
            resultado.Should().Contain("Reabierto");
            resultado.Should().HaveCount(4);
        }
    }
}
