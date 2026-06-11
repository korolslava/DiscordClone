using Microsoft.Extensions.Configuration;
using DiscordClone.Application.Common.Interfaces;
using DiscordClone.Domain.Entities;
using DiscordClone.Domain.Exceptions;
using DiscordClone.Shared.Contracts.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Application.Features.Auth.Commands;

public sealed class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly ITokenService _tokens;
    private readonly IConfiguration _config;

    public RegisterCommandHandler(IApplicationDbContext db,
        ITokenService tokens, IConfiguration config)
    {
        _db = db;
        _tokens = tokens;
        _config = config;
    }

    public async Task<AuthResponse> Handle(
        RegisterCommand request, CancellationToken ct)
    {
        var emailTaken = await _db.Users
            .AnyAsync(u => u.Email == request.Email.ToLower(), ct);
        if (emailTaken)
            throw new ConflictException("Email is already taken.");

        var usernameTaken = await _db.Users
            .AnyAsync(u => u.Username == request.Username.ToLower(), ct);
        if (usernameTaken)
            throw new ConflictException("Username is already taken.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = User.Create(
            request.Username,
            request.DisplayName,
            request.Email,
            passwordHash);

        var accessToken = _tokens.GenerateAccessToken(user);
        var refreshToken = _tokens.GenerateRefreshToken();
        var expiryDays = int.Parse(_config["Jwt:RefreshTokenExpiryDays"]!);
        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(expiryDays));

        _db.Users.Add(user);
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