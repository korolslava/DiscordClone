using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DiscordClone.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DiscordClone.Infrastructure.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    private ClaimsPrincipal? User =>
        _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var value = User?.FindFirstValue(JwtRegisteredClaimNames.Sub)
                     ?? User?.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? Username =>
        User?.FindFirstValue(JwtRegisteredClaimNames.UniqueName)
        ?? User?.FindFirstValue(ClaimTypes.Name);

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated ?? false;
}