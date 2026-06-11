using DiscordClone.Shared.Contracts.Responses;
using MediatR;

namespace DiscordClone.Application.Features.Auth.Commands;

public record RefreshTokenCommand(
    string AccessToken,
    string RefreshToken) : IRequest<AuthResponse>;