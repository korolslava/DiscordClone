using MediatR;

namespace DiscordClone.Application.Features.Messages.Commands;

public record AddReactionCommand(
    Guid MessageId,
    string Emoji) : IRequest;