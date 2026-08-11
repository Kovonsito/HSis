namespace HSis.Logic.Services

{
    public interface INotificacionStorageService
    {
        Task<List<NotificacionLocal>> ObtenerNotificacionesAsync(int userId);
        Task GuardarNotificacionAsync(int userId, int ticketId, string mensaje);
        Task MarcarComoLeidaAsync(int userId, Guid id);
        Task MarcarTodasComoLeidasAsync(int userId);
        Task LimpiarTodasAsync(int userId);
        Task SincronizarDesdeBDAsync(int userId);
    }
}
