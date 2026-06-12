using DiscordClone.Shared.Contracts.Responses;
using MediatR;

namespace DiscordClone.Application.Features.Servers.Commands;

public record UpdateServerCommand(
    Guid ServerId,
    string Name,
    string? Description) : IRequest<ServerResponse>;