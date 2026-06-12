using DiscordClone.Shared.Contracts.Responses;
using MediatR;

namespace DiscordClone.Application.Features.Messages.Commands;

public record EditMessageCommand(
    Guid MessageId,
    string Content) : IRequest<MessageResponse>;