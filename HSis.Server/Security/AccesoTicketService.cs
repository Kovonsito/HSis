using System.Security.Claims;
using HSis.Contracts.Constants;
using HSis.Contracts.Services;

namespace HSis.Server.Security;

public enum AlcanceTicket
{
    // Administrador; tecnico asignado o con ticket sin asignar; cliente solo si es el solicitante.
    Consulta,
    // Administrador; tecnico asignado o con ticket sin asignar (para tomarlo). Nunca el cliente.
    Gestion,
    // Administrador; tecnico unicamente si el ticket esta asignado a el.
    Materiales,
    // Solo el solicitante del ticket (o el administrador).
    Solicitante
}

public sealed class AccesoTicketService(ITicketService ticketService)
{
    public async Task<bool> PermiteAsync(ClaimsPrincipal usuario, int idTicket, AlcanceTicket alcance)
    {
        var rol = usuario.ObtenerRol();
        if (rol == RolUsuarioEnum.Administrador)
        {
            return true;
        }

        var idUsuario = usuario.ObtenerIdUsuario();
        if (rol is null || idUsuario == 0)
        {
            return false;
        }

        var ticket = await ticketService.ObtenerTicketPorIdAsync(idTicket);
        if (ticket is null)
        {
            return false;
        }

        return rol switch
        {
            RolUsuarioEnum.Tecnico => alcance switch
            {
                AlcanceTicket.Materiales => ticket.IdTecnico == idUsuario,
                AlcanceTicket.Solicitante => ticket.IdUsuario == idUsuario,
                _ => ticket.IdTecnico == idUsuario || ticket.IdTecnico is null
            },
            RolUsuarioEnum.Cliente => alcance is AlcanceTicket.Consulta or AlcanceTicket.Solicitante
                && ticket.IdUsuario == idUsuario,
            _ => false
        };
    }
}