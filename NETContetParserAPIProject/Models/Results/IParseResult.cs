namespace Api.Models.Results;

public interface IParseResult
{
    bool IsSuccess { get; }
    int Count { get; }
    object? RawData { get; }
    string? ErrorMessage { get; }
}