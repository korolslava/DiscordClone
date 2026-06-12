using DiscordClone.Shared.Contracts.Responses;
using MediatR;

namespace DiscordClone.Application.Features.Channels.Commands;

public record UpdateChannelCommand(
    Guid ChannelId,
    string Name,
    string? Topic) : IRequest<ChannelResponse>;