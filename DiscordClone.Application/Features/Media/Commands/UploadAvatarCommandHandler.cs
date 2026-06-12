using DiscordClone.Application.Common.Interfaces;
using DiscordClone.Domain.Exceptions;
using DiscordClone.Shared.Contracts.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Application.Features.Media.Commands;

public sealed class UploadAvatarCommandHandler
    : IRequestHandler<UploadAvatarCommand, UserResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IStorageService _storage;

    private static readonly string[] AllowedTypes =
        ["image/jpeg", "image/png", "image/gif", "image/webp"];

    private const long MaxSizeBytes = 8 * 1024 * 1024; // 8MB

    public UploadAvatarCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser,
        IStorageService storage)
    {
        _db = db;
        _currentUser = currentUser;
        _storage = storage;
    }

    public async Task<UserResponse> Handle(
        UploadAvatarCommand request, CancellationToken ct)
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

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == userId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.User), userId);

        if (user.AvatarUrl is not null)
            await _storage.DeleteAsync(user.AvatarUrl, ct);

        var avatarUrl = await _storage.UploadAsync(
            request.FileStream,
            request.FileName,
            request.ContentType,
            ct);

        user.SetAvatar(avatarUrl);
        await _db.SaveChangesAsync(ct);

        return new UserResponse(
            user.Id,
            user.Username,
            user.DisplayName,
            user.AvatarUrl,
            user.About,
            user.Status.ToString());
    }
}