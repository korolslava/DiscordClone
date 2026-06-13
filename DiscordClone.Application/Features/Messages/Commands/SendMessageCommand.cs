using DiscordClone.Shared.Contracts.Responses;
using MediatR;

namespace DiscordClone.Application.Features.Messages.Commands;

public record SendMessageCommand(
    Guid ChannelId,
    string Content,
    Guid? ReplyToMessageId) : IRequest<MessageResponse>;