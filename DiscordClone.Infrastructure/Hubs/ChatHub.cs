using System.Security.Claims;
using DiscordClone.Application.Common.Interfaces;
using DiscordClone.Shared.Contracts.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace DiscordClone.Infrastructure.Hubs;

[Authorize]
public sealed class ChatHub : Hub
{
    private readonly IPresenceService _presence;
    private readonly IApplicationDbContext _db;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(IPresenceService presence,
        IApplicationDbContext db, ILogger<ChatHub> logger)
    {
        _presence = presence;
        _db = db;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return;

        await _presence.SetUserOnlineAsync(userId, Context.ConnectionId);

        var serverIds = _db.ServerMembers
            .Where(sm => sm.UserId == userId)
            .Select(sm => sm.ServerId)
            .ToList();

        foreach (var serverId in serverIds)
            await Groups.AddToGroupAsync(Context.ConnectionId, $"server:{serverId}");

        await Clients.Others.SendAsync(RealtimeEvents.UserOnline,
            new { UserId = userId });

        _logger.LogInformation("User {UserId} connected [{ConnectionId}]",
            userId, Context.ConnectionId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return;

        await _presence.SetUserOfflineAsync(userId, Context.ConnectionId);

        var stillOnline = await _presence.IsUserOnlineAsync(userId);
        if (!stillOnline)
            await Clients.Others.SendAsync(RealtimeEvents.UserOffline,
                new { UserId = userId });

        _logger.LogInformation("User {UserId} disconnected", userId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinChannel(Guid channelId) =>
        await Groups.AddToGroupAsync(Context.ConnectionId, $"channel:{channelId}");

    public async Task LeaveChannel(Guid channelId) =>
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"channel:{channelId}");

    public async Task SendTyping(Guid channelId) =>
        await Clients.OthersInGroup($"channel:{channelId}")
            .SendAsync(RealtimeEvents.UserTyping,
                new { UserId = GetUserId(), ChannelId = channelId });

    private Guid GetUserId()
    {
        var value = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                 ?? Context.User?.FindFirstValue(JwtRegisteredClaimNames.Sub);

        return Guid.TryParse(value, out var id) ? id : Guid.Empty;
    }
}