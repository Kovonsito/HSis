namespace HSis.Logic.Exceptions;

public abstract class DomainException(
    string code,
    string userMessage,
    string? technicalDetails = null,
    Exception? innerException = null) : Exception(technicalDetails ?? userMessage, innerException)
{
    public string Code { get; } = code;

    public string UserMessage { get; } = userMessage;

    public string? TechnicalDetails { get; } = technicalDetails;
}
