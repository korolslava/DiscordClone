using DiscordClone.Application.Common.Interfaces;
using DiscordClone.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Application.Features.Servers.Commands;

public sealed class DeleteServerCommandHandler
    : IRequestHandler<DeleteServerCommand>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeleteServerCommandHandler(
        IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteServerCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new ForbiddenException();

        var server = await _db.Servers
            .FirstOrDefaultAsync(s => s.Id == request.ServerId, ct)
            ?? throw new NotFoundException(
                nameof(Domain.Entities.Server), request.ServerId);

        if (server.OwnerId != userId)
            throw new ForbiddenException("Only the server owner can delete the server.");

        _db.Servers.Remove(server);
        await _db.SaveChangesAsync(ct);
    }
}