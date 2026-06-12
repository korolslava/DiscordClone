using DiscordClone.Application.Common.Interfaces;
using DiscordClone.Domain.Enums;
using DiscordClone.Domain.Exceptions;
using DiscordClone.Shared.Contracts.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Application.Features.Friends.Queries;

public sealed class GetPendingRequestsQueryHandler
    : IRequestHandler<GetPendingRequestsQuery,
        IEnumerable<FriendRequestResponse>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetPendingRequestsQueryHandler(
        IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<FriendRequestResponse>> Handle(
        GetPendingRequestsQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new ForbiddenException();

        return await _db.FriendRequests
            .Where(fr =>
                fr.Status == FriendRequestStatus.Pending &&
                fr.ReceiverId == userId)
            .Include(fr => fr.Sender)
            .Include(fr => fr.Receiver)
            .AsNoTracking()
            .Select(fr => new FriendRequestResponse(
                fr.Id,
                new UserResponse(
                    fr.Sender.Id,
                    fr.Sender.Username,
                    fr.Sender.DisplayName,
                    fr.Sender.AvatarUrl,
                    fr.Sender.About,
                    fr.Sender.Status.ToString()),
                new UserResponse(
                    fr.Receiver.Id,
                    fr.Receiver.Username,
                    fr.Receiver.DisplayName,
                    fr.Receiver.AvatarUrl,
                    fr.Receiver.About,
                    fr.Receiver.Status.ToString()),
                fr.Status.ToString(),
                fr.CreatedAt))
            .ToListAsync(ct);
    }
}