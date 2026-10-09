using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.Contracts.Constants;
using HSis.Server.Configurations;
using HSis.Server.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HSis.Server.Controllers
{
    [Authorize(Policy = PoliticasAutorizacion.PersonalAtencion)]
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController(IUsuarioService usuarioService) : ControllerBase
    {

        [HttpGet("rol/{idRol}")]
        public async Task<ActionResult<List<UsuarioDto>>> ObtenerUsuariosPorRol(int idRol)
        {
            var usuarios = await usuarioService.ObtenerUsuariosPorRolAsync(idRol);

            // El tecnico solo necesita identificar personas para asignar y registrar tickets; el detalle es del administrador.
            if (!User.EsAdministrador())
            {
                usuarios = [.. usuarios.Select(u => new UsuarioDto
                {
                    IdUsuario = u.IdUsuario,
                    Nombre = u.Nombre,
                    IdRol = u.IdRol
                })];
            }

            return Ok(usuarios);
        }

    }
}

