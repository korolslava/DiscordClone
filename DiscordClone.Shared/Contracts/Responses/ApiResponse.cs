namespace DiscordClone.Shared.Contracts.Responses;

public record ApiResponse<T>(
    bool Success,
    string? Message,
    T? Data,
    IDictionary<string, string[]>? Errors = null)
{
    public static ApiResponse<T> Ok(T data, string? message = null) =>
        new(true, message, data);

    public static ApiResponse<T> Fail(string message,
        IDictionary<string, string[]>? errors = null) =>
        new(false, message, default, errors);
}