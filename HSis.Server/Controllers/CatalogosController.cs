using HSis.Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace HSis.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogosController(ICatalogoService catalogoService) : ControllerBase
    {

        [HttpGet("{entidad}")]
        public async Task<ActionResult> ObtenerTodos(string entidad)
        {
            var type = ObtenerTipoEntidad(entidad);
            if (type == null) return NotFound(new { Message = $"Catálogo '{entidad}' no encontrado." });

            var data = await catalogoService.ObtenerTodosPorTipoAsync(type);
            return Ok(data);
        }

        [HttpPost("{entidad}")]
        public async Task<ActionResult> Crear(string entidad, [FromBody] System.Text.Json.JsonElement payload)
        {
            var type = ObtenerTipoEntidad(entidad);
            if (type == null) return NotFound(new { Message = $"Catálogo '{entidad}' no encontrado." });

            object? dto = System.Text.Json.JsonSerializer.Deserialize(payload.GetRawText(), type);
            if (dto == null) return BadRequest(new { Message = "Cuerpo de solicitud inválido." });

            var metodo = typeof(ICatalogoService).GetMethod("CrearAsync")!.MakeGenericMethod(type);
            Task task = (Task)metodo.Invoke(catalogoService, [dto])!;
            await task;

            return Ok(dto);
        }

        [HttpPut("{entidad}")]
        public async Task<ActionResult> Actualizar(string entidad, [FromBody] System.Text.Json.JsonElement payload)
        {
            var type = ObtenerTipoEntidad(entidad);
            if (type == null) return NotFound(new { Message = $"Catálogo '{entidad}' no encontrado." });

            object? dto = System.Text.Json.JsonSerializer.Deserialize(payload.GetRawText(), type);
            if (dto == null) return BadRequest(new { Message = "Cuerpo de solicitud inválido." });

            var metodo = typeof(ICatalogoService).GetMethod("ActualizarAsync")!.MakeGenericMethod(type);
            Task task = (Task)metodo.Invoke(catalogoService, [dto])!;
            await task;

            return Ok(dto);
        }

        [HttpDelete("{entidad}/{id}")]
        public async Task<ActionResult> Eliminar(string entidad, string id)
        {
            var type = ObtenerTipoEntidad(entidad);
            if (type == null) return NotFound(new { Message = $"Catálogo '{entidad}' no encontrado." });

            object idParsed = int.TryParse(id, out int idInt) ? idInt : id;

            var metodo = typeof(ICatalogoService).GetMethod("EliminarAsync")!.MakeGenericMethod(type);
            Task task = (Task)metodo.Invoke(catalogoService, [idParsed])!;
            await task;

            return NoContent();
        }

        private static Type? ObtenerTipoEntidad(string entidad)
        {
            var assembly = typeof(HSis.Data.Models.Ticket).Assembly;
            return assembly.GetTypes().FirstOrDefault(t => t.Name.Equals(entidad, StringComparison.OrdinalIgnoreCase));
        }
    }
}
