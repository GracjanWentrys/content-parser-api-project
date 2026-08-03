using System.Text.Json;
using Api.Enums;
using Api.Models.Results;
using Api.Services.Abstractions;

namespace Api.Services.Implementations;

public class InternalJsonContentParser : IContentParser
{
    public ContentType SupportedType => ContentType.INTERNAL_JSON;

    public IParseResult Parse(string rawContent)
    {
        try
        {
            using var document = JsonDocument.Parse(rawContent);
            var root = document.RootElement;

            // Zapewniamy spójną strukturę tablicy obiektów w odpowiedzi
            if (root.ValueKind == JsonValueKind.Array)
            {
                var list = JsonSerializer.Deserialize<List<Dictionary<string, object?>>>(root.GetRawText());
                return ParseResult<List<Dictionary<string, object?>>>.Success(list ?? new(), list?.Count ?? 0);
            }

            if (root.ValueKind == JsonValueKind.Object)
            {
                var singleObj = JsonSerializer.Deserialize<Dictionary<string, object?>>(root.GetRawText());
                var list = singleObj != null ? new List<Dictionary<string, object?>> { singleObj } : new List<Dictionary<string, object?>>();
                return ParseResult<List<Dictionary<string, object?>>>.Success(list, list.Count);
            }

            return ParseResult<object>.Failure("Invalid JSON structure. Expected object or array of objects.");
        }
        catch (JsonException ex)
        {
            return ParseResult<object>.Failure($"JSON parsing error: {ex.Message}");
        }
    }
}