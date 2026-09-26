using System.Net.Http;
using System.Net;

namespace HSis.UI.ApiClients;

public sealed class ApiException : HttpRequestException
{
    public ApiException(
        HttpStatusCode statusCode,
        string userMessage,
        string? code,
        string? traceId,
        IReadOnlyDictionary<string, string[]>? validationErrors,
        string rawResponse,
        string? type,
        string? title)
        : base(userMessage, null, statusCode)
    {
        UserMessage = userMessage;
        Code = code;
        TraceId = traceId;
        ValidationErrors = validationErrors;
        RawResponse = rawResponse;
        Type = type;
        Title = title;
    }

    public string UserMessage { get; }

    public string? Code { get; }

    public string? TraceId { get; }

    public IReadOnlyDictionary<string, string[]>? ValidationErrors { get; }

    public string RawResponse { get; }

    public string? Type { get; }

    public string? Title { get; }

    public string? TechnicalReference
        => string.IsNullOrWhiteSpace(TraceId)
            ? Code
            : string.IsNullOrWhiteSpace(Code)
                ? $"Seguimiento: {TraceId}"
                : $"{Code} / Seguimiento: {TraceId}";
}
