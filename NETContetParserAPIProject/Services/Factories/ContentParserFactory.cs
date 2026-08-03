using Api.Enums;
using Api.Exceptions;
using Api.Services.Abstractions;

namespace Api.Services.Factories;

public class ContentParserFactory
{
    private readonly Dictionary<ContentType, IContentParser> _parsers;

    public ContentParserFactory(IEnumerable<IContentParser> parsers)
    {
        _parsers = parsers.ToDictionary(x => x.SupportedType);
    }

    public IContentParser GetParser(ContentType type)
    {
        if (_parsers.TryGetValue(type, out var parser))
        {
            return parser;
        }

        throw new UnprocessablePayloadException($"Content type '{type}' is not supported.");
    }
}