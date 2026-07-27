namespace HSis.Logic.Services

{
    public interface INotificationClientService
    {
        event Action<string, int, string>? OnNotificationReceived;
        event Action? OnConnected;
        event Action? OnDisconnected;
        event Action? OnReconnecting;
        event Action<string?>? OnReconnected;

        bool IsConnected { get; }

        Task IniciarAsync(int userId, string role);
        Task DetenerAsync();
        Task NotifyTicketCreatedAsync(int ticketId, string ticketFolio, string titulo);
        Task NotifyTicketStatusChangedAsync(int clientUserId, int ticketId, string ticketFolio, string newStatus);
        Task NotifyTicketRatedAsync(int technicianUserId, int ticketId, string ticketFolio, int rating, string comment);
    }
}
