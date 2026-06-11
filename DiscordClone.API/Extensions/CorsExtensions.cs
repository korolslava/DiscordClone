namespace DiscordClone.API.Extensions;

public static class CorsExtensions
{
    public const string PolicyName = "CorsPolicy";

    public static IServiceCollection AddCorsPolicy(
        this IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration
            .GetSection("AllowedOrigins")
            .Get<string[]>() ?? ["http://localhost:3000"];

        services.AddCors(opt =>
            opt.AddPolicy(PolicyName, policy =>
                policy
                    .WithOrigins(origins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()));

        return services;
    }
}