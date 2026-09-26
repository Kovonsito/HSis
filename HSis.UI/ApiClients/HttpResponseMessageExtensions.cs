using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Net;
using HSis.Contracts.Errors;

namespace HSis.UI.ApiClients
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<T?> GetFromJsonWithDetailsAsync<T>(
            this HttpClient httpClient,
            string requestUri,
            CancellationToken cancellationToken = default)
        {
            using var response = await httpClient.GetAsync(requestUri, cancellationToken);
            return await response.ReadFromJsonWithDetailsAsync<T>(cancellationToken);
        }

        public static async Task<T?> ReadFromJsonWithDetailsAsync<T>(
            this HttpResponseMessage response,
            CancellationToken cancellationToken = default)
        {
            await response.EnsureSuccessStatusCodeWithDetailsAsync();

            if (response.StatusCode == HttpStatusCode.NoContent
                || response.Content.Headers.ContentLength == 0)
            {
                return default;
            }

            return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
        }

        public static async Task<T> ReadRequiredJsonWithDetailsAsync<T>(
            this HttpResponseMessage response,
            string resourceName,
            CancellationToken cancellationToken = default)
        {
            var result = await response.ReadFromJsonWithDetailsAsync<T>(cancellationToken);
            return result ?? throw new HttpRequestException($"La API no devolvió el {resourceName} esperado.");
        }

        public static async Task EnsureSuccessStatusCodeWithDetailsAsync(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode) return;

            string rawContent = await response.Content.ReadAsStringAsync();
            var problem = LeerProblemDetails(rawContent);
            string userMessage = ObtenerMensajeUsuario(response.StatusCode, problem);
            var exception = new ApiException(
                response.StatusCode,
                userMessage,
                problem.Code,
                problem.TraceId,
                problem.Errors,
                rawContent,
                problem.Type,
                problem.Title);

            throw exception;
        }

        private static ParsedProblem LeerProblemDetails(string rawContent)
        {
            if (string.IsNullOrWhiteSpace(rawContent)) return new ParsedProblem();

            try
            {
                using var document = JsonDocument.Parse(rawContent);
                var root = document.RootElement;
                if (root.ValueKind != JsonValueKind.Object) return new ParsedProblem();

                var errors = LeerErrores(root);
                return new ParsedProblem(
                    LeerString(root, "code"),
                    LeerString(root, "traceId"),
                    LeerString(root, "detail") ?? LeerString(root, "Detail") ?? LeerString(root, "Error") ?? LeerString(root, "Message") ?? LeerString(root, "message"),
                    LeerString(root, "type"),
                    LeerString(root, "title"),
                    errors);
            }
            catch (JsonException)
            {
                return new ParsedProblem();
            }
        }

        private static IReadOnlyDictionary<string, string[]>? LeerErrores(JsonElement root)
        {
            if (!root.TryGetProperty("errors", out var errorsElement) || errorsElement.ValueKind != JsonValueKind.Object)
            {
                if (!root.TryGetProperty("Detalles", out var detallesElement) || detallesElement.ValueKind != JsonValueKind.Array)
                {
                    return null;
                }

                var detalles = detallesElement.EnumerateArray()
                    .Where(item => item.ValueKind == JsonValueKind.String)
                    .Select(item => item.GetString())
                    .Where(item => !string.IsNullOrWhiteSpace(item))
                    .Cast<string>()
                    .ToArray();
                return detalles.Length == 0 ? null : new Dictionary<string, string[]> { ["general"] = detalles };
            }

            return errorsElement.EnumerateObject()
                .ToDictionary(
                    property => property.Name,
                    property => property.Value.ValueKind == JsonValueKind.Array
                        ? property.Value.EnumerateArray().Where(item => item.ValueKind == JsonValueKind.String).Select(item => item.GetString()!).ToArray()
                        : [property.Value.ToString()]);
        }

        private static string? LeerString(JsonElement root, string propertyName)
            => root.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String
                ? property.GetString()
                : null;

        private static string ObtenerMensajeUsuario(HttpStatusCode statusCode, ParsedProblem problem)
        {
            if (problem.Errors is { Count: > 0 })
            {
                var detalles = problem.Errors.SelectMany(pair => pair.Value).Where(error => !string.IsNullOrWhiteSpace(error)).ToArray();
                if (detalles.Length > 0)
                {
                    return string.IsNullOrWhiteSpace(problem.Message)
                        ? $"Revise los datos indicados:\n\n{string.Join("\n", detalles.Select(error => $"• {error}"))}"
                        : $"{problem.Message}\n\n{string.Join("\n", detalles.Select(error => $"• {error}"))}";
                }
            }

            if (!string.IsNullOrWhiteSpace(problem.Message)) return problem.Message;

            return statusCode switch
            {
                HttpStatusCode.Unauthorized => "Su sesión no es válida o ha caducado. Inicie sesión nuevamente.",
                HttpStatusCode.Forbidden => "No tiene permisos suficientes para realizar esta operación.",
                HttpStatusCode.NotFound => "No se encontró la información solicitada.",
                HttpStatusCode.Conflict => "La operación no puede realizarse porque la información cambió o no está en un estado válido.",
                >= HttpStatusCode.BadRequest and < HttpStatusCode.InternalServerError => "La información enviada no es válida. Revise los datos e inténtelo de nuevo.",
                _ => "Ocurrió un problema inesperado. Inténtelo de nuevo y contacte al soporte si continúa."
            };
        }

        private sealed record ParsedProblem(
            string? Code = null,
            string? TraceId = null,
            string? Message = null,
            string? Type = null,
            string? Title = null,
            IReadOnlyDictionary<string, string[]>? Errors = null);
    }
}

