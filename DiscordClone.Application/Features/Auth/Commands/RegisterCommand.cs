using DiscordClone.Shared.Contracts.Responses;
using MediatR;

namespace DiscordClone.Application.Features.Auth.Commands;

public record RegisterCommand(
    string Username,
    string DisplayName,
    string Email,
    string Password) : IRequest<AuthResponse>;