using DiscordClone.Domain.Entities;
using DiscordClone.Domain.Enums;

namespace DiscordClone.Application.Common.Interfaces;

public interface IPresenceService
{
    Task SetUserOnlineAsync(Guid userId, string connectionId);
    Task SetUserOfflineAsync(Guid userId, string connectionId);
    Task<bool> IsUserOnlineAsync(Guid userId);
    Task<IEnumerable<string>> GetUserConnectionsAsync(Guid userId);
    Task UpdateUserStatusAsync(Guid userId, UserStatus status);
}