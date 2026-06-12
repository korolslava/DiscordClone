using DiscordClone.Application.Common.Interfaces;
using DiscordClone.Domain.Entities;
using DiscordClone.Domain.Exceptions;
using DiscordClone.Shared.Contracts.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Application.Features.Servers.Commands;

public sealed class CreateServerCommandHandler
    : IRequestHandler<CreateServerCommand, ServerResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateServerCommandHandler(
        IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ServerResponse> Handle(
        CreateServerCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new ForbiddenException();

        var server = Server.Create(request.Name, userId, request.Description);

        var member = ServerMember.Create(userId, server.Id);

        var defaultRole = ServerRole.CreateDefault(server.Id);

        var generalChannel = Channel.Create(
            "general", Domain.Enums.ChannelType.Text, server.Id, position: 0);

        _db.Servers.Add(server);
        _db.ServerMembers.Add(member);
        _db.ServerRoles.Add(defaultRole);
        _db.Channels.Add(generalChannel);

        await _db.SaveChangesAsync(ct);

        return new ServerResponse(
            server.Id,
            server.Name,
            server.Description,
            server.IconUrl,
            server.InviteCode,
            server.OwnerId,
            MemberCount: 1,
            server.CreatedAt);
    }
}