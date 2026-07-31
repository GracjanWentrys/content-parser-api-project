using Api.Enums;
using Api.Models.Results;

namespace Api.Services.Abstractions;

public interface IContentParser
{
    ContentType SupportedType { get; }
    IParseResult Parse(string rawContent);
}