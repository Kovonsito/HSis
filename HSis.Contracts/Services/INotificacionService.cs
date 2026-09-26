using HSis.Contracts.DTOs;

namespace HSis.Contracts.Services;

public interface INotificacionService
{
    Task<NotificacionesResumenDto> ObtenerResumenAsync(
        int usuarioId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<NotificacionDto>> ObtenerNuevasAsync(
        int usuarioId,
        int desdeId,
        CancellationToken cancellationToken = default);

    Task<bool> MarcarComoLeidaAsync(
        int usuarioId,
        int notificacionId,
        CancellationToken cancellationToken = default);

    Task<int> MarcarTodasComoLeidasAsync(
        int usuarioId,
        CancellationToken cancellationToken = default);

    Task<int> LimpiarTodasAsync(
        int usuarioId,
        CancellationToken cancellationToken = default);
}
