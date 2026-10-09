using HSis.Contracts.DTOs;
using HSis.Contracts.Errors;
using HSis.Contracts.Services;
using HSis.Server.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HSis.Server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class TicketDetalleController(ITicketDetalleService ticketDetalleService, AccesoTicketService accesoTicket) : ControllerBase
    {

        [HttpGet("ticket/{idTicket}")]
        public async Task<ActionResult<List<TicketDetalleDto>>> ObtenerDetallesTicket(int idTicket)
        {
            if (!await accesoTicket.PermiteAsync(User, idTicket, AlcanceTicket.Gestion))
            {
                return Forbid();
            }

            return Ok(await ticketDetalleService.ObtenerDetallesTicketAsync(idTicket));
        }


        [HttpGet("ticket/{idTicket}/material/{idMaterial}")]
        public async Task<ActionResult<TicketDetalleDto>> ObtenerDetallePorId(int idTicket, int idMaterial)
        {
            if (!await accesoTicket.PermiteAsync(User, idTicket, AlcanceTicket.Gestion))
            {
                return Forbid();
            }

            var detalle = await ticketDetalleService.ObtenerDetallePorIdAsync(idTicket, idMaterial);
            if (detalle == null)
            {
                return Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Material del ticket no encontrado",
                    detail: "No se encontró el material asociado al ticket solicitado.",
                    extensions: new Dictionary<string, object?>
                    {
                        ["code"] = ApiErrorCodes.ResourceNotFound,
                        ["traceId"] = HttpContext.TraceIdentifier
                    });
            }
            return Ok(detalle);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarMaterialATicket([FromBody] TicketDetalleDto detTicketDto)
        {
            if (!await accesoTicket.PermiteAsync(User, detTicketDto.IdTicket, AlcanceTicket.Materiales))
            {
                return Forbid();
            }

            await ticketDetalleService.AgregarMaterialATicketAsync(detTicketDto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> ActualizarDetalleTicket([FromBody] TicketDetalleDto detTicketDto)
        {
            if (!await accesoTicket.PermiteAsync(User, detTicketDto.IdTicket, AlcanceTicket.Materiales))
            {
                return Forbid();
            }

            await ticketDetalleService.ActualizarDetalleTicketAsync(detTicketDto);
            return NoContent();
        }

        [HttpDelete("ticket/{idTicket}/material/{idMaterial}")]
        public async Task<IActionResult> EliminarMaterialDeTicket(int idTicket, int idMaterial)
        {
            if (!await accesoTicket.PermiteAsync(User, idTicket, AlcanceTicket.Materiales))
            {
                return Forbid();
            }

            await ticketDetalleService.EliminarMaterialDeTicketAsync(idTicket, idMaterial);
            return NoContent();
        }

        [HttpGet("ticket/{idTicket}/costo-total")]
        public async Task<ActionResult<decimal>> ObtenerCostoTotalMaterialesTicket(int idTicket)
        {
            if (!await accesoTicket.PermiteAsync(User, idTicket, AlcanceTicket.Gestion))
            {
                return Forbid();
            }

            return Ok(await ticketDetalleService.ObtenerCostoTotalMaterialesTicketAsync(idTicket));
        }

    }
}

