namespace HSis.Contracts.Services;

public interface INotificacionDestinatariosService
{
    Task<IReadOnlyList<int>> ObtenerDestinatariosNuevoTicketAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<int>> ObtenerDestinatariosCalificacionAsync(
        int? tecnicoId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<int>> ObtenerDestinatariosCambioEstadoAsync(
        int usuarioPropietarioId,
        int? tecnicoId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<int>> ObtenerDestinatariosAsignacionAsync(
        int usuarioPropietarioId,
        int? tecnicoAnteriorId,
        int? tecnicoNuevoId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<int>> ObtenerDestinatariosCambioPrioridadAsync(
        int usuarioPropietarioId,
        int? tecnicoId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<int>> ObtenerDestinatariosActualizacionSolucionAsync(
        int usuarioPropietarioId,
        int? tecnicoId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<int>> ObtenerDestinatariosCambioMaterialTicketAsync(
        int usuarioPropietarioId,
        int? tecnicoId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<int>> ObtenerDestinatariosMovimientoMaterialAsync(
        CancellationToken cancellationToken = default);
}
