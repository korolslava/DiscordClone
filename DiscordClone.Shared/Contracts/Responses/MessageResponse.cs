namespace DiscordClone.Shared.Contracts.Responses;

public record MessageResponse(
    Guid Id,
    string Content,
    bool IsEdited,
    bool IsDeleted,
    UserResponse Author,
    Guid? ChannelId,
    Guid? DirectMessageId,
    Guid? ReplyToMessageId,
    IEnumerable<AttachmentResponse> Attachments,
    IEnumerable<ReactionResponse> Reactions,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record AttachmentResponse(
    Guid Id,
    string FileName,
    string FileUrl,
    string ContentType,
    long FileSize
);

public record ReactionResponse(
    string Emoji,
    int Count,
    bool IsReactedByCurrentUser
);