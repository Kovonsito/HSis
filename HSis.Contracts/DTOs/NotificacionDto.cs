namespace HSis.Contracts.DTOs;

/// <summary>
/// Representa una notificación persistida y entregable a la interfaz de usuario.
/// </summary>
public sealed record NotificacionDto(
    int IdNotificacion,
    int? TicketId,
    string Tipo,
    string Mensaje,
    DateTimeOffset FechaCreacion,
    bool Leido,
    int? MaterialId = null);
