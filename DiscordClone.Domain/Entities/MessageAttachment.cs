namespace DiscordClone.Domain.Entities;

public class MessageAttachment : BaseEntity
{
    public string FileName { get; private set; } = string.Empty;
    public string FileUrl { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long FileSize { get; private set; }
    public Guid MessageId { get; private set; }

    public Message Message { get; private set; } = null!;

    private MessageAttachment() { }

    public static MessageAttachment Create(string fileName, string fileUrl,
        string contentType, long fileSize, Guid messageId) => new()
        {
            FileName = fileName,
            FileUrl = fileUrl,
            ContentType = contentType,
            FileSize = fileSize,
            MessageId = messageId
        };
}