using DiscordClone.Application.Common.Interfaces;
using DiscordClone.Domain.Exceptions;
using DiscordClone.Shared.Contracts.Events;
using DiscordClone.Shared.Contracts.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Application.Features.Messages.Commands;

public sealed class EditMessageCommandHandler
    : IRequestHandler<EditMessageCommand, MessageResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService _notifications;

    public EditMessageCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser,
        INotificationService notifications)
    {
        _db = db;
        _currentUser = currentUser;
        _notifications = notifications;
    }

    public async Task<MessageResponse> Handle(
        EditMessageCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new ForbiddenException();

        var message = await _db.Messages
            .Include(m => m.Author)
            .Include(m => m.Attachments)
            .Include(m => m.Reactions)
            .FirstOrDefaultAsync(m => m.Id == request.MessageId, ct)
            ?? throw new NotFoundException(
                nameof(Domain.Entities.Message), request.MessageId);

        if (message.AuthorId != userId)
            throw new ForbiddenException(
                "You can only edit your own messages.");

        if (message.IsDeleted)
            throw new ConflictException("Cannot edit a deleted message.");

        message.Edit(request.Content);
        await _db.SaveChangesAsync(ct);

        var response = MapToResponse(message);

        if (message.ChannelId.HasValue)
            await _notifications.SendToChannelGroupAsync(
                message.ChannelId.Value,
                RealtimeEvents.MessageEdited,
                response);

        return response;
    }

    private static MessageResponse MapToResponse(
        Domain.Entities.Message message) => new(
            message.Id,
            message.Content,
            message.IsEdited,
            message.IsDeleted,
            new UserResponse(
                message.Author.Id,
                message.Author.Username,
                message.Author.DisplayName,
                message.Author.AvatarUrl,
                message.Author.About,
                message.Author.Status.ToString()),
            message.ChannelId,
            message.DirectMessageId,
            message.ReplyToMessageId,
            message.Attachments.Select(a => new AttachmentResponse(
                a.Id, a.FileName, a.FileUrl, a.ContentType, a.FileSize)),
            message.Reactions
                .GroupBy(r => r.Emoji)
                .Select(g => new ReactionResponse(g.Key, g.Count(), false)),
            message.CreatedAt,
            message.UpdatedAt);
}