using DiscordClone.Application.Common.Interfaces;
using DiscordClone.Domain.Entities;
using DiscordClone.Domain.Enums;
using DiscordClone.Domain.Exceptions;
using DiscordClone.Shared.Contracts.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Application.Features.Channels.Commands;

public sealed class CreateChannelCommandHandler
    : IRequestHandler<CreateChannelCommand, ChannelResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateChannelCommandHandler(
        IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ChannelResponse> Handle(
        CreateChannelCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new ForbiddenException();

        var server = await _db.Servers
            .FirstOrDefaultAsync(s => s.Id == request.ServerId, ct)
            ?? throw new NotFoundException(
                nameof(Server), request.ServerId);

        if (server.OwnerId != userId)
            throw new ForbiddenException(
                "Only the server owner can create channels.");

        if (!Enum.TryParse<ChannelType>(request.Type, true, out var channelType))
            throw new Domain.Exceptions.ValidationException(
                new Dictionary<string, string[]>
                {
                    ["Type"] = [$"Invalid channel type '{request.Type}'."]
                });

        var position = await _db.Channels
            .CountAsync(c => c.ServerId == request.ServerId, ct);

        var channel = Channel.Create(
            request.Name, channelType,
            request.ServerId, position,
            request.Topic, request.IsPrivate);

        _db.Channels.Add(channel);
        await _db.SaveChangesAsync(ct);

        return new ChannelResponse(
            channel.Id, channel.Name, channel.Topic,
            channel.Type.ToString(), channel.Position,
            channel.IsPrivate, channel.ServerId);
    }
}