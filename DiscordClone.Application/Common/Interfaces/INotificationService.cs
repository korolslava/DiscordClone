namespace DiscordClone.Application.Common.Interfaces;

public interface INotificationService
{
    Task SendToUserAsync(Guid userId, string eventName, object payload);
    Task SendToChannelGroupAsync(Guid channelId, string eventName, object payload);
    Task SendToServerGroupAsync(Guid serverId, string eventName, object payload);
}