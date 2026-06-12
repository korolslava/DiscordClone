using DiscordClone.Application.Common.Interfaces;
using DiscordClone.Domain.Exceptions;
using DiscordClone.Shared.Contracts.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Application.Features.Media.Commands;

public sealed class UploadServerIconCommandHandler
    : IRequestHandler<UploadServerIconCommand, ServerResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IStorageService _storage;

    private static readonly string[] AllowedTypes =
        ["image/jpeg", "image/png", "image/gif", "image/webp"];

    private const long MaxSizeBytes = 8 * 1024 * 1024;

    public UploadServerIconCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser,
        IStorageService storage)
    {
        _db = db;
        _currentUser = currentUser;
        _storage = storage;
    }

    public async Task<ServerResponse> Handle(
        UploadServerIconCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new ForbiddenException();

        if (!AllowedTypes.Contains(request.ContentType))
            throw new ValidationException(
                new Dictionary<string, string[]>
                {
                    ["File"] = ["Only JPEG, PNG, GIF and WebP images are allowed."]
                });

        if (request.FileSize > MaxSizeBytes)
            throw new ValidationException(
                new Dictionary<string, string[]>
                {
                    ["File"] = ["File size must not exceed 8MB."]
                });

        var server = await _db.Servers
            .FirstOrDefaultAsync(s => s.Id == request.ServerId, ct)
            ?? throw new NotFoundException(
                nameof(Domain.Entities.Server), request.ServerId);

        if (server.OwnerId != userId)
            throw new ForbiddenException(
                "Only the server owner can change the server icon.");

        if (server.IconUrl is not null)
            await _storage.DeleteAsync(server.IconUrl, ct);

        var iconUrl = await _storage.UploadAsync(
            request.FileStream,
            request.FileName,
            request.ContentType,
            ct);

        server.SetIcon(iconUrl);
        await _db.SaveChangesAsync(ct);

        var memberCount = await _db.ServerMembers
            .CountAsync(sm => sm.ServerId == server.Id, ct);

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