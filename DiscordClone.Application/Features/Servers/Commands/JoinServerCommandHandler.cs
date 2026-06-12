using DiscordClone.Application.Common.Interfaces;
using DiscordClone.Domain.Entities;
using DiscordClone.Domain.Exceptions;
using DiscordClone.Shared.Contracts.Events;
using DiscordClone.Shared.Contracts.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Application.Features.Servers.Commands;

public sealed class JoinServerCommandHandler
    : IRequestHandler<JoinServerCommand, ServerResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService _notifications;

    public JoinServerCommandHandler(IApplicationDbContext db,
        ICurrentUserService currentUser, INotificationService notifications)
    {
        _db = db;
        _currentUser = currentUser;
        _notifications = notifications;
    }

    public async Task<ServerResponse> Handle(
        JoinServerCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new ForbiddenException();

        var server = await _db.Servers
            .FirstOrDefaultAsync(s => s.InviteCode == request.InviteCode, ct)
            ?? throw new NotFoundException("Server", request.InviteCode);

        var alreadyMember = await _db.ServerMembers
            .AnyAsync(sm => sm.ServerId == server.Id && sm.UserId == userId, ct);

        if (alreadyMember)
            throw new ConflictException("You are already a member of this server.");

        var member = ServerMember.Create(userId, server.Id);
        _db.ServerMembers.Add(member);
        await _db.SaveChangesAsync(ct);

        var memberCount = await _db.ServerMembers
            .CountAsync(sm => sm.ServerId == server.Id, ct);

        await _notifications.SendToServerGroupAsync(server.Id,
            RealtimeEvents.MemberJoined, new { UserId = userId, ServerId = server.Id });

        return new ServerResponse(
            server.Id,
            server.Name,
            server.Description,
            server.IconUrl,
            server.InviteCode,
            server.OwnerId,
            memberCount,
            server.CreatedAt);
    }
}