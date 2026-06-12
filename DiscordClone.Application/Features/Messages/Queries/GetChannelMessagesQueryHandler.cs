using DiscordClone.Application.Common.Interfaces;
using DiscordClone.Domain.Exceptions;
using DiscordClone.Shared.Contracts.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Application.Features.Messages.Queries;

public sealed class GetChannelMessagesQueryHandler
    : IRequestHandler<GetChannelMessagesQuery, PagedResponse<MessageResponse>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetChannelMessagesQueryHandler(
        IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<PagedResponse<MessageResponse>> Handle(
        GetChannelMessagesQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new ForbiddenException();

        var channel = await _db.Channels
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.ChannelId, ct)
            ?? throw new NotFoundException(
                nameof(Domain.Entities.Channel), request.ChannelId);

        var isMember = await _db.ServerMembers
            .AnyAsync(sm =>
                sm.ServerId == channel.ServerId &&
                sm.UserId == userId, ct);

        if (!isMember)
            throw new ForbiddenException(
                "You are not a member of this server.");

        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var page = Math.Max(1, request.Page);

        var totalCount = await _db.Messages
            .CountAsync(m =>
                m.ChannelId == request.ChannelId &&
                !m.IsDeleted, ct);

        var messages = await _db.Messages
            .Where(m => m.ChannelId == request.ChannelId)
            .Include(m => m.Author)
            .Include(m => m.Attachments)
            .Include(m => m.Reactions)
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(ct);

        var items = messages
            .OrderBy(m => m.CreatedAt)
            .Select(m => new MessageResponse(
                m.Id,
                m.Content,
                m.IsEdited,
                m.IsDeleted,
                new UserResponse(
                    m.Author.Id,
                    m.Author.Username,
                    m.Author.DisplayName,
                    m.Author.AvatarUrl,
                    m.Author.About,
                    m.Author.Status.ToString()),
                m.ChannelId,
                m.DirectMessageId,
                m.ReplyToMessageId,
                m.Attachments.Select(a => new AttachmentResponse(
                    a.Id, a.FileName, a.FileUrl, a.ContentType, a.FileSize)),
                m.Reactions
                    .GroupBy(r => r.Emoji)
                    .Select(g => new ReactionResponse(
                        g.Key,
                        g.Count(),
                        g.Any(r => r.UserId == userId))),
                m.CreatedAt,
                m.UpdatedAt));

        return new PagedResponse<MessageResponse>(
            items,
            totalCount,
            page,
            pageSize,
            HasNextPage: page * pageSize < totalCount,
            HasPreviousPage: page > 1);
    }
}