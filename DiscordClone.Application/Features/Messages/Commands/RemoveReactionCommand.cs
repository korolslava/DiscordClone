using MediatR;

namespace DiscordClone.Application.Features.Messages.Commands;

public record RemoveReactionCommand(
    Guid MessageId,
    string Emoji) : IRequest;