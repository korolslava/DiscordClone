namespace DiscordClone.Shared.Contracts.Responses;

public record ServerDetailResponse(
    Guid Id,
    string Name,
    string? Description,
    string? IconUrl,
    string InviteCode,
    Guid OwnerId,
    IEnumerable<ChannelResponse> Channels,
    IEnumerable<ServerMemberResponse> Members,
    DateTime CreatedAt
);

public record ServerMemberResponse(
    Guid UserId,
    string Username,
    string DisplayName,
    string? AvatarUrl,
    string? Nickname,
    string Status,
    DateTime JoinedAt
);