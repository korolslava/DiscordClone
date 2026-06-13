using DiscordClone.Application.Common.Interfaces;
using DiscordClone.Domain.Entities;
using DiscordClone.Domain.Exceptions;
using DiscordClone.Shared.Contracts.Events;
using DiscordClone.Shared.Contracts.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Application.Features.Friends.Commands;

public sealed class SendFriendRequestCommandHandler
    : IRequestHandler<SendFriendRequestCommand>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService _notifications;

    public SendFriendRequestCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser,
        INotificationService notifications)
    {
        _db = db;
        _currentUser = currentUser;
        _notifications = notifications;
    }

    public async Task Handle(
        SendFriendRequestCommand request, CancellationToken ct)
    {
        var senderId = _currentUser.UserId
            ?? throw new ForbiddenException();

        var receiver = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == request.Username.ToLower(), ct)
            ?? throw new NotFoundException("User", request.Username);

        if (receiver.Id == senderId)
            throw new ConflictException(
                "You cannot send a friend request to yourself.");

        var exists = await _db.FriendRequests
            .AnyAsync(fr =>
                (fr.SenderId == senderId && fr.ReceiverId == receiver.Id) ||
                (fr.SenderId == receiver.Id && fr.ReceiverId == senderId), ct);

        if (exists)
            throw new ConflictException(
                "A friend request already exists between these users.");

        var friendRequest = FriendRequest.Create(senderId, receiver.Id);
        _db.FriendRequests.Add(friendRequest);
        await _db.SaveChangesAsync(ct);

        var sender = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == senderId, ct);

        await _notifications.SendToUserAsync(
            receiver.Id,
            RealtimeEvents.FriendRequestReceived,
            new FriendRequestResponse(
                friendRequest.Id,
                new UserResponse(
                    senderId,
                    sender!.Username,
                    sender.DisplayName,
                    sender.AvatarUrl,
                    sender.About,
                    sender.Status.ToString()),
                new UserResponse(
                    receiver.Id,
                    receiver.Username,
                    receiver.DisplayName,
                    receiver.AvatarUrl,
                    receiver.About,
                    receiver.Status.ToString()),
                friendRequest.Status.ToString(),
                friendRequest.CreatedAt));
    }
}