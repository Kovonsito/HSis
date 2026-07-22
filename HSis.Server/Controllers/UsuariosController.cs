using Microsoft.AspNetCore.Mvc;
using HSis.Logic.Services;
using HSis.Logic.DTOs;

namespace HSis.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController(IUsuarioService usuarioService) : ControllerBase
    {

        [HttpGet("rol/{idRol}")]
        public async Task<ActionResult<List<UsuarioDto>>> ObtenerUsuariosPorRol(int idRol)
        {
            return Ok(await usuarioService.ObtenerUsuariosPorRolAsync(idRol));
        }
    }
}
