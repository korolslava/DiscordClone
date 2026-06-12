using DiscordClone.Application.Common.Interfaces;
using DiscordClone.Domain.Enums;
using DiscordClone.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Application.Features.Friends.Commands;

public sealed class RemoveFriendCommandHandler
    : IRequestHandler<RemoveFriendCommand>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public RemoveFriendCommandHandler(
        IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task Handle(RemoveFriendCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new ForbiddenException();

        var friendRequest = await _db.FriendRequests
            .FirstOrDefaultAsync(fr =>
                fr.Status == FriendRequestStatus.Accepted &&
                ((fr.SenderId == userId && fr.ReceiverId == request.FriendUserId) ||
                 (fr.SenderId == request.FriendUserId && fr.ReceiverId == userId)), ct)
            ?? throw new NotFoundException("Friendship", request.FriendUserId);

        _db.FriendRequests.Remove(friendRequest);
        await _db.SaveChangesAsync(ct);
    }
}