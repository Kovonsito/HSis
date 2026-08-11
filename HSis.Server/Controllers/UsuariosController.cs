using HSis.Logic.DTOs;
using HSis.Logic.Services;
using Microsoft.AspNetCore.Mvc;

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
