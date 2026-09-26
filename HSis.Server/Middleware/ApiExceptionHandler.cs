using System.Net;
using FluentValidation;
using HSis.Contracts.Errors;
using HSis.Logic.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HSis.Server.Middleware;

public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = httpContext.TraceIdentifier;
        var error = MapException(exception);

        using var scope = logger.BeginScope(new Dictionary<string, object?>
        {
            ["TraceId"] = traceId,
            ["ErrorCode"] = error.Code,
            ["RequestMethod"] = httpContext.Request.Method,
            ["RequestPath"] = httpContext.Request.Path.Value
        });

        if (error.LogLevel == LogLevel.Warning)
        {
            logger.LogWarning(exception, "Solicitud rechazada. Código {ErrorCode}. Mensaje para usuario: {UserMessage}", error.Code, error.UserMessage);
        }
        else
        {
            logger.LogError(exception, "Error no controlado. Código {ErrorCode}. Mensaje para usuario: {UserMessage}", error.Code, error.UserMessage);
        }

        if (httpContext.Response.HasStarted)
        {
            logger.LogWarning(
                "No se pudo devolver ProblemDetails porque la respuesta ya había comenzado. Código {ErrorCode}",
                error.Code);
            return false;
        }

        httpContext.Response.StatusCode = error.StatusCode;
        httpContext.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Status = error.StatusCode,
            Title = error.Title,
            Detail = error.UserMessage,
            Instance = httpContext.Request.Path
        };
        problemDetails.Extensions["code"] = error.Code;
        problemDetails.Extensions["traceId"] = traceId;

        if (error.ValidationErrors is not null)
        {
            problemDetails.Extensions["errors"] = error.ValidationErrors;
        }

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }

    internal static string ObtenerCodigoError(int statusCode)
        => statusCode switch
        {
            StatusCodes.Status400BadRequest => ApiErrorCodes.BadRequest,
            StatusCodes.Status401Unauthorized => ApiErrorCodes.Unauthorized,
            StatusCodes.Status403Forbidden => ApiErrorCodes.Forbidden,
            StatusCodes.Status404NotFound => ApiErrorCodes.ResourceNotFound,
            StatusCodes.Status409Conflict => ApiErrorCodes.Conflict,
            >= StatusCodes.Status500InternalServerError => ApiErrorCodes.InternalServerError,
            _ => ApiErrorCodes.BadRequest
        };

    internal static string ObtenerTituloError(int statusCode)
        => statusCode switch
        {
            StatusCodes.Status401Unauthorized => "Autenticación requerida",
            StatusCodes.Status403Forbidden => "Acceso no permitido",
            StatusCodes.Status404NotFound => "Recurso no encontrado",
            StatusCodes.Status409Conflict => "Operación no permitida",
            >= StatusCodes.Status500InternalServerError => "No se pudo completar la operación",
            _ => "Solicitud no válida"
        };

    internal static string ObtenerMensajeError(int statusCode)
        => statusCode switch
        {
            StatusCodes.Status401Unauthorized => "La sesión no es válida o ha caducado. Inicie sesión nuevamente.",
            StatusCodes.Status403Forbidden => "No tiene permisos suficientes para realizar esta operación.",
            StatusCodes.Status404NotFound => "No se encontró la información solicitada.",
            StatusCodes.Status409Conflict => "La operación no puede realizarse porque la información está en un estado no válido.",
            >= StatusCodes.Status500InternalServerError => "Ocurrió un problema inesperado. Inténtelo de nuevo y proporcione el código de seguimiento si el problema continúa.",
            _ => "La información enviada no es válida. Revise los datos e inténtelo de nuevo."
        };

    private static MappedError MapException(Exception exception)
        => exception switch
        {
            ValidationException validationException => new(
                StatusCodes.Status400BadRequest,
                ApiErrorCodes.Validation,
                "Los datos enviados no son válidos.",
                "Revise los campos indicados y vuelva a intentarlo.",
                LogLevel.Warning,
                validationException.Errors
                    .GroupBy(error => string.IsNullOrWhiteSpace(error.PropertyName) ? "general" : error.PropertyName)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(error => error.ErrorMessage).Distinct().ToArray())),
            DomainException domainException => new(
                GetDomainStatusCode(domainException),
                domainException.Code,
                GetDomainTitle(GetDomainStatusCode(domainException)),
                domainException.UserMessage,
                LogLevel.Warning),
            KeyNotFoundException => new(
                StatusCodes.Status404NotFound,
                ApiErrorCodes.ResourceNotFound,
                "Recurso no encontrado",
                "No se encontró la información solicitada. Compruebe los datos e inténtelo de nuevo.",
                LogLevel.Warning),
            ArgumentException => new(
                StatusCodes.Status400BadRequest,
                ApiErrorCodes.BadRequest,
                "Solicitud no válida",
                "La información enviada no tiene un formato válido. Revise los datos e inténtelo de nuevo.",
                LogLevel.Warning),
            InvalidOperationException => new(
                StatusCodes.Status409Conflict,
                ApiErrorCodes.Conflict,
                "Operación no permitida",
                "La operación no puede realizarse en el estado actual de la información.",
                LogLevel.Warning),
            _ => new(
                StatusCodes.Status500InternalServerError,
                ApiErrorCodes.InternalServerError,
                "No se pudo completar la operación",
                "Ocurrió un problema inesperado. Inténtelo de nuevo y proporcione el código de seguimiento al equipo de soporte si el problema continúa.",
                LogLevel.Error)
        };

    private static int GetDomainStatusCode(DomainException exception)
        => exception.Code switch
        {
            ApiErrorCodes.TicketNotFound => StatusCodes.Status404NotFound,
            ApiErrorCodes.TicketRatingInvalid => StatusCodes.Status400BadRequest,
            ApiErrorCodes.TicketNotClosed => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest
        };

    private static string GetDomainTitle(int statusCode)
        => ObtenerTituloError(statusCode);

    private sealed record MappedError(
        int StatusCode,
        string Code,
        string Title,
        string UserMessage,
        LogLevel LogLevel,
        IReadOnlyDictionary<string, string[]>? ValidationErrors = null);
}
