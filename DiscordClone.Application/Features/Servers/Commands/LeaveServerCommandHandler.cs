using DiscordClone.Application.Common.Interfaces;
using DiscordClone.Domain.Exceptions;
using DiscordClone.Shared.Contracts.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Application.Features.Servers.Commands;

public sealed class LeaveServerCommandHandler
    : IRequestHandler<LeaveServerCommand>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService _notifications;

    public LeaveServerCommandHandler(IApplicationDbContext db,
        ICurrentUserService currentUser, INotificationService notifications)
    {
        _db = db;
        _currentUser = currentUser;
        _notifications = notifications;
    }

    public async Task Handle(LeaveServerCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new ForbiddenException();

        var server = await _db.Servers
            .FirstOrDefaultAsync(s => s.Id == request.ServerId, ct)
            ?? throw new NotFoundException(
                nameof(Domain.Entities.Server), request.ServerId);

        if (server.OwnerId == userId)
            throw new ForbiddenException(
                "Server owner cannot leave. Transfer ownership or delete the server.");

        var member = await _db.ServerMembers
            .FirstOrDefaultAsync(
                sm => sm.ServerId == request.ServerId && sm.UserId == userId, ct)
            ?? throw new NotFoundException("ServerMember", userId);

        _db.ServerMembers.Remove(member);
        await _db.SaveChangesAsync(ct);

        await _notifications.SendToServerGroupAsync(request.ServerId,
            RealtimeEvents.MemberLeft, new { UserId = userId, ServerId = request.ServerId });
    }
}