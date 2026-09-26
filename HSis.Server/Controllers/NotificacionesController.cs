using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HSis.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class NotificacionesController(
    INotificacionService notificacionService,
    ICurrentUserService currentUserService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<NotificacionesResumenDto>> ObtenerResumen(
        CancellationToken cancellationToken)
    {
        var usuarioId = ObtenerUsuarioAutenticado();
        if (usuarioId is null)
        {
            return ProblemaAutenticacion();
        }

        return Ok(await notificacionService.ObtenerResumenAsync(
            usuarioId.Value,
            cancellationToken));
    }

    [HttpGet("nuevas")]
    [ProducesResponseType(typeof(IReadOnlyList<NotificacionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<NotificacionDto>>> ObtenerNuevas(
        [FromQuery] int desdeId = 0,
        CancellationToken cancellationToken = default)
    {
        if (desdeId < 0)
        {
            return BadRequest("desdeId no puede ser negativo.");
        }

        var usuarioId = ObtenerUsuarioAutenticado();
        if (usuarioId is null)
        {
            return ProblemaAutenticacion();
        }

        return Ok(await notificacionService.ObtenerNuevasAsync(
            usuarioId.Value,
            desdeId,
            cancellationToken));
    }

    [HttpPut("{idNotificacion:int}/leida")]
    public async Task<IActionResult> MarcarComoLeida(
        int idNotificacion,
        CancellationToken cancellationToken)
    {
        var usuarioId = ObtenerUsuarioAutenticado();
        if (usuarioId is null)
        {
            return ProblemaAutenticacion();
        }

        var actualizada = await notificacionService.MarcarComoLeidaAsync(
            usuarioId.Value,
            idNotificacion,
            cancellationToken);

        if (!actualizada)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Notificación no encontrada",
                detail: "La notificación no existe o no pertenece al usuario autenticado.");
        }

        return NoContent();
    }

    [HttpPut("leidas")]
    public async Task<ActionResult<int>> MarcarTodasComoLeidas(
        CancellationToken cancellationToken)
    {
        var usuarioId = ObtenerUsuarioAutenticado();
        if (usuarioId is null)
        {
            return ProblemaAutenticacion();
        }

        var actualizadas = await notificacionService.MarcarTodasComoLeidasAsync(
            usuarioId.Value,
            cancellationToken);

        return Ok(actualizadas);
    }

    [HttpDelete]
    public async Task<IActionResult> LimpiarTodas(CancellationToken cancellationToken)
    {
        var usuarioId = ObtenerUsuarioAutenticado();
        if (usuarioId is null)
        {
            return ProblemaAutenticacion();
        }

        await notificacionService.LimpiarTodasAsync(
            usuarioId.Value,
            cancellationToken);

        return NoContent();
    }

    private int? ObtenerUsuarioAutenticado()
    {
        var usuarioId = currentUserService.GetCurrentUserId();
        return usuarioId > 0 ? usuarioId : null;
    }

    private ObjectResult ProblemaAutenticacion()
        => Problem(
            statusCode: StatusCodes.Status401Unauthorized,
            title: "Autenticación requerida",
            detail: "No se pudo identificar al usuario autenticado.");
}
