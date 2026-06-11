namespace DiscordClone.Shared.Contracts.Responses;

public record ServerResponse(
    Guid Id,
    string Name,
    string? Description,
    string? IconUrl,
    string InviteCode,
    Guid OwnerId,
    int MemberCount,
    DateTime CreatedAt
);