using HSis.Contracts.DTOs;

namespace HSis.Contracts.Services;

/// <summary>
/// Contrato de mensajes que el servidor puede enviar a un cliente SignalR.
/// </summary>
public interface INotificationClient
{
    Task ReceiveNotification(NotificacionDto notification);
}
