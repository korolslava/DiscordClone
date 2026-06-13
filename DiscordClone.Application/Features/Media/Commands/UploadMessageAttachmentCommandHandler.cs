using DiscordClone.Application.Common.Interfaces;
using DiscordClone.Domain.Entities;
using DiscordClone.Domain.Exceptions;
using DiscordClone.Shared.Contracts.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Application.Features.Media.Commands;

public sealed class UploadMessageAttachmentCommandHandler
    : IRequestHandler<UploadMessageAttachmentCommand, AttachmentResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IStorageService _storage;

    private const long MaxSizeBytes = 25 * 1024 * 1024;

    public UploadMessageAttachmentCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser,
        IStorageService storage)
    {
        _db = db;
        _currentUser = currentUser;
        _storage = storage;
    }

    public async Task<AttachmentResponse> Handle(
        UploadMessageAttachmentCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new ForbiddenException();

        if (request.FileSize > MaxSizeBytes)
            throw new ValidationException(
                new Dictionary<string, string[]>
                {
                    ["File"] = ["File size must not exceed 25MB."]
                });

        var message = await _db.Messages
            .FirstOrDefaultAsync(m => m.Id == request.MessageId, ct)
            ?? throw new NotFoundException(
                nameof(Message), request.MessageId);

        if (message.AuthorId != userId)
            throw new ForbiddenException(
                "You can only attach files to your own messages.");

        var fileUrl = await _storage.UploadAsync(
            request.FileStream,
            request.FileName,
            request.ContentType,
            ct);

        var attachment = MessageAttachment.Create(
            request.FileName,
            fileUrl,
            request.ContentType,
            request.FileSize,
            request.MessageId);

        _db.MessageAttachments.Add(attachment);
        await _db.SaveChangesAsync(ct);

        return new AttachmentResponse(
            attachment.Id,
            attachment.FileName,
            attachment.FileUrl,
            attachment.ContentType,
            attachment.FileSize);
    }
}