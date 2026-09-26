namespace HSis.Contracts.DTOs;

/// <summary>
/// Contiene el historial visible y el total actual de notificaciones no leídas.
/// </summary>
public sealed record NotificacionesResumenDto(
    IReadOnlyList<NotificacionDto> Notificaciones,
    int NoLeidas);
