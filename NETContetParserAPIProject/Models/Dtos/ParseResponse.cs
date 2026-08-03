namespace Api.Models.Dtos;

public record ParseResponse<T>(
    bool IsSuccess,
    int RecordCount,
    T? Data,
    string? ErrorMessage = null
)
{
    public static ParseResponse<T> Success(T data, int count) =>
        new(true, count, data);

    public static ParseResponse<T> Failure(string message) =>
        new(false, 0, default, message);
}