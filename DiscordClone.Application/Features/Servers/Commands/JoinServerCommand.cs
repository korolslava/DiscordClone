using DiscordClone.Shared.Contracts.Responses;
using MediatR;

namespace DiscordClone.Application.Features.Servers.Commands;

public record JoinServerCommand(string InviteCode) : IRequest<ServerResponse>;