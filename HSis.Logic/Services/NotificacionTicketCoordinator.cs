using HSis.Contracts.Constants;
using HSis.Contracts.Services;
using HSis.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HSis.Logic.Services;

public sealed record NotificacionTicketPendiente(
    int TicketId,
    string Tipo,
    string Mensaje,
    IReadOnlyList<int> Destinatarios);

public sealed class NotificacionTicketCoordinator(
    INotificacionDestinatariosService destinatariosService,
    IDbContextFactory<HSisDbContext>? dbContextFactory = null)
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
            var nombreAnterior = await ObtenerNombreTecnicoAsync(ticketAnterior.IdTecnico, cancellationToken);
            var nombreActual = await ObtenerNombreTecnicoAsync(ticketActual.IdTecnico, cancellationToken);
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
                    nombreAnterior,
                    nombreActual)
                : NotificacionFactory.CrearMensajeAsignacion(
                    ticketActual.IdTicket,
                    ticketActual.IdTicket.ToString(),
                    nombreActual);

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

    // Se muestra el nombre del tecnico; el numero solo es respaldo si no se puede consultar.
    private async Task<string?> ObtenerNombreTecnicoAsync(int? idTecnico, CancellationToken cancellationToken)
    {
        if (idTecnico is not > 0)
        {
            return null;
        }

        if (dbContextFactory is not null)
        {
            await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            var nombre = await db.Usuarios
                .AsNoTracking()
                .Where(usuario => usuario.IdUsuario == idTecnico.Value)
                .Select(usuario => usuario.Nombre)
                .FirstOrDefaultAsync(cancellationToken);
            if (!string.IsNullOrWhiteSpace(nombre))
            {
                return nombre.Trim();
            }
        }

        return $"técnico #{idTecnico.Value}";
    }

    private static bool StringEquals(string? left, string? right)
        => string.Equals(left?.Trim(), right?.Trim(), StringComparison.Ordinal);
}
