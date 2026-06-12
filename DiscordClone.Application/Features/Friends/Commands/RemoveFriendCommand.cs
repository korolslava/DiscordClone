using MediatR;

namespace DiscordClone.Application.Features.Friends.Commands;

public record RemoveFriendCommand(Guid FriendUserId) : IRequest;