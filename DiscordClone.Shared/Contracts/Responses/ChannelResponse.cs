namespace DiscordClone.Shared.Contracts.Responses;

public record ChannelResponse(
    Guid Id,
    string Name,
    string? Topic,
    string Type,
    int Position,
    bool IsPrivate,
    Guid ServerId
);