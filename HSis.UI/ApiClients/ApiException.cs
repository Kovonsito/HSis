using System.Net.Http;
using System.Net;

namespace HSis.UI.ApiClients;

public sealed class ApiException(
    HttpStatusCode statusCode,
    string userMessage,
    string? code,
    string? traceId,
    IReadOnlyDictionary<string, string[]>? validationErrors,
    string rawResponse,
    string? type,
    string? title) : HttpRequestException(userMessage, null, statusCode)
{
    public string UserMessage { get; } = userMessage;

    public string? Code { get; } = code;

    public string? TraceId { get; } = traceId;

    public IReadOnlyDictionary<string, string[]>? ValidationErrors { get; } = validationErrors;

    public string RawResponse { get; } = rawResponse;

    public string? Type { get; } = type;

    public string? Title { get; } = title;

    public string? TechnicalReference
        => string.IsNullOrWhiteSpace(TraceId)
            ? Code
            : string.IsNullOrWhiteSpace(Code)
                ? $"Seguimiento: {TraceId}"
                : $"{Code} / Seguimiento: {TraceId}";
}
