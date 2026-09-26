using FluentAssertions;
using HSis.Contracts.Constants;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.Server.Hubs;
using HSis.Server.Services;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Xunit;

namespace HSis.Tests.Server;

public sealed class ServerNotificationDispatcherTests
{
    [Fact]
    public async Task NotifyTicketCreatedDebeEnviarPayloadCanonicoATecnicosYAdministradores()
    {
        var hub = CrearHubMock(out var clients, out var grupos);
        var dispatcher = new ServerNotificationDispatcher(hub.Object);

        await dispatcher.NotifyTicketCreatedAsync(42, "TCK-00042", "Sin acceso");

        grupos.Verify(item => item.ReceiveNotification(It.Is<NotificacionDto>(notification =>
            notification.IdNotificacion == 0
            && notification.TicketId == 42
            && notification.Tipo == ConstantesTiposNotificacion.NuevoTicket
            && notification.Mensaje.Contains("42"))), Times.Once);
    }

    [Fact]
    public async Task NotifyTicketStatusChangedDebeEnviarAlGrupoDelUsuario()
    {
        var hub = CrearHubMock(out var clients, out var grupos);
        var dispatcher = new ServerNotificationDispatcher(hub.Object);

        await dispatcher.NotifyTicketStatusChangedAsync(7, 42, "TCK-00042", "Cerrado");

        grupos.Verify(item => item.ReceiveNotification(It.Is<NotificacionDto>(notification =>
            notification.TicketId == 42
            && notification.Tipo == ConstantesTiposNotificacion.EstadoTicket
            && notification.Mensaje.Contains("Cerrado"))), Times.Once);
        clients.Verify(item => item.Group(GruposNotificaciones.Usuario(7)), Times.Once);
    }

    [Fact]
    public async Task NotifyTicketRatedDebeEnviarATecnicoYAdministradoresSinGrupoTecnicoSiNoExiste()
    {
        var hub = CrearHubMock(out var clients, out var grupos);
        var dispatcher = new ServerNotificationDispatcher(hub.Object);

        await dispatcher.NotifyTicketRatedAsync(7, 42, "TCK-00042", 5, "Excelente");

        grupos.Verify(item => item.ReceiveNotification(It.Is<NotificacionDto>(notification =>
            notification.Tipo == ConstantesTiposNotificacion.Calificacion
            && notification.Mensaje.Contains("Excelente"))), Times.Exactly(2));
        clients.Verify(item => item.Group(GruposNotificaciones.Usuario(7)), Times.Once);
        clients.Verify(item => item.Group(GruposNotificaciones.Administradores), Times.Once);
    }

    [Fact]
    public async Task NotifyTicketChangeDebeEnviarSoloALosUsuariosIndicados()
    {
        var hub = CrearHubMock(out var clients, out var grupos);
        var dispatcher = new ServerNotificationDispatcher(hub.Object);

        await dispatcher.NotifyTicketChangeAsync(
            [7, 8, 7],
            42,
            ConstantesTiposNotificacion.PrioridadTicket,
            "La prioridad cambió a Alta.");

        clients.Verify(item => item.Groups(It.Is<IReadOnlyList<string>>(groups =>
            groups.SequenceEqual(new[]
            {
                GruposNotificaciones.Usuario(7),
                GruposNotificaciones.Usuario(8)
            }))), Times.Once);
        grupos.Verify(item => item.ReceiveNotification(It.Is<NotificacionDto>(notification =>
            notification.TicketId == 42
            && notification.Tipo == ConstantesTiposNotificacion.PrioridadTicket
            && notification.Mensaje.Contains("Alta"))), Times.Once);
    }

    private static Mock<IHubContext<NotificationHub, INotificationClient>> CrearHubMock(
        out Mock<IHubClients<INotificationClient>> clients,
        out Mock<INotificationClient> grupos)
    {
        clients = new Mock<IHubClients<INotificationClient>>();
        grupos = new Mock<INotificationClient>();
        clients.Setup(item => item.Groups(It.IsAny<IReadOnlyList<string>>())).Returns(grupos.Object);
        clients.Setup(item => item.Group(It.IsAny<string>())).Returns(grupos.Object);
        grupos.Setup(item => item.ReceiveNotification(It.IsAny<NotificacionDto>()))
            .Returns(Task.CompletedTask);

        var hub = new Mock<IHubContext<NotificationHub, INotificationClient>>();
        hub.SetupGet(item => item.Clients).Returns(clients.Object);
        return hub;
    }
}
