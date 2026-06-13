using DiscordClone.Shared.Contracts.Responses;
using MediatR;

namespace DiscordClone.Application.Features.Servers.Commands;

public record CreateServerCommand(
    string Name,
    string? Description) : IRequest<ServerResponse>;