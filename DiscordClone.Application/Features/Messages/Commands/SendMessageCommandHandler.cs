using DiscordClone.Application.Common.Interfaces;
using DiscordClone.Domain.Entities;
using DiscordClone.Domain.Exceptions;
using DiscordClone.Shared.Contracts.Events;
using DiscordClone.Shared.Contracts.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Application.Features.Messages.Commands;

public sealed class SendMessageCommandHandler
    : IRequestHandler<SendMessageCommand, MessageResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService _notifications;

    public SendMessageCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser,
        INotificationService notifications)
    {
        _db = db;
        _currentUser = currentUser;
        _notifications = notifications;
    }

    public async Task<MessageResponse> Handle(
        SendMessageCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new ForbiddenException();

        var channel = await _db.Channels
            .Include(c => c.Server)
            .FirstOrDefaultAsync(c => c.Id == request.ChannelId, ct)
            ?? throw new NotFoundException(
                nameof(Channel), request.ChannelId);

        var isMember = await _db.ServerMembers
            .AnyAsync(sm =>
                sm.ServerId == channel.ServerId &&
                sm.UserId == userId, ct);

        if (!isMember)
            throw new ForbiddenException(
                "You are not a member of this server.");

        if (request.ReplyToMessageId.HasValue)
        {
            var replyExists = await _db.Messages
                .AnyAsync(m =>
                    m.Id == request.ReplyToMessageId &&
                    m.ChannelId == request.ChannelId, ct);

            if (!replyExists)
                throw new NotFoundException(
                    "ReplyToMessage", request.ReplyToMessageId);
        }

        var author = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, ct)
            ?? throw new NotFoundException(nameof(User), userId);

        var message = Message.CreateChannelMessage(
            request.Content,
            userId,
            request.ChannelId,
            request.ReplyToMessageId);

        _db.Messages.Add(message);
        await _db.SaveChangesAsync(ct);

        var response = BuildResponse(message, author);

        await _notifications.SendToChannelGroupAsync(
            request.ChannelId,
            RealtimeEvents.MessageReceived,
            response);

        return response;
    }

    private static MessageResponse BuildResponse(Message message, User author) =>
        new(
            message.Id,
            message.Content,
            message.IsEdited,
            message.IsDeleted,
            new UserResponse(
                author.Id,
                author.Username,
                author.DisplayName,
                author.AvatarUrl,
                author.About,
                author.Status.ToString()),
            message.ChannelId,
            message.DirectMessageId,
            message.ReplyToMessageId,
            [],
            [],
            message.CreatedAt,
            message.UpdatedAt);
}