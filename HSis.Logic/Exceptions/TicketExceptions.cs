using HSis.Contracts.Errors;

namespace HSis.Logic.Exceptions;

public sealed class TicketNotFoundException(int idTicket) : DomainException(
    ApiErrorCodes.TicketNotFound,
    "No se encontró el ticket solicitado. Puede que haya sido eliminado o que ya no esté disponible.",
    $"No se encontró el ticket con IdTicket={idTicket}.")
{
    public int IdTicket { get; } = idTicket;
}

public sealed class TicketRatingInvalidException(int calificacion) : DomainException(
    ApiErrorCodes.TicketRatingInvalid,
    "La calificación debe ser un valor entre 1 y 5. Seleccione una calificación válida e inténtelo de nuevo.",
    $"Se recibió una calificación fuera de rango: {calificacion}. El rango permitido es de 1 a 5.")
{
    public int Calificacion { get; } = calificacion;
}

public sealed class TicketNotClosedException(int idTicket) : DomainException(
    ApiErrorCodes.TicketNotClosed,
    "El ticket todavía no está cerrado. Solo puede calificarse un ticket cuando la atención haya finalizado.",
    $"Se intentó calificar el ticket con IdTicket={idTicket}, pero su estatus actual no es Cerrado.")
{
    public int IdTicket { get; } = idTicket;
}
