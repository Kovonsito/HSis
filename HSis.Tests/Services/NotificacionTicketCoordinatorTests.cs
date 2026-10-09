using FluentAssertions;
using HSis.Contracts.Constants;
using HSis.Contracts.Services;
using HSis.Data.Models;
using HSis.Logic.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace HSis.Tests.Services;

public sealed class NotificacionTicketCoordinatorTests
{
    [Fact]
    public async Task AsignacionYReasignacionDebenMostrarElNombreDelTecnico()
    {
        var options = new DbContextOptionsBuilder<HSisDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using (var db = new HSisDbContext(options))
        {
            db.Usuarios.AddRange(
                new Usuario { IdUsuario = 3, Nombre = "Kevin Manjarrez", IdRol = (int)RolUsuarioEnum.Tecnico },
                new Usuario { IdUsuario = 4, Nombre = "Ana Lopez", IdRol = (int)RolUsuarioEnum.Tecnico });
            await db.SaveChangesAsync();
        }

        var factory = new Mock<IDbContextFactory<HSisDbContext>>();
        factory.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((CancellationToken _) => new HSisDbContext(options));
        var destinatarios = new Mock<INotificacionDestinatariosService>();
        destinatarios.Setup(d => d.ObtenerDestinatariosAsignacionAsync(It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([1]);
        var coordinador = new NotificacionTicketCoordinator(destinatarios.Object, factory.Object);

        var asignacion = await coordinador.DetectarCambiosAsync(
            new Ticket { IdTicket = 1023, IdUsuario = 9 },
            new Ticket { IdTicket = 1023, IdUsuario = 9, IdTecnico = 3 });
        var reasignacion = await coordinador.DetectarCambiosAsync(
            new Ticket { IdTicket = 1023, IdUsuario = 9, IdTecnico = 3 },
            new Ticket { IdTicket = 1023, IdUsuario = 9, IdTecnico = 4 });

        asignacion.Single().Mensaje.Should().Contain("Kevin Manjarrez").And.NotContain("#3");
        reasignacion.Single().Mensaje.Should().Contain("Kevin Manjarrez").And.Contain("Ana Lopez");
    }
}