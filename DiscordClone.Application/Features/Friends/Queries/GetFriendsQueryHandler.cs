using DiscordClone.Application.Common.Interfaces;
using DiscordClone.Domain.Enums;
using DiscordClone.Domain.Exceptions;
using DiscordClone.Shared.Contracts.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Application.Features.Friends.Queries;

public sealed class GetFriendsQueryHandler
    : IRequestHandler<GetFriendsQuery, IEnumerable<FriendResponse>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IPresenceService _presence;

    public GetFriendsQueryHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser,
        IPresenceService presence)
    {
        _db = db;
        _currentUser = currentUser;
        _presence = presence;
    }

    public async Task<IEnumerable<FriendResponse>> Handle(
        GetFriendsQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new ForbiddenException();

        var friendRequests = await _db.FriendRequests
            .Where(fr =>
                fr.Status == FriendRequestStatus.Accepted &&
                (fr.SenderId == userId || fr.ReceiverId == userId))
            .Include(fr => fr.Sender)
            .Include(fr => fr.Receiver)
            .AsNoTracking()
            .ToListAsync(ct);

        var result = new List<FriendResponse>();

        foreach (var fr in friendRequests)
        {
            var friend = fr.SenderId == userId ? fr.Receiver : fr.Sender;
            var isOnline = await _presence.IsUserOnlineAsync(friend.Id);

            var dm = await _db.DirectMessageParticipants
                .Where(p => p.UserId == userId)
                .Join(_db.DirectMessageParticipants
                    .Where(p => p.UserId == friend.Id),
                    p1 => p1.DirectMessageId,
                    p2 => p2.DirectMessageId,
                    (p1, p2) => p1.DirectMessageId)
                .FirstOrDefaultAsync(ct);

            result.Add(new FriendResponse(
                friend.Id,
                friend.Username,
                friend.DisplayName,
                friend.AvatarUrl,
                isOnline ? "Online" : friend.Status.ToString(),
                dm == Guid.Empty ? null : dm));
        }

        return result;
    }
}