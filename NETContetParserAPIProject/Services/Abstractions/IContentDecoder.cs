namespace Api.Services.Abstractions;

public interface IContentDecoder
{
    string DecodeBase64(string encodedContent);
}