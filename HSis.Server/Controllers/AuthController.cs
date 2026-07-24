using Microsoft.AspNetCore.Mvc;
using HSis.Logic.Services;
using HSis.Logic.DTOs;
using HSis.Server.Services;

namespace HSis.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IUsuarioService usuarioService, IJwtTokenService jwtTokenService) : ControllerBase
    {

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> IniciarSesion([FromBody] LoginRequest request)
        {
            var usuario = await usuarioService.AutenticarAsync(request.Username, request.Password);
            if (usuario == null) return Unauthorized(new { Message = "Credenciales incorrectas o usuario inactivo." });

            var token = jwtTokenService.GenerarToken(usuario);

            return Ok(new LoginResponseDto
            {
                Usuario = usuario,
                Token = token
            });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
