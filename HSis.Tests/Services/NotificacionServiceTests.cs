using FluentAssertions;
using HSis.Contracts.Constants;
using HSis.Data.Models;
using HSis.Logic.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace HSis.Tests.Services;

public sealed class NotificacionServiceTests
{
    [Fact]
    public async Task ObtenerResumenAsyncDebeAislarPorUsuarioOrdenarYLimitarHistorial()
    {
        var options = CrearOpciones();
        var fechaBase = new DateTime(2026, 1, 1, 8, 0, 0);

        await using (var db = new HSisDbContext(options))
        {
            db.Usuarios.AddRange(
                new Usuario { IdUsuario = 1, Nombre = "Usuario 1" },
                new Usuario { IdUsuario = 2, Nombre = "Usuario 2" });

            db.Notificaciones.AddRange(
                Enumerable.Range(1, 51).Select(indice => new Notificacion
                {
                    IdNotificacion = indice,
                    UsuarioDestinoId = 1,
                    TicketId = indice,
                    Tipo = ConstantesTiposNotificacion.EstadoTicket,
                    Mensaje = $"Mensaje {indice}",
                    FechaCreacion = fechaBase.AddMinutes(indice),
                    Leido = indice % 2 == 0
                }));

            db.Notificaciones.Add(new Notificacion
            {
                IdNotificacion = 100,
                UsuarioDestinoId = 2,
                TicketId = 100,
                Tipo = ConstantesTiposNotificacion.NuevoTicket,
                Mensaje = "No visible",
                FechaCreacion = fechaBase.AddHours(2)
            });

            await db.SaveChangesAsync();
        }

        var resultado = await CrearServicio(options).ObtenerResumenAsync(1);

        resultado.Notificaciones.Should().HaveCount(50);
        resultado.Notificaciones.Should().BeInDescendingOrder(item => item.FechaCreacion);
        resultado.Notificaciones.Should().OnlyContain(item => item.TicketId >= 1 && item.TicketId <= 51);
        resultado.NoLeidas.Should().Be(26);
        resultado.Notificaciones.First().TicketId.Should().Be(51);
    }

    [Fact]
    public async Task MarcarComoLeidaAsyncDebeRespetarPropietarioYSerIdempotente()
    {
        var options = CrearOpciones();
        await SembrarNotificacionesAsync(options);
        var servicio = CrearServicio(options);

        var actualizada = await servicio.MarcarComoLeidaAsync(1, 1);
        var ajena = await servicio.MarcarComoLeidaAsync(2, 1);
        var repetida = await servicio.MarcarComoLeidaAsync(1, 1);

        actualizada.Should().BeTrue();
        ajena.Should().BeFalse();
        repetida.Should().BeTrue();

        await using var db = new HSisDbContext(options);
        (await db.Notificaciones.FindAsync(1))!.Leido.Should().BeTrue();
    }

    [Fact]
    public async Task ObtenerNuevasAsyncDebeFiltrarPorIdYPropietarioYOrdenarAscendente()
    {
        var options = CrearOpciones();
        var fechaBase = new DateTime(2026, 1, 1, 8, 0, 0);

        await using (var db = new HSisDbContext(options))
        {
            db.Notificaciones.AddRange(
                new Notificacion
                {
                    IdNotificacion = 1,
                    UsuarioDestinoId = 1,
                    Tipo = "Tipo",
                    Mensaje = "Anterior",
                    FechaCreacion = fechaBase
                },
                new Notificacion
                {
                    IdNotificacion = 3,
                    UsuarioDestinoId = 1,
                    Tipo = "Tipo",
                    Mensaje = "Posterior 3",
                    FechaCreacion = fechaBase.AddMinutes(3)
                },
                new Notificacion
                {
                    IdNotificacion = 2,
                    UsuarioDestinoId = 1,
                    Tipo = "Tipo",
                    Mensaje = "Posterior 2",
                    FechaCreacion = fechaBase.AddMinutes(2)
                },
                new Notificacion
                {
                    IdNotificacion = 4,
                    UsuarioDestinoId = 2,
                    Tipo = "Tipo",
                    Mensaje = "Ajena",
                    FechaCreacion = fechaBase.AddMinutes(4)
                });

            await db.SaveChangesAsync();
        }

        var resultado = await CrearServicio(options).ObtenerNuevasAsync(1, 1);

        resultado.Select(item => item.IdNotificacion).Should().Equal(2, 3);
        resultado.Should().OnlyContain(item => item.Mensaje.StartsWith("Posterior", StringComparison.Ordinal));
    }

    [Fact]
    public async Task MarcarTodasComoLeidasYLimpiarTodasSoloAfectanAlUsuario()
    {
        var options = CrearOpciones();
        await SembrarNotificacionesAsync(options);
        var servicio = CrearServicio(options);

        var marcadas = await servicio.MarcarTodasComoLeidasAsync(1);
        var eliminadas = await servicio.LimpiarTodasAsync(1);

        marcadas.Should().Be(2);
        eliminadas.Should().Be(3);

        await using var db = new HSisDbContext(options);
        (await db.Notificaciones.CountAsync(item => item.UsuarioDestinoId == 1)).Should().Be(0);
        (await db.Notificaciones.CountAsync(item => item.UsuarioDestinoId == 2)).Should().Be(1);
    }

    private static NotificacionService CrearServicio(DbContextOptions<HSisDbContext> options)
    {
        var factory = new Mock<IDbContextFactory<HSisDbContext>>();
        factory.Setup(item => item.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((CancellationToken _) => new HSisDbContext(options));
        factory.Setup(item => item.CreateDbContext())
            .Returns(() => new HSisDbContext(options));
        return new NotificacionService(factory.Object);
    }

    private static async Task SembrarNotificacionesAsync(DbContextOptions<HSisDbContext> options)
    {
        await using var db = new HSisDbContext(options);
        db.Usuarios.AddRange(
            new Usuario { IdUsuario = 1, Nombre = "Usuario 1" },
            new Usuario { IdUsuario = 2, Nombre = "Usuario 2" });
        db.Notificaciones.AddRange(
            new Notificacion { IdNotificacion = 1, UsuarioDestinoId = 1, Tipo = "Tipo", Mensaje = "Uno", FechaCreacion = DateTime.Now },
            new Notificacion { IdNotificacion = 2, UsuarioDestinoId = 1, Tipo = "Tipo", Mensaje = "Dos", FechaCreacion = DateTime.Now, Leido = true },
            new Notificacion { IdNotificacion = 3, UsuarioDestinoId = 1, Tipo = "Tipo", Mensaje = "Tres", FechaCreacion = DateTime.Now },
            new Notificacion { IdNotificacion = 4, UsuarioDestinoId = 2, Tipo = "Tipo", Mensaje = "Ajena", FechaCreacion = DateTime.Now });
        await db.SaveChangesAsync();
    }

    private static DbContextOptions<HSisDbContext> CrearOpciones()
        => new DbContextOptionsBuilder<HSisDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
}
