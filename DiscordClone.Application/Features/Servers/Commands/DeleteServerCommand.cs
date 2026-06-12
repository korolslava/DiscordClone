using MediatR;

namespace DiscordClone.Application.Features.Servers.Commands;

public record DeleteServerCommand(Guid ServerId) : IRequest;