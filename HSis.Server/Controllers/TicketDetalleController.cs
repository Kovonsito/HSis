using HSis.Logic.DTOs;
using HSis.Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace HSis.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketDetalleController(ITicketDetalleService ticketDetalleService) : ControllerBase
    {

        [HttpGet("ticket/{idTicket}")]
        public async Task<ActionResult<List<TicketDetalleDto>>> ObtenerDetallesTicket(int idTicket)
        {
            return Ok(await ticketDetalleService.ObtenerDetallesTicketAsync(idTicket));
        }


        [HttpGet("ticket/{idTicket}/material/{idMaterial}")]
        public async Task<ActionResult<TicketDetalleDto>> ObtenerDetallePorId(int idTicket, int idMaterial)
        {
            var detalle = await ticketDetalleService.ObtenerDetallePorIdAsync(idTicket, idMaterial);
            if (detalle == null) return NotFound();
            return Ok(detalle);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarMaterialATicket([FromBody] TicketDetalleDto detTicketDto)
        {
            await ticketDetalleService.AgregarMaterialATicketAsync(detTicketDto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> ActualizarDetalleTicket([FromBody] TicketDetalleDto detTicketDto)
        {
            await ticketDetalleService.ActualizarDetalleTicketAsync(detTicketDto);
            return NoContent();
        }

        [HttpDelete("ticket/{idTicket}/material/{idMaterial}")]
        public async Task<IActionResult> EliminarMaterialDeTicket(int idTicket, int idMaterial)
        {
            await ticketDetalleService.EliminarMaterialDeTicketAsync(idTicket, idMaterial);
            return NoContent();
        }

        [HttpGet("ticket/{idTicket}/costo-total")]
        public async Task<ActionResult<decimal>> ObtenerCostoTotalMaterialesTicket(int idTicket)
        {
            return Ok(await ticketDetalleService.ObtenerCostoTotalMaterialesTicketAsync(idTicket));
        }

    }
}
