using DiscordClone.Application.Common.Interfaces;
using DiscordClone.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Application.Features.Channels.Commands;

public sealed class DeleteChannelCommandHandler
    : IRequestHandler<DeleteChannelCommand>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeleteChannelCommandHandler(
        IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteChannelCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new ForbiddenException();

        var channel = await _db.Channels
            .Include(c => c.Server)
            .FirstOrDefaultAsync(c => c.Id == request.ChannelId, ct)
            ?? throw new NotFoundException(
                nameof(Domain.Entities.Channel), request.ChannelId);

        if (channel.Server.OwnerId != userId)
            throw new ForbiddenException(
                "Only the server owner can delete channels.");

        var channelCount = await _db.Channels
            .CountAsync(c => c.ServerId == channel.ServerId, ct);

        if (channelCount <= 1)
            throw new ConflictException(
                "Cannot delete the last channel in a server.");

        _db.Channels.Remove(channel);
        await _db.SaveChangesAsync(ct);
    }
}