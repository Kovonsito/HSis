using HSis.Contracts.DTOs;

namespace HSis.Contracts.Services;

public interface IClienteSignalRNotificaciones
{
    event Action<NotificacionDto>? OnNotificationReceived;
    event Action? OnConnected;
    event Action? OnDisconnected;
    event Action? OnReconnecting;
    event Action<string?>? OnReconnected;

    bool IsConnected { get; }

    Task IniciarAsync(string token);
    Task DetenerAsync();
}
