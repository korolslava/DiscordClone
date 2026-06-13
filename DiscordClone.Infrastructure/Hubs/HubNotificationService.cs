using DiscordClone.Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace DiscordClone.Infrastructure.Hubs;

public sealed class HubNotificationService : INotificationService
{
    private readonly IHubContext<ChatHub> _hub;
    private readonly IPresenceService _presence;

    public HubNotificationService(IHubContext<ChatHub> hub,
        IPresenceService presence)
    {
        _hub = hub;
        _presence = presence;
    }

    public async Task SendToUserAsync(Guid userId, string eventName, object payload)
    {
        var connections = await _presence.GetUserConnectionsAsync(userId);
        foreach (var connectionId in connections)
            await _hub.Clients.Client(connectionId).SendAsync(eventName, payload);
    }

    public async Task SendToChannelGroupAsync(Guid channelId,
        string eventName, object payload) =>
        await _hub.Clients.Group($"channel:{channelId}").SendAsync(eventName, payload);

    public async Task SendToServerGroupAsync(Guid serverId,
        string eventName, object payload) =>
        await _hub.Clients.Group($"server:{serverId}").SendAsync(eventName, payload);
}