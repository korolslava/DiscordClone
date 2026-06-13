using DiscordClone.Application.Common.Interfaces;
using DiscordClone.Domain.Exceptions;
using DiscordClone.Shared.Contracts.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Application.Features.Messages.Commands;

public sealed class DeleteMessageCommandHandler
    : IRequestHandler<DeleteMessageCommand>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService _notifications;

    public DeleteMessageCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser,
        INotificationService notifications)
    {
        _db = db;
        _currentUser = currentUser;
        _notifications = notifications;
    }

    public async Task Handle(
        DeleteMessageCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new ForbiddenException();

        var message = await _db.Messages
            .Include(m => m.Channel)
                .ThenInclude(c => c!.Server)
            .FirstOrDefaultAsync(m => m.Id == request.MessageId, ct)
            ?? throw new NotFoundException(
                nameof(Domain.Entities.Message), request.MessageId);

        var isAuthor = message.AuthorId == userId;
        var isServerOwner = message.Channel?.Server.OwnerId == userId;

        if (!isAuthor && !isServerOwner)
            throw new ForbiddenException(
                "You do not have permission to delete this message.");

        message.SoftDelete();
        await _db.SaveChangesAsync(ct);

        if (message.ChannelId.HasValue)
            await _notifications.SendToChannelGroupAsync(
                message.ChannelId.Value,
                RealtimeEvents.MessageDeleted,
                new { MessageId = message.Id, ChannelId = message.ChannelId });
    }
}