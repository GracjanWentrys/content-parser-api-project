using System.Text;
using System.Text.Json;
using Api.Models;
using Microsoft.VisualBasic.FileIO;

namespace Api.Services;

public interface IContentDecoder
{
    string DecodeBase64(string encodedContent);
}

public class Base64ContentDecoder : IContentDecoder
{
    public string DecodeBase64(string encodedContent)
    {
        if (string.IsNullOrWhiteSpace(encodedContent))
        {
            throw new ArgumentException("Payload content cannot be empty.");
        }

        var bytes = Convert.FromBase64String(encodedContent);
        return Encoding.UTF8.GetString(bytes);
    }
}

public interface IContentParser
{
    ContentType SupportedType { get; }
    IParseResult Parse(string rawContent);
}

public class CsvContentParser : IContentParser
{
    public ContentType SupportedType => ContentType.CSV;

    public IParseResult Parse(string rawContent)
    {
        if (string.IsNullOrWhiteSpace(rawContent))
        {
            return ParseResult<List<CsvRow>>.Success(new List<CsvRow>(), 0);
        }

        using var reader = new StringReader(rawContent);
        using var csv = new TextFieldParser(reader);

        csv.SetDelimiters(",");
        csv.HasFieldsEnclosedInQuotes = true;

        var headers = csv.ReadFields();
        if (headers == null)
        {
            return ParseResult<List<CsvRow>>.Success(new List<CsvRow>(), 0);
        }

        var rows = new List<CsvRow>();

        while (!csv.EndOfData)
        {
            var fields = csv.ReadFields();
            if (fields == null)
                continue;

            var columns = new Dictionary<string, string>();

            for (int i = 0; i < headers.Length; i++)
            {
                columns[headers[i]] = i < fields.Length ? fields[i] : string.Empty;
            }

            rows.Add(new CsvRow(columns));
        }

        return ParseResult<List<CsvRow>>.Success(rows, rows.Count);
    }
}

public class InternalJsonContentParser : IContentParser
{
    public ContentType SupportedType => ContentType.INTERNAL_JSON;

    public IParseResult Parse(string rawContent)
    {
        using var document = JsonDocument.Parse(rawContent);
        var root = document.RootElement.Clone();

        if (root.ValueKind == JsonValueKind.Array)
        {
            return ParseResult<JsonElement>.Success(root, root.GetArrayLength());
        }

        if (root.ValueKind == JsonValueKind.Object)
        {
            return ParseResult<JsonElement>.Success(root, 1);
        }

        return ParseResult<JsonElement>.Failure("Invalid JSON structure. Expected object or array.");
    }
}

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

        throw new NotSupportedException($"Content type '{type}' is not supported.");
    }
}