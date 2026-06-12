using MediatR;

namespace DiscordClone.Application.Features.Friends.Commands;

public record RespondFriendRequestCommand(
    Guid RequestId,
    bool Accept) : IRequest;