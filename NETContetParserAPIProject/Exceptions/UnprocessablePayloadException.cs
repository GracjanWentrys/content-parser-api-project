namespace Api.Exceptions;

public class UnprocessablePayloadException : DomainException
{
    public UnprocessablePayloadException(string message) : base(message) { }
    public UnprocessablePayloadException(string message, Exception innerException) : base(message, innerException) { }
}