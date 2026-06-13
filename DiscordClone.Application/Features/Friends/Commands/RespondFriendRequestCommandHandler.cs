using DiscordClone.Application.Common.Interfaces;
using DiscordClone.Domain.Entities;
using DiscordClone.Domain.Enums;
using DiscordClone.Domain.Exceptions;
using DiscordClone.Shared.Contracts.Events;
using DiscordClone.Shared.Contracts.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Application.Features.Friends.Commands;

public sealed class RespondFriendRequestCommandHandler
    : IRequestHandler<RespondFriendRequestCommand>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService _notifications;

    public RespondFriendRequestCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser,
        INotificationService notifications)
    {
        _db = db;
        _currentUser = currentUser;
        _notifications = notifications;
    }

    public async Task Handle(
        RespondFriendRequestCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new ForbiddenException();

        var friendRequest = await _db.FriendRequests
            .Include(fr => fr.Sender)
            .Include(fr => fr.Receiver)
            .FirstOrDefaultAsync(fr => fr.Id == request.RequestId, ct)
            ?? throw new NotFoundException("FriendRequest", request.RequestId);

        if (friendRequest.ReceiverId != userId)
            throw new ForbiddenException(
                "You can only respond to your own friend requests.");

        if (friendRequest.Status != FriendRequestStatus.Pending)
            throw new ConflictException(
                "This friend request has already been responded to.");

        if (request.Accept)
        {
            friendRequest.Accept();

            var dm = DirectMessage.Create();
            _db.DirectMessages.Add(dm);

            _db.DirectMessageParticipants.Add(
                DirectMessageParticipant.Create(
                    friendRequest.SenderId, dm.Id));
            _db.DirectMessageParticipants.Add(
                DirectMessageParticipant.Create(
                    friendRequest.ReceiverId, dm.Id));

            await _db.SaveChangesAsync(ct);

            await _notifications.SendToUserAsync(
                friendRequest.SenderId,
                RealtimeEvents.FriendRequestAccepted,
                new UserResponse(
                    friendRequest.Receiver.Id,
                    friendRequest.Receiver.Username,
                    friendRequest.Receiver.DisplayName,
                    friendRequest.Receiver.AvatarUrl,
                    friendRequest.Receiver.About,
                    friendRequest.Receiver.Status.ToString()));
        }
        else
        {
            friendRequest.Decline();
            await _db.SaveChangesAsync(ct);
        }
    }
}