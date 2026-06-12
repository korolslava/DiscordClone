using DiscordClone.Application.Common.Interfaces;
using DiscordClone.Domain.Exceptions;
using DiscordClone.Shared.Contracts.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Application.Features.Servers.Queries;

public sealed class GetMyServersQueryHandler
    : IRequestHandler<GetMyServersQuery, IEnumerable<ServerResponse>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetMyServersQueryHandler(
        IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<ServerResponse>> Handle(
        GetMyServersQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new ForbiddenException();

        return await _db.ServerMembers
            .Where(sm => sm.UserId == userId)
            .Include(sm => sm.Server)
            .Select(sm => new ServerResponse(
                sm.Server.Id,
                sm.Server.Name,
                sm.Server.Description,
                sm.Server.IconUrl,
                sm.Server.InviteCode,
                sm.Server.OwnerId,
                _db.ServerMembers.Count(m => m.ServerId == sm.ServerId),
                sm.Server.CreatedAt))
            .AsNoTracking()
            .ToListAsync(ct);
    }
}