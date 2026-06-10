namespace DiscordClone.Domain.Entities;

public class MessageReaction : BaseEntity
{
    public string Emoji { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }
    public Guid MessageId { get; private set; }

    public User User { get; private set; } = null!;
    public Message Message { get; private set; } = null!;

    private MessageReaction() { }

    public static MessageReaction Create(string emoji, Guid userId, Guid messageId) => new()
    {
        Emoji = emoji,
        UserId = userId,
        MessageId = messageId
    };
}