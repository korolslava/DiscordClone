namespace DiscordClone.Shared.Contracts.Responses;

public record UserResponse(
    Guid Id,
    string Username,
    string DisplayName,
    string? AvatarUrl,
    string? About,
    string Status
);