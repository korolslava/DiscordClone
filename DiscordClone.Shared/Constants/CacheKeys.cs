namespace DiscordClone.Shared.Constants;

public static class CacheKeys
{
    public static string UserPresence(Guid userId) => $"presence:user:{userId}";
    public static string UserConnections(Guid userId) => $"connections:user:{userId}";
    public static string UserById(Guid userId) => $"user:{userId}";
    public static string ServerMembers(Guid serverId) => $"server:members:{serverId}";
}