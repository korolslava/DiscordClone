namespace DiscordClone.API.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerWithJwt(
        this IServiceCollection services)
    {
        // Configure basic Swagger generation. Detailed OpenAPI objects are
        // intentionally omitted to avoid dependency/version conflicts with
        // the Microsoft.OpenApi package in this workspace.
        services.AddSwaggerGen();
        return services;
    }
}
