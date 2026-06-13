using Microsoft.Extensions.Configuration;
using DiscordClone.Application.Common.Interfaces;
using DiscordClone.Domain.Exceptions;
using DiscordClone.Shared.Contracts.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DiscordClone.Application.Features.Auth.Commands;

public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly ITokenService _tokens;
    private readonly IConfiguration _config;

    public LoginCommandHandler(IApplicationDbContext db,
        ITokenService tokens, IConfiguration config)
    {
        _db = db;
        _tokens = tokens;
        _config = config;
    }

    public async Task<AuthResponse> Handle(
        LoginCommand request, CancellationToken ct)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower(), ct)
            ?? throw new ForbiddenException("Invalid email or password.");

        var validPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!validPassword)
            throw new ForbiddenException("Invalid email or password.");

        var accessToken = _tokens.GenerateAccessToken(user);
        var refreshToken = _tokens.GenerateRefreshToken();
        var expiryDays = int.Parse(_config["Jwt:RefreshTokenExpiryDays"]!);
        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(expiryDays));

        await _db.SaveChangesAsync(ct);

        return new AuthResponse(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddMinutes(
                int.Parse(_config["Jwt:AccessTokenExpiryMinutes"]!)),
            new UserResponse(
                user.Id,
                user.Username,
                user.DisplayName,
                user.AvatarUrl,
                user.About,
                user.Status.ToString()));
    }
}