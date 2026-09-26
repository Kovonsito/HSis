using FluentAssertions;
using HSis.Contracts.Constants;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HSis.Tests.Integration;

[Collection(NotificationServerCollection.Name)]
public sealed class NotificationPipelineSignalRIntegrationTests(NotificationServerFixture fixture)
{
    [Fact]
    public async Task HubSinTokenDebeRechazarLaConexion()
    {
        await using var connection = fixture.CrearConexion(null);

        var exception = await Record.ExceptionAsync(() => connection.StartAsync());

        exception.Should().NotBeNull();
        connection.State.Should().Be(HubConnectionState.Disconnected);
    }

    [Fact]
    public async Task DispatcherDebeEntregarCambioDeEstadoSoloAlUsuarioAutorizado()
    {
        await fixture.ReiniciarDatosAsync();
        await using var cliente = fixture.CrearConexion(
            fixture.CrearToken(100, RolUsuarioEnum.Cliente));
        await using var tecnico = fixture.CrearConexion(
            fixture.CrearToken(200, RolUsuarioEnum.Tecnico));

        var notificacionCliente = CrearReceptor(cliente);
        var notificacionTecnico = CrearReceptor(tecnico);

        await cliente.StartAsync();
        await tecnico.StartAsync();

        var dispatcher = fixture.Services.GetRequiredService<IServerNotificationDispatcher>();
        await dispatcher.NotifyTicketStatusChangedAsync(
            100,
            42,
            "TCK-00042",
            ConstantesEstatus.EN_PROCESO);

        var recibida = await notificacionCliente.Task.WaitAsync(TimeSpan.FromSeconds(5));
        recibida.TicketId.Should().Be(42);
        recibida.Tipo.Should().Be(ConstantesTiposNotificacion.EstadoTicket);
        recibida.Mensaje.Should().Contain(ConstantesEstatus.EN_PROCESO);
        (await EsperarRecepcionAsync(notificacionTecnico.Task)).Should().BeFalse();
    }

    [Fact]
    public async Task DispatcherDebeEntregarNuevoTicketALosGruposDeTecnicosYAdministradores()
    {
        await fixture.ReiniciarDatosAsync();
        await using var cliente = fixture.CrearConexion(
            fixture.CrearToken(100, RolUsuarioEnum.Cliente));
        await using var tecnico = fixture.CrearConexion(
            fixture.CrearToken(200, RolUsuarioEnum.Tecnico));
        await using var administrador = fixture.CrearConexion(
            fixture.CrearToken(300, RolUsuarioEnum.Administrador));

        var notificacionCliente = CrearReceptor(cliente);
        var notificacionTecnico = CrearReceptor(tecnico);
        var notificacionAdministrador = CrearReceptor(administrador);

        await cliente.StartAsync();
        await tecnico.StartAsync();
        await administrador.StartAsync();

        var dispatcher = fixture.Services.GetRequiredService<IServerNotificationDispatcher>();
        await dispatcher.NotifyTicketCreatedAsync(42, "TCK-00042", "Sin acceso");

        var recibidaPorTecnico = await notificacionTecnico.Task.WaitAsync(TimeSpan.FromSeconds(5));
        var recibidaPorAdministrador = await notificacionAdministrador.Task.WaitAsync(TimeSpan.FromSeconds(5));

        recibidaPorTecnico.TicketId.Should().Be(42);
        recibidaPorTecnico.Tipo.Should().Be(ConstantesTiposNotificacion.NuevoTicket);
        recibidaPorAdministrador.Should().BeEquivalentTo(recibidaPorTecnico);
        (await EsperarRecepcionAsync(notificacionCliente.Task)).Should().BeFalse();
    }

    private static TaskCompletionSource<NotificacionDto> CrearReceptor(HubConnection connection)
    {
        var receptor = new TaskCompletionSource<NotificacionDto>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        connection.On<NotificacionDto>(
            "ReceiveNotification",
            notification =>
            {
                receptor.TrySetResult(notification);
                return Task.CompletedTask;
            });
        return receptor;
    }

    private static async Task<bool> EsperarRecepcionAsync(
        Task<NotificacionDto> task,
        int milisegundos = 750)
    {
        var tareaFinalizada = await Task.WhenAny(task, Task.Delay(milisegundos));
        return tareaFinalizada == task;
    }
}
