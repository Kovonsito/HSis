using FluentAssertions;
using HSis.Contracts.Constants;
using HSis.Data.Models;
using HSis.Logic.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace HSis.Tests.Services;

public sealed class NotificacionDestinatariosAndFactoryTests
{
    [Fact]
    public async Task ObtenerDestinatariosNuevoTicketDebeRetornarTecnicosYAdministradoresSinDuplicados()
    {
        var options = CrearOpciones();
        await using (var db = new HSisDbContext(options))
        {
            db.Usuarios.AddRange(
                new Usuario { IdUsuario = 1, IdRol = (int)RolUsuarioEnum.Administrador },
                new Usuario { IdUsuario = 2, IdRol = (int)RolUsuarioEnum.Tecnico },
                new Usuario { IdUsuario = 3, IdRol = (int)RolUsuarioEnum.Cliente },
                new Usuario { IdUsuario = 4, IdRol = (int)RolUsuarioEnum.Tecnico });
            await db.SaveChangesAsync();
        }

        var factory = CrearFactory(options);
        var resultado = await new NotificacionDestinatariosService(factory.Object)
            .ObtenerDestinatariosNuevoTicketAsync();

        resultado.Should().Equal(1, 2, 4);
    }

    [Fact]
    public async Task ObtenerDestinatariosCalificacionDebeIncluirTecnicoYAdministradoresUnaSolaVez()
    {
        var options = CrearOpciones();
        await using (var db = new HSisDbContext(options))
        {
            db.Usuarios.AddRange(
                new Usuario { IdUsuario = 1, IdRol = (int)RolUsuarioEnum.Administrador },
                new Usuario { IdUsuario = 2, IdRol = (int)RolUsuarioEnum.Administrador },
                new Usuario { IdUsuario = 3, IdRol = (int)RolUsuarioEnum.Tecnico });
            await db.SaveChangesAsync();
        }

        var resultado = await new NotificacionDestinatariosService(CrearFactory(options).Object)
            .ObtenerDestinatariosCalificacionAsync(3);

        resultado.Should().Equal(1, 2, 3);
    }

    [Fact]
    public void CrearDebeConstruirEntidadNoLeidaConTicketYMensajeCanonico()
    {
        var fecha = new DateTime(2026, 2, 3, 10, 15, 0);
        var mensaje = NotificacionFactory.CrearMensajeCambioEstado(42, "TCK-00042", "En proceso");
        var resultado = NotificacionFactory.Crear(7, 42, ConstantesTiposNotificacion.EstadoTicket, mensaje, fecha);

        resultado.UsuarioDestinoId.Should().Be(7);
        resultado.TicketId.Should().Be(42);
        resultado.Tipo.Should().Be(ConstantesTiposNotificacion.EstadoTicket);
        resultado.Mensaje.Should().Contain("42").And.Contain("En proceso");
        resultado.FechaCreacion.Should().Be(fecha);
        resultado.Leido.Should().BeFalse();
    }

    [Theory]
    [InlineData("TCK-00042", 42, "42")]
    [InlineData("ticket-00108", 42, "108")]
    [InlineData(null, 42, "42")]
    public void ObtenerFolioNumericoDebeUsarElFolioEstructuradoOCaerAlId(
        string? folio,
        int ticketId,
        string esperado)
        => NotificacionFactory.ObtenerFolioNumerico(folio, ticketId).Should().Be(esperado);

    private static Mock<IDbContextFactory<HSisDbContext>> CrearFactory(DbContextOptions<HSisDbContext> options)
    {
        var factory = new Mock<IDbContextFactory<HSisDbContext>>();
        factory.Setup(item => item.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((CancellationToken _) => new HSisDbContext(options));
        factory.Setup(item => item.CreateDbContext())
            .Returns(() => new HSisDbContext(options));
        return factory;
    }

    private static DbContextOptions<HSisDbContext> CrearOpciones()
        => new DbContextOptionsBuilder<HSisDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
}
