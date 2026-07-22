using Microsoft.AspNetCore.Mvc;
using HSis.Logic.Services;
using HSis.Logic.DTOs;

namespace HSis.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IUsuarioService usuarioService) : ControllerBase
    {

        [HttpPost("login")]
        public async Task<ActionResult<UsuarioDto>> IniciarSesion([FromBody] LoginRequest request)
        {
            var usuario = await usuarioService.AutenticarAsync(request.Username, request.Password);
            if (usuario == null) return Unauthorized(new { Message = "Credenciales incorrectas o usuario inactivo." });
            return Ok(usuario);
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
