using MediatR;

namespace DiscordClone.Application.Features.Friends.Commands;

public record SendFriendRequestCommand(string Username) : IRequest;