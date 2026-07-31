namespace Api.Models;

public record CsvRow(IReadOnlyDictionary<string, string> Columns);