using DiscordClone.Application.Common.Interfaces;
using DiscordClone.Domain.Entities;
using DiscordClone.Domain.Exceptions;
using DiscordClone.Shared.Contracts.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Application.Features.Messages.Commands;

public sealed class AddReactionCommandHandler
    : IRequestHandler<AddReactionCommand>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService _notifications;

    public AddReactionCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser,
        INotificationService notifications)
    {
        _db = db;
        _currentUser = currentUser;
        _notifications = notifications;
    }

    public async Task Handle(AddReactionCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new ForbiddenException();

        var message = await _db.Messages
            .FirstOrDefaultAsync(m => m.Id == request.MessageId, ct)
            ?? throw new NotFoundException(
                nameof(Message), request.MessageId);

        var alreadyReacted = await _db.MessageReactions
            .AnyAsync(r =>
                r.MessageId == request.MessageId &&
                r.UserId == userId &&
                r.Emoji == request.Emoji, ct);

        if (alreadyReacted)
            throw new ConflictException(
                "You have already reacted with this emoji.");

        var reaction = MessageReaction.Create(
            request.Emoji, userId, request.MessageId);

        _db.MessageReactions.Add(reaction);
        await _db.SaveChangesAsync(ct);

        if (message.ChannelId.HasValue)
            await _notifications.SendToChannelGroupAsync(
                message.ChannelId.Value,
                RealtimeEvents.ReactionAdded,
                new
                {
                    MessageId = message.Id,
                    Emoji = request.Emoji,
                    UserId = userId
                });
    }
}