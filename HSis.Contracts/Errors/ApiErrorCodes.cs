namespace HSis.Contracts.Errors;

public static class ApiErrorCodes
{
    public const string Validation = "HSIS-VALIDATION";
    public const string BadRequest = "HSIS-BAD-REQUEST";
    public const string Unauthorized = "HSIS-UNAUTHORIZED";
    public const string Forbidden = "HSIS-FORBIDDEN";
    public const string ResourceNotFound = "HSIS-RESOURCE-NOT-FOUND";
    public const string Conflict = "HSIS-CONFLICT";
    public const string InternalServerError = "HSIS-INTERNAL-ERROR";
    public const string ServiceUnavailable = "HSIS-SERVICE-UNAVAILABLE";

    public const string TicketNotFound = "HSIS-TICKET-NOT-FOUND";
    public const string TicketRatingInvalid = "HSIS-TICKET-RATING-INVALID";
    public const string TicketNotClosed = "HSIS-TICKET-NOT-CLOSED";
}
