using HSis.Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace HSis.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaterialesController(IMaterialService materialService) : ControllerBase
    {

        [HttpPut("{idMaterial}/costo")]
        public async Task<IActionResult> ActualizarCosto(int idMaterial, [FromBody] decimal nuevoCosto)
        {
            await materialService.ActualizarCostoMaterialAsync(idMaterial, nuevoCosto);
            return NoContent();
        }
    }
}
