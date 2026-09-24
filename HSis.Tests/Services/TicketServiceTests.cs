using FluentAssertions;
using FluentValidation;
using HSis.Data.Models;
using HSis.Contracts.Constants;
using HSis.Contracts.DTOs;
using HSis.Logic.Services;
using HSis.Contracts.Validators;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

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
            new Logic.Profiles.TicketProfile().Register(config);
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
        public async Task CrearTicketAsyncConDatosValidosDebeGuardarYRetornarDto()
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
            resultado.Estatus.Should().Be("Abierto");

            using var contextVerification = new HSisDbContext(options);
            var ticketEnBd = await contextVerification.Tickets.FindAsync(resultado.IdTicket);
            ticketEnBd.Should().NotBeNull();
            ticketEnBd!.Descripcion.Should().Be(nuevoTicketDto.Descripcion);
        }

        [Fact]
        public async Task CrearTicketAsyncConSolicitanteNoRegistradoDebeGuardarEtiquetaEnDescripcion()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<HSisDbContext>()
                .UseInMemoryDatabase(databaseName: "HSis_Test_SolicitanteNoRegistrado")
                .Options;

            var service = new TicketService(CreateFactory(options), _mapper, _createValidator, _updateValidator);

            string nombreSolicitante = "Zulema Ortiz";
            string descripcionProblema = "Fallo en la impresión de reporte.";
            string descripcionConTag = $"[Solicitante no registrado: {nombreSolicitante}]\r\n\r\n{descripcionProblema}";

            var nuevoTicketDto = new TicketCreateDto
            {
                IdUsuario = 1,
                Descripcion = descripcionConTag
            };

            // Act
            var resultado = await service.CrearTicketAsync(nuevoTicketDto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.IdTicket.Should().BeGreaterThan(0);

            using var contextVerification = new HSisDbContext(options);
            var ticketEnBd = await contextVerification.Tickets.FindAsync(resultado.IdTicket);
            ticketEnBd.Should().NotBeNull();
            ticketEnBd!.Descripcion.Should().StartWith($"[Solicitante no registrado: {nombreSolicitante}]");
        }

        [Fact]
        public async Task CrearTicketAsyncConDescripcionCortaDebeLanzarValidationException()
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
        public void ObtenerEstatusPermitidosParaAdminDebeRetornarTodosLosEstatus()
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
        public async Task ObtenerTicketsFiltradosAsyncConFiltrosVariosDebeFiltrarCorrectamente()
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

                db.Tickets.Add(new Ticket { IdTicket = 101, IdUsuario = 10, Estatus = "Abierto", Prioridad = ConstantesPrioridad.ALTA, FechaAlta = DateTime.Today });
                db.Tickets.Add(new Ticket { IdTicket = 102, IdUsuario = 10, Estatus = "En Proceso", Prioridad = ConstantesPrioridad.MEDIA, FechaAlta = DateTime.Today });
                db.Tickets.Add(new Ticket { IdTicket = 103, IdUsuario = 11, Estatus = "Cerrado", Prioridad = ConstantesPrioridad.BAJA, FechaAlta = DateTime.Today.AddDays(-5) });

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
            var filtroComplejo = new TicketFilterDto { Prioridad = ConstantesPrioridad.MEDIA, RangoTemporal = VistaTemporal.Semana };

            var resultadoComplejo = await service.ObtenerTicketsFiltradosAsync(filtroComplejo);

            // Assert 3
            resultadoComplejo.Should().HaveCount(1);
            resultadoComplejo.First().IdTicket.Should().Be(102);
        }

        [Fact]
        public async Task ObtenerIndicadoresTecnicoAsyncDebeCalcularKPIsCorrectamente()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<HSisDbContext>()
                .UseInMemoryDatabase(databaseName: "HSis_Test_IndicadoresTecnico")
                .Options;

            var service = new TicketService(CreateFactory(options), _mapper, _createValidator, _updateValidator);

            using (var db = new HSisDbContext(options))
            {
                // Tecnico 5
                db.Tickets.Add(new Ticket { IdTicket = 201, IdTecnico = 5, Estatus = ConstantesEstatus.EN_PROCESO, Calificacion = 5 });
                db.Tickets.Add(new Ticket { IdTicket = 202, IdTecnico = 5, Estatus = ConstantesEstatus.CERRADO, Calificacion = 4 });
                // Ticket disponible (sin tecnico, estatus Abierto)
                db.Tickets.Add(new Ticket { IdTicket = 203, IdTecnico = null, Estatus = ConstantesEstatus.ABIERTO });
                // Otro tecnico
                db.Tickets.Add(new Ticket { IdTicket = 204, IdTecnico = 6, Estatus = ConstantesEstatus.EN_PROCESO });

                await db.SaveChangesAsync();
            }

            // Act
            var indicadores = await service.ObtenerIndicadoresTecnicoAsync(5);

            // Assert
            indicadores.Asignados.Should().Be(1); // 201 is not closed
            indicadores.Cerrados.Should().Be(1); // 202 is closed
            indicadores.Disponibles.Should().Be(1); // 203
            indicadores.PromedioCalificacion.Should().Be(4.5); // (5+4)/2
        }

        [Fact]
        public async Task ObtenerResumenClienteAsyncDebeRetornarTicketsYContadores()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<HSisDbContext>()
                .UseInMemoryDatabase(databaseName: "HSis_Test_ResumenCliente")
                .Options;

            var service = new TicketService(CreateFactory(options), _mapper, _createValidator, _updateValidator);

            using (var db = new HSisDbContext(options))
            {
                db.Usuarios.Add(new Usuario { IdUsuario = 20, Nombre = "Cliente 1" });
                db.Tickets.Add(new Ticket { IdTicket = 301, IdUsuario = 20, Estatus = ConstantesEstatus.ABIERTO, FechaAlta = DateTime.Now });
                db.Tickets.Add(new Ticket { IdTicket = 302, IdUsuario = 20, Estatus = ConstantesEstatus.EN_PROCESO, FechaAlta = DateTime.Now });
                db.Tickets.Add(new Ticket { IdTicket = 303, IdUsuario = 20, Estatus = ConstantesEstatus.CERRADO, FechaAlta = DateTime.Now });
                // Otro usuario
                db.Tickets.Add(new Ticket { IdTicket = 304, IdUsuario = 21, Estatus = ConstantesEstatus.ABIERTO, FechaAlta = DateTime.Now });

                await db.SaveChangesAsync();
            }

            // Act
            var resumen = await service.ObtenerResumenClienteAsync(20);

            // Assert
            resumen.Activos.Should().Be(2); // 301, 302
            resumen.Cerrados.Should().Be(1); // 303
            resumen.Tickets.Should().HaveCount(3);
        }
    }
}

