namespace DiscordClone.Domain.Entities;

public class DirectMessage : BaseEntity
{
    public ICollection<DirectMessageParticipant> Participants { get; private set; } = [];
    public ICollection<Message> Messages { get; private set; } = [];

    private DirectMessage() { }

    public static DirectMessage Create() => new();
}