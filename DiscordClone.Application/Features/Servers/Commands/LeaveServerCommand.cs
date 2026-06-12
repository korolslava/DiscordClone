using MediatR;

namespace DiscordClone.Application.Features.Servers.Commands;

public record LeaveServerCommand(Guid ServerId) : IRequest;