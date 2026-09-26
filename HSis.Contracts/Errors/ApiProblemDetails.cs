using System.Text.Json.Serialization;

namespace HSis.Contracts.Errors;

/// <summary>
/// Representa el formato de error que devuelve la API. El detalle técnico no se
/// expone en esta respuesta; se registra en el servidor junto con <see cref="TraceId"/>.
/// </summary>
public sealed record ApiProblemDetails
{
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("status")]
    public int? Status { get; init; }

    [JsonPropertyName("detail")]
    public string? Detail { get; init; }

    [JsonPropertyName("instance")]
    public string? Instance { get; init; }

    [JsonPropertyName("code")]
    public string? Code { get; init; }

    [JsonPropertyName("traceId")]
    public string? TraceId { get; init; }

    [JsonPropertyName("errors")]
    public IReadOnlyDictionary<string, string[]>? Errors { get; init; }
}
