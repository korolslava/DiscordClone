namespace DiscordClone.Shared.Contracts.Responses;

public record FriendResponse(
    Guid Id,
    string Username,
    string DisplayName,
    string? AvatarUrl,
    string Status,
    Guid? DirectMessageId
);

public record FriendRequestResponse(
    Guid Id,
    UserResponse Sender,
    UserResponse Receiver,
    string Status,
    DateTime CreatedAt
);