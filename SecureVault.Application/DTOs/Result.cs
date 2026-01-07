namespace SecureVault.Application.DTOs;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string? Error { get; private set; }
    public List<string> ValidationErrors { get; private set; } = new();

    public static Result<T> Success(T data) => new()
    {
        IsSuccess = true,
        Data = data
    };

    public static Result<T> Failure(string error) => new()
    {
        IsSuccess = false,
        Error = error
    };

    public static Result<T> ValidationFailure(List<string> errors) => new()
    {
        IsSuccess = false,
        ValidationErrors = errors,
        Error = "Validation failed"
    };
}