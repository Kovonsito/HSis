using System.Net;
using System.Text.Json;
using FluentValidation;

namespace HSis.Server.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción no controlada procesando la solicitud {Path}: {Message}", context.Request.Path, ex.Message);
                await ManejarExcepcionAsync(context, ex);
            }
        }

        private static Task ManejarExcepcionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = HttpStatusCode.InternalServerError;
            object responseBody;

            switch (exception)
            {
                case ValidationException valEx:
                    statusCode = HttpStatusCode.BadRequest;
                    responseBody = new
                    {
                        Error = "Error de validación",
                        Detalles = valEx.Errors.Select(e => e.ErrorMessage)
                    };
                    break;

                case KeyNotFoundException keyEx:
                    statusCode = HttpStatusCode.NotFound;
                    responseBody = new { Error = keyEx.Message };
                    break;

                case ArgumentException argEx:
                    statusCode = HttpStatusCode.BadRequest;
                    responseBody = new { Error = argEx.Message };
                    break;

                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    responseBody = new { Error = "Ocurrió un error interno en el servidor.", Detalle = "Consulte los registros del servidor para más información." };
                    break;
            }

            context.Response.StatusCode = (int)statusCode;
            return context.Response.WriteAsync(JsonSerializer.Serialize(responseBody));
        }
    }
}

