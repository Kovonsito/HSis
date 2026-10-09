using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.Server.Configurations;
using HSis.Server.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HSis.Server.Controllers
{
    [ApiController]
    [Authorize(Policy = PoliticasAutorizacion.PersonalAtencion)]
    [Route("api/[controller]")]
    public class MaterialesController(IMaterialService materialService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<MaterialDto>>> ObtenerTodos()
        {
            return Ok(RedaccionCostos.Para(User, await materialService.ObtenerMaterialesAsync()));
        }

        [HttpGet("{idMaterial}/kardex")]
        public async Task<ActionResult<List<KardexMovimientoDto>>> ObtenerKardex(int idMaterial)
        {
            return Ok(RedaccionCostos.Para(User, await materialService.ObtenerKardexPorMaterialAsync(idMaterial)));
        }

        [Authorize(Policy = PoliticasAutorizacion.Administrador)]
        [HttpPut("{idMaterial}/costo")]
        public async Task<IActionResult> ActualizarCosto(int idMaterial, [FromBody] decimal nuevoCosto)
        {
            await materialService.ActualizarCostoMaterialAsync(idMaterial, nuevoCosto);
            return NoContent();
        }

        [HttpPost("movimientos")]
        public async Task<IActionResult> RegistrarMovimiento([FromBody] KardexMovimientoDto movimiento)
        {
            await materialService.RegistrarMovimientoAsync(movimiento);
            return Ok();
        }
    }
}
