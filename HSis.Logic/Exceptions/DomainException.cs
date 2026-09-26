namespace HSis.Logic.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(
        string code,
        string userMessage,
        string? technicalDetails = null,
        Exception? innerException = null)
        : base(technicalDetails ?? userMessage, innerException)
    {
        Code = code;
        UserMessage = userMessage;
        TechnicalDetails = technicalDetails;
    }

    public string Code { get; }

    public string UserMessage { get; }

    public string? TechnicalDetails { get; }
}
