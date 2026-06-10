namespace DiscordClone.Domain.Entities;

public class DirectMessageParticipant : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid DirectMessageId { get; private set; }

    public User User { get; private set; } = null!;
    public DirectMessage DirectMessage { get; private set; } = null!;

    private DirectMessageParticipant() { }

    public static DirectMessageParticipant Create(Guid userId, Guid dmId) => new()
    {
        UserId = userId,
        DirectMessageId = dmId
    };
}