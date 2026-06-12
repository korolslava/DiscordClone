using DiscordClone.Application.Common.Interfaces;
using DiscordClone.Domain.Exceptions;
using DiscordClone.Shared.Contracts.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Application.Features.Messages.Commands;

public sealed class RemoveReactionCommandHandler
    : IRequestHandler<RemoveReactionCommand>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService _notifications;

    public RemoveReactionCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser,
        INotificationService notifications)
    {
        _db = db;
        _currentUser = currentUser;
        _notifications = notifications;
    }

    public async Task Handle(RemoveReactionCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new ForbiddenException();

        var reaction = await _db.MessageReactions
            .FirstOrDefaultAsync(r =>
                r.MessageId == request.MessageId &&
                r.UserId == userId &&
                r.Emoji == request.Emoji, ct)
            ?? throw new NotFoundException("Reaction", request.Emoji);

        var message = await _db.Messages
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == request.MessageId, ct);

        _db.MessageReactions.Remove(reaction);
        await _db.SaveChangesAsync(ct);

        if (message?.ChannelId.HasValue == true)
            await _notifications.SendToChannelGroupAsync(
                message.ChannelId.Value,
                RealtimeEvents.ReactionRemoved,
                new
                {
                    MessageId = request.MessageId,
                    Emoji = request.Emoji,
                    UserId = userId
                });
    }
}