#nullable enable
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.Versioning;
using System.Windows.Forms;
using FluentValidation;
using HSis.UI.ApiClients;
using Serilog;

namespace HSis.UI.Helpers;

/// <summary>
/// Punto único para clasificar, registrar y presentar errores en la aplicación de escritorio.
/// </summary>
[SupportedOSPlatform("windows")]
public static class ManejadorErroresUI
{
    public static void Manejar(
        Exception exception,
        string? contexto = null,
        IWin32Window? propietario = null,
        bool mostrarDialogo = true,
        bool esFatal = false)
    {
        var error = ObtenerExcepcionPrincipal(exception);
        var contextoNormalizado = string.IsNullOrWhiteSpace(contexto)
            ? "la operación"
            : contexto;

        switch (error)
        {
            case ValidationException validationException:
                var validaciones = validationException.Errors
                    .GroupBy(error => string.IsNullOrWhiteSpace(error.PropertyName) ? "general" : error.PropertyName)
                    .ToDictionary(
                        grupo => grupo.Key,
                        grupo => grupo.Select(error => error.ErrorMessage).Distinct().ToArray());
                var textoValidaciones = string.Join(
                    "\n",
                    validaciones.SelectMany(grupo => grupo.Value.Select(mensaje => $"• {mensaje}")));

                Log.Warning(
                    validationException,
                    "Validación fallida durante {Contexto}. Errores: {ValidationErrors}",
                    contextoNormalizado,
                    textoValidaciones);

                if (mostrarDialogo)
                {
                    DialogoUIHelper.MostrarAdvertencia(
                        $"Por favor corrija los siguientes datos:\n\n{textoValidaciones}",
                        "Validación de datos",
                        propietario);
                }

                return;

            case ApiException apiException:
                var mensajeApi = apiException.UserMessage;
                if (!string.IsNullOrWhiteSpace(apiException.TechnicalReference))
                {
                    mensajeApi += $"\n\nCódigo de seguimiento: {apiException.TechnicalReference}";
                }

                var erroresApi = apiException.ValidationErrors is null
                    ? null
                    : string.Join(
                        "; ",
                        apiException.ValidationErrors.SelectMany(
                            grupo => grupo.Value.Select(mensaje => $"{grupo.Key}: {mensaje}")));

                Log.Warning(
                    apiException,
                    "La API rechazó {Contexto}. Estado: {StatusCode}. Código: {ErrorCode}. TraceId: {TraceId}. Validaciones: {ValidationErrors}",
                    contextoNormalizado,
                    apiException.StatusCode,
                    apiException.Code,
                    apiException.TraceId,
                    erroresApi);

                if (mostrarDialogo)
                {
                    DialogoUIHelper.MostrarAdvertencia(
                        mensajeApi,
                        apiException.Title ?? "No se pudo completar la operación",
                        propietario);
                }

                return;

            case TaskCanceledException timeoutException:
                Log.Warning(
                    timeoutException,
                    "Tiempo de espera agotado durante {Contexto}",
                    contextoNormalizado);

                if (mostrarDialogo)
                {
                    DialogoUIHelper.MostrarAdvertencia(
                        "El servidor tardó demasiado en responder. Compruebe la conexión e inténtelo de nuevo.",
                        "Tiempo de espera agotado",
                        propietario);
                }

                return;

            case OperationCanceledException:
                Log.Information("Operación cancelada durante {Contexto}", contextoNormalizado);
                return;

            case HttpRequestException httpException:
                Log.Warning(
                    httpException,
                    "Fallo de comunicación HTTP durante {Contexto}",
                    contextoNormalizado);

                if (mostrarDialogo)
                {
                    DialogoUIHelper.MostrarAdvertencia(
                        "No se pudo comunicar con el servidor para completar la operación. Verifique su conexión de red y vuelva a intentarlo.",
                        "Problema de comunicación",
                        propietario);
                }

                return;

            case SocketException socketException:
                Log.Warning(
                    socketException,
                    "Fallo de red durante {Contexto}",
                    contextoNormalizado);

                if (mostrarDialogo)
                {
                    DialogoUIHelper.MostrarAdvertencia(
                        "No se pudo establecer comunicación con el servidor. Verifique su conexión de red y vuelva a intentarlo.",
                        "Problema de comunicación",
                        propietario);
                }

                return;

            default:
                var correlationId = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
                if (esFatal)
                {
                    Log.Fatal(
                        error,
                        "[{CorrelationId}] Excepción no controlada durante {Contexto}",
                        correlationId,
                        contextoNormalizado);
                }
                else
                {
                    Log.Error(
                        error,
                        "[{CorrelationId}] Error inesperado durante {Contexto}",
                        correlationId,
                        contextoNormalizado);
                }

                if (mostrarDialogo)
                {
                    DialogoUIHelper.MostrarError(
                        $"{contextoNormalizado}. No fue posible completar la operación.\n\n" +
                        $"Código de seguimiento: #{correlationId}\n" +
                        "Proporcione este código al equipo de soporte técnico si el problema continúa.",
                        "Error del sistema",
                        propietario);
                }

                return;
        }
    }

    private static Exception ObtenerExcepcionPrincipal(Exception exception)
    {
        while (exception is AggregateException aggregateException && aggregateException.InnerExceptions.Count == 1)
        {
            exception = aggregateException.InnerExceptions[0];
        }

        return exception;
    }
}
