using MediatR;

namespace DiscordClone.Application.Features.Channels.Commands;

public record DeleteChannelCommand(Guid ChannelId) : IRequest;