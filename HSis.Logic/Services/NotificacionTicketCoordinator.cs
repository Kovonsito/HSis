using HSis.Contracts.Constants;
using HSis.Contracts.Services;
using HSis.Data.Models;

namespace HSis.Logic.Services;

public sealed record NotificacionTicketPendiente(
    int TicketId,
    string Tipo,
    string Mensaje,
    IReadOnlyList<int> Destinatarios);

public sealed class NotificacionTicketCoordinator(
    INotificacionDestinatariosService destinatariosService)
{
    public async Task<IReadOnlyList<NotificacionTicketPendiente>> DetectarCambiosAsync(
        Ticket ticketAnterior,
        Ticket ticketActual,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ticketAnterior);
        ArgumentNullException.ThrowIfNull(ticketActual);

        if (ticketAnterior.IdTicket != ticketActual.IdTicket)
        {
            throw new ArgumentException("Los estados comparados deben pertenecer al mismo ticket.", nameof(ticketActual));
        }

        var estatusCambio = !StringEquals(ticketAnterior.Estatus, ticketActual.Estatus);
        var tecnicoCambio = ticketAnterior.IdTecnico != ticketActual.IdTecnico;
        var prioridadCambio = !StringEquals(ticketAnterior.Prioridad, ticketActual.Prioridad);
        var solucionCambio = !StringEquals(ticketAnterior.Solucion, ticketActual.Solucion);

        if (!estatusCambio && !tecnicoCambio && !prioridadCambio && !solucionCambio)
        {
            return Array.Empty<NotificacionTicketPendiente>();
        }

        var eventos = new List<NotificacionTicketPendiente>();

        if (tecnicoCambio)
        {
            var destinatarios = await destinatariosService.ObtenerDestinatariosAsignacionAsync(
                ticketActual.IdUsuario,
                ticketAnterior.IdTecnico,
                ticketActual.IdTecnico,
                cancellationToken);
            var tipo = ticketAnterior.IdTecnico is > 0 && ticketActual.IdTecnico is > 0
                ? ConstantesTiposNotificacion.ReasignacionTicket
                : ConstantesTiposNotificacion.AsignacionTicket;
            var mensaje = ticketAnterior.IdTecnico is > 0 && ticketActual.IdTecnico is > 0
                ? NotificacionFactory.CrearMensajeReasignacion(
                    ticketActual.IdTicket,
                    ticketActual.IdTicket.ToString(),
                    FormatearTecnico(ticketAnterior.IdTecnico),
                    FormatearTecnico(ticketActual.IdTecnico))
                : NotificacionFactory.CrearMensajeAsignacion(
                    ticketActual.IdTicket,
                    ticketActual.IdTicket.ToString(),
                    FormatearTecnico(ticketActual.IdTecnico));

            eventos.Add(CrearEvento(ticketActual, tipo, mensaje, destinatarios));
        }

        if (prioridadCambio)
        {
            var destinatarios = await destinatariosService.ObtenerDestinatariosCambioPrioridadAsync(
                ticketActual.IdUsuario,
                ticketActual.IdTecnico,
                cancellationToken);
            eventos.Add(CrearEvento(
                ticketActual,
                ConstantesTiposNotificacion.PrioridadTicket,
                NotificacionFactory.CrearMensajeCambioPrioridad(
                    ticketActual.IdTicket,
                    ticketActual.IdTicket.ToString(),
                    ticketAnterior.Prioridad,
                    ticketActual.Prioridad),
                destinatarios));
        }

        if (estatusCambio)
        {
            var destinatarios = await destinatariosService.ObtenerDestinatariosCambioEstadoAsync(
                ticketActual.IdUsuario,
                ticketActual.IdTecnico,
                cancellationToken);
            eventos.Add(CrearEvento(
                ticketActual,
                ConstantesTiposNotificacion.EstadoTicket,
                NotificacionFactory.CrearMensajeCambioEstado(
                    ticketActual.IdTicket,
                    ticketActual.IdTicket.ToString(),
                    ticketActual.Estatus ?? string.Empty),
                destinatarios));
        }

        if (solucionCambio)
        {
            var destinatarios = await destinatariosService.ObtenerDestinatariosActualizacionSolucionAsync(
                ticketActual.IdUsuario,
                ticketActual.IdTecnico,
                cancellationToken);
            eventos.Add(CrearEvento(
                ticketActual,
                ConstantesTiposNotificacion.SolucionTicket,
                NotificacionFactory.CrearMensajeSolucion(
                    ticketActual.IdTicket,
                    ticketActual.IdTicket.ToString(),
                    ticketActual.Solucion),
                destinatarios));
        }

        return eventos;
    }

    private static NotificacionTicketPendiente CrearEvento(
        Ticket ticket,
        string tipo,
        string mensaje,
        IReadOnlyList<int> destinatarios)
        => new(ticket.IdTicket, tipo, mensaje, destinatarios);

    private static string? FormatearTecnico(int? idTecnico)
        => idTecnico is > 0 ? $"técnico #{idTecnico.Value}" : null;

    private static bool StringEquals(string? left, string? right)
        => string.Equals(left?.Trim(), right?.Trim(), StringComparison.Ordinal);
}
