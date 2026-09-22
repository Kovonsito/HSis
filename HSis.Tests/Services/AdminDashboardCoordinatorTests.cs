using FluentAssertions;
using HSis.Contracts.Coordinators;
using HSis.Logic.Constants;
using HSis.Logic.DTOs;
using HSis.Logic.Services;
using Moq;
using Xunit;

namespace HSis.Tests.Services
{
    public class AdminDashboardCoordinatorTests
    {
        private readonly Mock<ITicketService> _mockTicketService;
        private readonly Mock<ICatalogoService> _mockCatalogoService;
        private readonly Mock<IUsuarioService> _mockUsuarioService;
        private readonly Mock<IMaterialService> _mockMaterialService;
        private readonly AdminDashboardCoordinator _coordinator;

        public AdminDashboardCoordinatorTests()
        {
            _mockTicketService = new Mock<ITicketService>();
            _mockCatalogoService = new Mock<ICatalogoService>();
            _mockUsuarioService = new Mock<IUsuarioService>();
            _mockMaterialService = new Mock<IMaterialService>();

            _coordinator = new AdminDashboardCoordinator(
                _mockTicketService.Object,
                _mockCatalogoService.Object,
                _mockUsuarioService.Object,
                _mockMaterialService.Object
            );
        }

        [Fact]
        public async Task ObtenerTecnicosYAdminsAsync_DebeIncluirValoresPorDefectoYUsuariosMapeados()
        {
            // Arrange
            _mockUsuarioService
                .Setup(s => s.ObtenerUsuariosPorRolAsync(RolUsuarioEnum.Administrador))
                .ReturnsAsync(new List<UsuarioDto>
                {
                    new() { IdUsuario = 101, Nombre = "Laura Admin" }
                });

            _mockUsuarioService
                .Setup(s => s.ObtenerUsuariosPorRolAsync(RolUsuarioEnum.Tecnico))
                .ReturnsAsync(new List<UsuarioDto>
                {
                    new() { IdUsuario = 202, Nombre = "Mario Tecnico" }
                });

            // Act
            var resultado = await _coordinator.ObtenerTecnicosYAdminsAsync();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(4);

            resultado[0].Id.Should().Be(0);
            resultado[0].Nombre.Should().Be("Todos");

            resultado[1].Id.Should().Be(-1);
            resultado[1].Nombre.Should().Be("Sin Asignar");

            resultado.Should().Contain(x => x.Id == 101 && x.Nombre == "Admin - Laura Admin");
            resultado.Should().Contain(x => x.Id == 202 && x.Nombre == "Técnico - Mario Tecnico");
        }

        [Fact]
        public async Task ObtenerResumenKPIsAsync_DebeLlamarATicketServiceYRetornarResumen()
        {
            // Arrange
            var resumenEsperado = new DashboardResumenDto
            {
                TicketsNuevos = 7,
                TicketsUrgentes = 3,
                TicketsEnProceso = 4,
                TicketsCerrados = 15,
                TicketsReabiertos = 1,
                PromedioCalificacion = 4.9
            };

            _mockTicketService
                .Setup(s => s.ObtenerResumenDashboardAsync(42))
                .ReturnsAsync(resumenEsperado);

            // Act
            var resultado = await _coordinator.ObtenerResumenKPIsAsync(42);

            // Assert
            resultado.Should().NotBeNull();
            resultado.TicketsNuevos.Should().Be(7);
            resultado.TicketsUrgentes.Should().Be(3);
            resultado.PromedioCalificacion.Should().Be(4.9);
            _mockTicketService.Verify(s => s.ObtenerResumenDashboardAsync(42), Times.Once);
        }

        [Fact]
        public async Task ObtenerCalificacionPromedioAsync_DebeLlamarATicketService()
        {
            // Arrange
            _mockTicketService
                .Setup(s => s.ObtenerPromedioCalificacionTecnicoAsync(15))
                .ReturnsAsync(4.75);

            // Act
            var promedio = await _coordinator.ObtenerCalificacionPromedioAsync(15);

            // Assert
            promedio.Should().Be(4.75);
            _mockTicketService.Verify(s => s.ObtenerPromedioCalificacionTecnicoAsync(15), Times.Once);
        }

        [Fact]
        public async Task FiltrarTicketsPaginadosAsync_DebeDelegarLlamadaATicketService()
        {
            // Arrange
            var filtros = new TicketFilterDto { Prioridad = "Alta" };
            var resultadoPaginado = new PaginatedResultDto<TicketDto>
            {
                TotalCount = 1,
                Items = [new TicketDto { IdTicket = 99, Folio = "TK-99", Prioridad = "Alta" }]
            };

            _mockTicketService
                .Setup(s => s.ObtenerTicketsFiltradosPaginadosAsync(filtros, 1, 20))
                .ReturnsAsync(resultadoPaginado);

            // Act
            var resultado = await _coordinator.FiltrarTicketsPaginadosAsync(filtros, 1, 20);

            // Assert
            resultado.Should().NotBeNull();
            resultado.TotalCount.Should().Be(1);
            resultado.Items.Should().ContainSingle(t => t.Folio == "TK-99");
            _mockTicketService.Verify(s => s.ObtenerTicketsFiltradosPaginadosAsync(filtros, 1, 20), Times.Once);
        }

        [Fact]
        public async Task CargarDatosCatalogoAsync_ConTipoMaterial_DebeLlamarAMaterialService()
        {
            // Arrange
            var listaMateriales = new List<MaterialDto>
            {
                new() { IdMaterial = 1, Codigo = "MAT-01", Nombre = "Cable UTP" }
            };

            _mockMaterialService
                .Setup(s => s.ObtenerMaterialesAsync())
                .ReturnsAsync(listaMateriales);

            // Act
            var resultado = await _coordinator.CargarDatosCatalogoAsync(typeof(MaterialDto));

            // Assert
            resultado.Should().BeEquivalentTo(listaMateriales);
            _mockMaterialService.Verify(s => s.ObtenerMaterialesAsync(), Times.Once);
            _mockCatalogoService.Verify(s => s.ObtenerTodosPorTipoAsync(It.IsAny<Type>()), Times.Never);
        }

        [Fact]
        public async Task CargarDatosCatalogoAsync_ConOtroTipo_DebeLlamarACatalogoService()
        {
            // Arrange
            var listaDepartamentos = new List<DepartamentoDto>
            {
                new() { IdDepartamento = 5, Nombre = "Sistemas" }
            };

            _mockCatalogoService
                .Setup(s => s.ObtenerTodosPorTipoAsync(typeof(DepartamentoDto)))
                .ReturnsAsync(listaDepartamentos);

            // Act
            var resultado = await _coordinator.CargarDatosCatalogoAsync(typeof(DepartamentoDto));

            // Assert
            resultado.Should().BeEquivalentTo(listaDepartamentos);
            _mockCatalogoService.Verify(s => s.ObtenerTodosPorTipoAsync(typeof(DepartamentoDto)), Times.Once);
            _mockMaterialService.Verify(s => s.ObtenerMaterialesAsync(), Times.Never);
        }
    }
}
