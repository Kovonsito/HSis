namespace HSis.Contracts.Services
{
    public interface IClienteSignalRNotificaciones : INotificadorTicket
    {
        event Action<string, int, string>? OnNotificationReceived;
        event Action? OnConnected;
        event Action? OnDisconnected;
        event Action? OnReconnecting;
        event Action<string?>? OnReconnected;

        bool IsConnected { get; }

        Task IniciarAsync(int userId, string role);
        Task DetenerAsync();
        Task NotificarCambioEstatusTicketAsync(int clientUserId, int ticketId, string ticketFolio, string newStatus);
        Task NotificarCalificacionTicketAsync(int technicianUserId, int ticketId, string ticketFolio, int rating, string comment);
    }
}
