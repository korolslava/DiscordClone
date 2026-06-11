namespace DiscordClone.Shared.Contracts.Requests;

public record SendMessageRequest(
    string Content,
    Guid? ReplyToMessageId = null
);

public record EditMessageRequest(string Content);