using System.Text;
using Api.Exceptions;
using Api.Services.Abstractions;

namespace Api.Services.Implementations;

public class Base64ContentDecoder : IContentDecoder
{
    public string DecodeBase64(string encodedContent)
    {
        if (string.IsNullOrWhiteSpace(encodedContent))
        {
            throw new UnprocessablePayloadException("Payload content cannot be empty.");
        }

        try
        {
            var bytes = Convert.FromBase64String(encodedContent);
            return Encoding.UTF8.GetString(bytes);
        }
        catch (FormatException ex)
        {
            throw new UnprocessablePayloadException("Invalid Base64 string in content field.", ex);
        }
    }
}