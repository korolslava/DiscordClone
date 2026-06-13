using DiscordClone.Application.Common.Interfaces;
using DiscordClone.Domain.Entities;
using DiscordClone.Domain.Enums;
using DiscordClone.Shared.Constants;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace DiscordClone.Infrastructure.Services;

public sealed class PresenceService : IPresenceService
{
    private readonly IDatabase _db;
    private readonly ILogger<PresenceService> _logger;

    public PresenceService(IConnectionMultiplexer redis,
        ILogger<PresenceService> logger)
    {
        _db = redis.GetDatabase();
        _logger = logger;
    }

    public async Task SetUserOnlineAsync(Guid userId, string connectionId)
    {
        var connectionsKey = CacheKeys.UserConnections(userId);
        await _db.SetAddAsync(connectionsKey, connectionId);
        await _db.KeyExpireAsync(connectionsKey, TimeSpan.FromDays(1));
        await _db.StringSetAsync(
            CacheKeys.UserPresence(userId), "Online", TimeSpan.FromDays(1));

        _logger.LogInformation("User {UserId} is online", userId);
    }

    public async Task SetUserOfflineAsync(Guid userId, string connectionId)
    {
        var connectionsKey = CacheKeys.UserConnections(userId);
        await _db.SetRemoveAsync(connectionsKey, connectionId);

        var remaining = await _db.SetLengthAsync(connectionsKey);
        if (remaining == 0)
        {
            await _db.KeyDeleteAsync(CacheKeys.UserPresence(userId));
            _logger.LogInformation("User {UserId} is offline", userId);
        }
    }

    public async Task<bool> IsUserOnlineAsync(Guid userId) =>
        await _db.KeyExistsAsync(CacheKeys.UserPresence(userId));

    public async Task<IEnumerable<string>> GetUserConnectionsAsync(Guid userId)
    {
        var members = await _db.SetMembersAsync(CacheKeys.UserConnections(userId));
        return members.Select(m => m.ToString());
    }

    public async Task UpdateUserStatusAsync(Guid userId, UserStatus status)
    {
        var key = CacheKeys.UserPresence(userId);
        if (await _db.KeyExistsAsync(key))
            await _db.StringSetAsync(key, status.ToString(), TimeSpan.FromDays(1));
    }
}