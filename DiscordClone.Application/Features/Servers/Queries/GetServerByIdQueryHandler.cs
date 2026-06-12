using DiscordClone.Application.Common.Interfaces;
using DiscordClone.Domain.Exceptions;
using DiscordClone.Shared.Contracts.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Application.Features.Servers.Queries;

public sealed class GetServerByIdQueryHandler
    : IRequestHandler<GetServerByIdQuery, ServerDetailResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IPresenceService _presence;

    public GetServerByIdQueryHandler(IApplicationDbContext db,
        ICurrentUserService currentUser, IPresenceService presence)
    {
        _db = db;
        _currentUser = currentUser;
        _presence = presence;
    }

    public async Task<ServerDetailResponse> Handle(
        GetServerByIdQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new ForbiddenException();

        var server = await _db.Servers
            .Include(s => s.Channels.OrderBy(c => c.Position))
            .Include(s => s.Members)
                .ThenInclude(m => m.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.ServerId, ct)
            ?? throw new NotFoundException(
                nameof(Domain.Entities.Server), request.ServerId);

        var isMember = server.Members.Any(m => m.UserId == userId);
        if (!isMember)
            throw new ForbiddenException("You are not a member of this server.");

        var channels = server.Channels
            .Select(c => new ChannelResponse(
                c.Id, c.Name, c.Topic,
                c.Type.ToString(), c.Position,
                c.IsPrivate, c.ServerId));

        var members = new List<ServerMemberResponse>();
        foreach (var m in server.Members)
        {
            var isOnline = await _presence.IsUserOnlineAsync(m.UserId);
            members.Add(new ServerMemberResponse(
                m.UserId,
                m.User.Username,
                m.User.DisplayName,
                m.User.AvatarUrl,
                m.Nickname,
                isOnline ? "Online" : m.User.Status.ToString(),
                m.JoinedAt));
        }

        return new ServerDetailResponse(
            server.Id,
            server.Name,
            server.Description,
            server.IconUrl,
            server.InviteCode,
            server.OwnerId,
            channels,
            members,
            server.CreatedAt);
    }
}