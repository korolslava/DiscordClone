using MediatR;

namespace DiscordClone.Application.Features.Messages.Commands;

public record DeleteMessageCommand(Guid MessageId) : IRequest;