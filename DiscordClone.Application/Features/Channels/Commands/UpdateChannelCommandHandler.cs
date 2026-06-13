using DiscordClone.Application.Common.Interfaces;
using DiscordClone.Domain.Exceptions;
using DiscordClone.Shared.Contracts.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Application.Features.Channels.Commands;

public sealed class UpdateChannelCommandHandler
    : IRequestHandler<UpdateChannelCommand, ChannelResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateChannelCommandHandler(
        IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ChannelResponse> Handle(
        UpdateChannelCommand request, CancellationToken ct)
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
                "Only the server owner can update channels.");

        channel.Update(request.Name, request.Topic);
        await _db.SaveChangesAsync(ct);

        return new ChannelResponse(
            channel.Id, channel.Name, channel.Topic,
            channel.Type.ToString(), channel.Position,
            channel.IsPrivate, channel.ServerId);
    }
}