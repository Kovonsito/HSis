using HSis.Contracts.DTOs;
using HSis.Contracts.Errors;
using HSis.Contracts.Services;
using HSis.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace HSis.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(
        IUsuarioService usuarioService,
        IJwtTokenService jwtTokenService,
        ILogger<AuthController> logger) : ControllerBase
    {

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> IniciarSesion([FromBody] LoginRequestDto request)
        {
            var usuario = await usuarioService.AutenticarAsync(request.Username, request.Password);
            if (usuario == null)
            {
                logger.LogWarning(
                    "Intento de inicio de sesión rechazado para el usuario {Username}. TraceId: {TraceId}",
                    request.Username,
                    HttpContext.TraceIdentifier);

                return Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "No se pudo iniciar sesión",
                    detail: "El usuario o la contraseña no son válidos, o la cuenta está inactiva.",
                    extensions: new Dictionary<string, object?>
                    {
                        ["code"] = ApiErrorCodes.Unauthorized,
                        ["traceId"] = HttpContext.TraceIdentifier
                    });
            }

            var token = jwtTokenService.GenerarToken(usuario);

            return Ok(new LoginResponseDto
            {
                Usuario = usuario,
                Token = token
            });
        }
    }

}

