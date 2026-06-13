using System.Net;
using System.Text.Json;
using DiscordClone.Domain.Exceptions;

namespace DiscordClone.API.Middleware;

public sealed class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ExceptionMiddleware(RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleAsync(context, ex);
        }
    }

    private static async Task HandleAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (status, message, errors) = exception switch
        {
            NotFoundException ex => (HttpStatusCode.NotFound,
                                        ex.Message, (IDictionary<string, string[]>?)null),
            ForbiddenException ex => (HttpStatusCode.Forbidden,
                                        ex.Message, null),
            ConflictException ex => (HttpStatusCode.Conflict,
                                        ex.Message, null),
            ValidationException ex => (HttpStatusCode.UnprocessableEntity,
                                        "Validation failed.", ex.Errors),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized,
                                        "Unauthorized.", null),
            _ => (HttpStatusCode.InternalServerError,
                                        "An unexpected error occurred.", null)
        };

        context.Response.StatusCode = (int)status;

        await context.Response.WriteAsync(JsonSerializer.Serialize(new
        {
            success = false,
            message,
            errors
        }, JsonOptions));
    }
}