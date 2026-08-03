using Api.Enums;
using Api.Models;
using Api.Models.Results;
using Api.Services.Abstractions;

namespace Api.Services.Implementations;

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
        var headerLine = reader.ReadLine();

        if (string.IsNullOrWhiteSpace(headerLine))
        {
            return ParseResult<List<CsvRow>>.Success(new List<CsvRow>(), 0);
        }

        var headers = ParseCsvLine(headerLine);
        var rows = new List<CsvRow>();

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var fields = ParseCsvLine(line);
            var columns = new Dictionary<string, string>();

            int maxColumns = Math.Max(headers.Count, fields.Count);

            for (int i = 0; i < maxColumns; i++)
            {
                string headerName = i < headers.Count && !string.IsNullOrWhiteSpace(headers[i])
                    ? headers[i]
                    : $"Column_{i + 1}";

                string value = i < fields.Count ? fields[i] : string.Empty;
                columns[headerName] = value;
            }

            rows.Add(new CsvRow(columns));
        }

        return ParseResult<List<CsvRow>>.Success(rows, rows.Count);
    }

    private static List<string> ParseCsvLine(string line)
    {
        var result = new List<string>();
        var inQuotes = false;
        var current = new System.Text.StringBuilder();

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current.ToString().Trim());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        result.Add(current.ToString().Trim());
        return result;
    }
}