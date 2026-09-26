using HSis.Contracts.DTOs;

namespace HSis.Contracts.Services;

public interface INotificacionesApiClient
{
    Task<NotificacionesResumenDto> ObtenerResumenAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<NotificacionDto>> ObtenerNuevasAsync(
        int desdeId,
        CancellationToken cancellationToken = default);

    Task MarcarComoLeidaAsync(
        int idNotificacion,
        CancellationToken cancellationToken = default);

    Task<int> MarcarTodasComoLeidasAsync(
        CancellationToken cancellationToken = default);

    Task LimpiarTodasAsync(
        CancellationToken cancellationToken = default);
}
