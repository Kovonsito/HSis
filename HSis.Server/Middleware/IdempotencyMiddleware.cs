#nullable enable
using System.IO;
using System.Security.Claims;
using System.Text;
using HSis.Contracts.Constants;
using HSis.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HSis.Server.Middleware;

/// <summary>
/// Middleware que intercepta peticiones mutativas con el encabezado X-Idempotency-Key.
/// Si la clave ya fue procesada, retransmite la respuesta exacta previa sin reejecutar la lógica de negocio.
/// Si es nueva y exitosa (2xx), almacena el resultado para responder de forma idempotente a futuros reintentos.
/// </summary>
public class IdempotencyMiddleware(RequestDelegate next, ILogger<IdempotencyMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, IDbContextFactory<HSisDbContext> dbContextFactory)
    {
        // 1. Verificar si viene la cabecera X-Idempotency-Key
        if (!context.Request.Headers.TryGetValue(ConstantesSincronizacion.HeaderIdempotencia, out var keyVal) ||
            string.IsNullOrWhiteSpace(keyVal))
        {
            await next(context);
            return;
        }

        if (!Guid.TryParse(keyVal.ToString(), out var idOperacion))
        {
            await next(context);
            return;
        }

        // Solo se aplica a métodos mutativos
        var metodo = context.Request.Method.ToUpperInvariant();
        if (metodo is not ("POST" or "PUT" or "PATCH" or "DELETE"))
        {
            await next(context);
            return;
        }

        // 2. Verificar si la operación ya fue procesada anteriormente
        await using var db = await dbContextFactory.CreateDbContextAsync(context.RequestAborted);
        var operacionExistente = await db.OperacionesIdempotentes
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.IdOperacion == idOperacion, context.RequestAborted);

        if (operacionExistente != null)
        {
            logger.LogInformation(
                "Idempotencia detectada: Repitiendo respuesta para IdOperacion {IdOperacion} [{Metodo} {Ruta}]",
                idOperacion,
                operacionExistente.MetodoHttp,
                operacionExistente.RutaEndpoint);

            context.Response.StatusCode = operacionExistente.CodigoEstadoHttp;
            context.Response.Headers[ConstantesSincronizacion.HeaderIdempotenciaReplay] = "true";

            if (!string.IsNullOrEmpty(operacionExistente.RespuestaJson))
            {
                context.Response.ContentType = "application/json; charset=utf-8";
                await context.Response.WriteAsync(operacionExistente.RespuestaJson, Encoding.UTF8);
            }

            return;
        }

        // 3. Capturar la respuesta generada por el endpoint
        var originalBodyStream = context.Response.Body;
        await using var responseBodyMemoryStream = new MemoryStream();
        context.Response.Body = responseBodyMemoryStream;

        try
        {
            await next(context);

            responseBodyMemoryStream.Seek(0, SeekOrigin.Begin);
            var responseBodyText = await new StreamReader(responseBodyMemoryStream, Encoding.UTF8).ReadToEndAsync();
            responseBodyMemoryStream.Seek(0, SeekOrigin.Begin);

            // Solo registrar idempotencia si la operación fue exitosa (200..299)
            if (context.Response.StatusCode is >= 200 and < 300)
            {
                var idUsuarioStr = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _ = int.TryParse(idUsuarioStr, out var idUsuario);

                var nuevaOperacion = new OperacionIdempotente
                {
                    IdOperacion = idOperacion,
                    IdUsuario = idUsuario,
                    RutaEndpoint = context.Request.Path.Value ?? string.Empty,
                    MetodoHttp = metodo,
                    CodigoEstadoHttp = context.Response.StatusCode,
                    RespuestaJson = string.IsNullOrWhiteSpace(responseBodyText) ? null : responseBodyText,
                    FechaCreacion = DateTime.UtcNow
                };

                try
                {
                    await using var saveDb = await dbContextFactory.CreateDbContextAsync(context.RequestAborted);
                    saveDb.OperacionesIdempotentes.Add(nuevaOperacion);
                    await saveDb.SaveChangesAsync(context.RequestAborted);
                }
                catch (DbUpdateException ex)
                {
                    // Si otra petición concurrente idéntica ya insertó el registro, no fallar
                    logger.LogWarning(ex, "Colisión concurrente de clave de idempotencia {IdOperacion}", idOperacion);
                }
            }

            await responseBodyMemoryStream.CopyToAsync(originalBodyStream);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }
}
