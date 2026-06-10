namespace DiscordClone.Domain.Entities;

public class Message : BaseEntity
{
    public string Content { get; private set; } = string.Empty;
    public bool IsEdited { get; private set; }
    public bool IsDeleted { get; private set; }
    public Guid AuthorId { get; private set; }
    public Guid? ChannelId { get; private set; }
    public Guid? DirectMessageId { get; private set; }
    public Guid? ReplyToMessageId { get; private set; }

    public User Author { get; private set; } = null!;
    public Channel? Channel { get; private set; }
    public DirectMessage? DirectMessage { get; private set; }
    public Message? ReplyToMessage { get; private set; }
    public ICollection<MessageAttachment> Attachments { get; private set; } = [];
    public ICollection<MessageReaction> Reactions { get; private set; } = [];

    private Message() { }

    public static Message CreateChannelMessage(string content,
        Guid authorId, Guid channelId, Guid? replyToId = null) => new()
        {
            Content = content.Trim(),
            AuthorId = authorId,
            ChannelId = channelId,
            ReplyToMessageId = replyToId
        };

    public static Message CreateDirectMessage(string content,
        Guid authorId, Guid directMessageId) => new()
        {
            Content = content.Trim(),
            AuthorId = authorId,
            DirectMessageId = directMessageId
        };

    public void Edit(string newContent)
    {
        Content = newContent.Trim();
        IsEdited = true;
        SetUpdatedAt();
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        Content = "This message was deleted.";
        SetUpdatedAt();
    }
}