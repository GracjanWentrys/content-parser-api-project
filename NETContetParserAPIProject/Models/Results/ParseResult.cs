namespace Api.Models.Results;

public record ParseResult<T>(
    bool IsSuccess,
    int Count,
    T? Data,
    string? ErrorMessage = null
) : IParseResult
{
    public object? RawData => Data;

    public static ParseResult<T> Success(T data, int count) =>
        new(true, count, data);

    public static ParseResult<T> Failure(string errorMessage) =>
        new(false, 0, default, errorMessage);
}