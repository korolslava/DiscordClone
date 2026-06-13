using DiscordClone.Shared.Contracts.Responses;
using MediatR;

namespace DiscordClone.Application.Features.Channels.Commands;

public record CreateChannelCommand(
    Guid ServerId,
    string Name,
    string Type,
    string? Topic,
    bool IsPrivate) : IRequest<ChannelResponse>;