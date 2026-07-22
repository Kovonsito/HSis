using Microsoft.AspNetCore.Mvc;
using HSis.Logic.Services;

namespace HSis.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogosController(ICatalogoService catalogoService) : ControllerBase
    {

        [HttpGet("{entidad}")]
        public async Task<ActionResult> ObtenerTodos(string entidad)
        {
            var assembly = typeof(HSis.Data.Models.Ticket).Assembly;
            var type = assembly.GetTypes().FirstOrDefault(t => t.Name.Equals(entidad, StringComparison.OrdinalIgnoreCase));

            if (type == null) return NotFound(new { Message = $"Catálogo '{entidad}' no encontrado." });

            var data = await catalogoService.ObtenerTodosPorTipoAsync(type);
            return Ok(data);
        }
    }
}
