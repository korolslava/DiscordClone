namespace DiscordClone.Shared.Contracts.Requests;

public record CreateChannelRequest(
    string Name,
    string Type,
    string? Topic = null,
    bool IsPrivate = false
);

public record UpdateChannelRequest(string Name, string? Topic);