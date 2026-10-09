using HSis.Contracts.DTOs;
using HSis.Contracts.Errors;
using HSis.Contracts.Constants;
using HSis.Contracts.Services;
using HSis.Server.Configurations;
using HSis.Server.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HSis.Server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class TicketsController(ITicketService ticketService, AccesoTicketService accesoTicket) : ControllerBase
    {

        [Authorize(Policy = PoliticasAutorizacion.Administrador)]
        [HttpGet]
        public async Task<ActionResult<List<TicketDto>>> ObtenerTickets()
        {
            return Ok(await ticketService.ObtenerTicketsAsync());
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<TicketDto>> ObtenerTicketPorId(int id)
        {
            if (!await accesoTicket.PermiteAsync(User, id, AlcanceTicket.Consulta))
            {
                return Forbid();
            }

            var ticket = await ticketService.ObtenerTicketPorIdAsync(id);
            if (ticket == null)
            {
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Ticket no encontrado",
                    detail: "No se encontró el ticket solicitado. Puede que haya sido eliminado o que ya no esté disponible.",
                    extensions: new Dictionary<string, object?>
                    {
                        ["code"] = ApiErrorCodes.TicketNotFound,
                        ["traceId"] = HttpContext.TraceIdentifier
                    });
            }
            return Ok(ticket);
        }

        [HttpPost]
        public async Task<ActionResult<TicketDto>> CrearTicket([FromBody] TicketCreateDto ticketDto)
        {
            // El cliente solo registra tickets propios, sin asignar tecnico ni prioridad.
            if (User.ObtenerRol() == RolUsuarioEnum.Cliente &&
                (ticketDto.IdUsuario != User.ObtenerIdUsuario() || ticketDto.IdTecnico is not null || ticketDto.Prioridad is not null))
            {
                return Forbid();
            }

            var ticket = await ticketService.CrearTicketAsync(ticketDto);
            return CreatedAtAction(nameof(ObtenerTicketPorId), new { id = ticket.IdTicket }, ticket);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarTicket(int id, [FromBody] TicketUpdateDto ticketDto)
        {
            if (id != ticketDto.IdTicket)
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Identificador de ticket no válido",
                    detail: "El identificador de la ruta no coincide con el identificador incluido en los datos enviados.",
                    extensions: new Dictionary<string, object?>
                    {
                        ["code"] = ApiErrorCodes.BadRequest,
                        ["traceId"] = HttpContext.TraceIdentifier
                    });
            }
            if (!await accesoTicket.PermiteAsync(User, id, AlcanceTicket.Gestion))
            {
                return Forbid();
            }

            // Un tecnico solo puede dejar el ticket sin asignar o asignado a si mismo.
            if (!User.EsAdministrador() && ticketDto.IdTecnico is not null && ticketDto.IdTecnico != User.ObtenerIdUsuario())
            {
                return Forbid();
            }

            await ticketService.ActualizarTicketAsync(ticketDto);
            return NoContent();
        }

        [HttpGet("usuario/{idUsuario}")]
        public async Task<ActionResult<List<TicketDto>>> ObtenerTicketsPorUsuario(int idUsuario)
        {
            if (!User.EsAdministradorOMismo(idUsuario))
            {
                return Forbid();
            }

            return Ok(await ticketService.ObtenerTicketsPorUsuarioAsync(idUsuario));
        }

        [HttpGet("tecnico/{idTecnico}/asignados")]
        [Authorize(Policy = PoliticasAutorizacion.PersonalAtencion)]
        public async Task<ActionResult<List<TicketDto>>> ObtenerTicketsAsignados(int idTecnico)
        {
            if (!User.EsAdministradorOMismo(idTecnico))
            {
                return Forbid();
            }

            return Ok(await ticketService.ObtenerTicketsAsignadosATecnicoAsync(idTecnico));
        }

        [HttpPost("filtrar")]
        [Authorize(Policy = PoliticasAutorizacion.Administrador)]
        public async Task<ActionResult<PaginatedResultDto<TicketDto>>> FiltrarPaginados(
            [FromBody] TicketFilterDto filtros,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50)
        {
            return Ok(await ticketService.ObtenerTicketsFiltradosPaginadosAsync(filtros, pageNumber, pageSize));
        }

        [HttpGet("{id}/historial")]
        public async Task<ActionResult<List<HistorialCambiosDto>>> ObtenerHistorial(int id)
        {
            if (!await accesoTicket.PermiteAsync(User, id, AlcanceTicket.Gestion))
            {
                return Forbid();
            }

            return Ok(await ticketService.ObtenerHistorialPorTicketAsync(id));
        }


        [HttpPost("{id}/calificar")]
        public async Task<IActionResult> CalificarTicket(int id, [FromBody] CalificarTicketRequest request)
        {
            if (!await accesoTicket.PermiteAsync(User, id, AlcanceTicket.Solicitante))
            {
                return Forbid();
            }

            var exito = await ticketService.RegistrarCalificacionAsync(id, request.Calificacion, request.Comentario);
            if (!exito)
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "No se pudo registrar la calificación",
                    detail: "No se pudo guardar la calificación del ticket. Compruebe que el ticket esté disponible e inténtelo de nuevo.",
                    extensions: new Dictionary<string, object?>
                    {
                        ["code"] = ApiErrorCodes.BadRequest,
                        ["traceId"] = HttpContext.TraceIdentifier
                    });
            }
            return Ok();
        }

        [HttpGet("disponibles")]
        [Authorize(Policy = PoliticasAutorizacion.PersonalAtencion)]
        public async Task<ActionResult<List<TicketDto>>> ObtenerTicketsDisponibles()
        {
            return Ok(await ticketService.ObtenerTicketsDisponiblesAsync());
        }


        [HttpGet("tecnico/{idTecnico}/cerrados")]
        [Authorize(Policy = PoliticasAutorizacion.PersonalAtencion)]
        public async Task<ActionResult<List<TicketDto>>> ObtenerTicketsCerradosPorTecnico(int idTecnico)
        {
            if (!User.EsAdministradorOMismo(idTecnico))
            {
                return Forbid();
            }

            return Ok(await ticketService.ObtenerTicketsCerradosPorTecnicoAsync(idTecnico));
        }


        [HttpGet("tecnico/{idTecnico}/promedio-calificacion")]
        [Authorize(Policy = PoliticasAutorizacion.PersonalAtencion)]
        public async Task<ActionResult<double>> ObtenerPromedioCalificacionTecnico(int idTecnico)
        {
            if (!User.EsAdministradorOMismo(idTecnico))
            {
                return Forbid();
            }

            return Ok(await ticketService.ObtenerPromedioCalificacionTecnicoAsync(idTecnico));
        }


        [HttpGet("tecnico/{idTecnico}/feedback")]
        [Authorize(Policy = PoliticasAutorizacion.PersonalAtencion)]
        public async Task<ActionResult<List<TicketDto>>> ObtenerFeedbackTecnico(int idTecnico)
        {
            if (!User.EsAdministradorOMismo(idTecnico))
            {
                return Forbid();
            }

            return Ok(await ticketService.ObtenerFeedbackTecnicoAsync(idTecnico));
        }


        [HttpGet("sla")]
        [Authorize(Policy = PoliticasAutorizacion.Administrador)]
        public async Task<ActionResult<List<TicketDto>>> ObtenerTicketsPorSLA([FromQuery] bool esUrgente)
        {
            return Ok(await ticketService.ObtenerTicketsPorSLAAsync(esUrgente));
        }


        [HttpGet("sla/count")]
        [Authorize(Policy = PoliticasAutorizacion.Administrador)]
        public async Task<ActionResult<int>> ObtenerCountTicketsPorSLA([FromQuery] bool esUrgente)
        {
            return Ok(await ticketService.ObtenerCountTicketsPorSLAAsync(esUrgente));
        }


        [HttpGet("estatus/{estatus}")]
        [Authorize(Policy = PoliticasAutorizacion.Administrador)]
        public async Task<ActionResult<List<TicketDto>>> ObtenerTicketsPorEstatus(string estatus)
        {
            return Ok(await ticketService.ObtenerTicketsPorEstatusAsync(estatus));
        }


        [HttpGet("estatus/{estatus}/count")]
        [Authorize(Policy = PoliticasAutorizacion.Administrador)]
        public async Task<ActionResult<int>> ObtenerCountTicketsPorEstatus(string estatus)
        {
            return Ok(await ticketService.ObtenerCountTicketsPorEstatusAsync(estatus));
        }


        [HttpPost("filtrar/todos")]
        [Authorize(Policy = PoliticasAutorizacion.Administrador)]
        public async Task<ActionResult<List<TicketDto>>> ObtenerTicketsFiltrados([FromBody] TicketFilterDto filtros)
        {
            return Ok(await ticketService.ObtenerTicketsFiltradosAsync(filtros));
        }


        [HttpGet("kpis")]
        [Authorize(Policy = PoliticasAutorizacion.Administrador)]
        public async Task<ActionResult<ReporteKpisDto>> ObtenerReporteKpis([FromQuery] DateTime inicio, [FromQuery] DateTime fin)
        {
            return Ok(await ticketService.ObtenerReporteKpisAsync(inicio, fin));
        }

        [HttpGet("dashboard-resumen")]
        [Authorize(Policy = PoliticasAutorizacion.Administrador)]
        public async Task<ActionResult<DashboardResumenDto>> ObtenerResumenDashboard([FromQuery] int? idTecnico = null)
        {
            return Ok(await ticketService.ObtenerResumenDashboardAsync(idTecnico));
        }

        [HttpGet("tecnico/{idTecnico}/indicadores")]
        [Authorize(Policy = PoliticasAutorizacion.PersonalAtencion)]
        public async Task<ActionResult<IndicadoresTecnicoDto>> ObtenerIndicadoresTecnico(int idTecnico)
        {
            if (!User.EsAdministradorOMismo(idTecnico))
            {
                return Forbid();
            }

            return Ok(await ticketService.ObtenerIndicadoresTecnicoAsync(idTecnico));
        }

        [HttpGet("usuario/{idUsuario}/resumen")]
        public async Task<ActionResult<ResumenClienteDto>> ObtenerResumenCliente(int idUsuario)
        {
            if (!User.EsAdministradorOMismo(idUsuario))
            {
                return Forbid();
            }

            return Ok(await ticketService.ObtenerResumenClienteAsync(idUsuario));
        }
    }

    public class CalificarTicketRequest
    {
        public int Calificacion { get; set; }
        public string? Comentario { get; set; }
    }
}

