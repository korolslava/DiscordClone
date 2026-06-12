using DiscordClone.Application.Common.Interfaces;
using DiscordClone.Domain.Exceptions;
using DiscordClone.Shared.Contracts.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Application.Features.Servers.Commands;

public sealed class UpdateServerCommandHandler
    : IRequestHandler<UpdateServerCommand, ServerResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateServerCommandHandler(
        IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ServerResponse> Handle(
        UpdateServerCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new ForbiddenException();

        var server = await _db.Servers
            .FirstOrDefaultAsync(s => s.Id == request.ServerId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Server), request.ServerId);

        if (server.OwnerId != userId)
            throw new ForbiddenException("Only the server owner can update server settings.");

        server.Update(request.Name, request.Description);
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