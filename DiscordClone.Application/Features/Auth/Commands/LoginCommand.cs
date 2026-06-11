using DiscordClone.Shared.Contracts.Responses;
using MediatR;

namespace DiscordClone.Application.Features.Auth.Commands;

public record LoginCommand(
    string Email,
    string Password) : IRequest<AuthResponse>;