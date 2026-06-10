namespace DiscordClone.Domain.Entities;

public class ServerMember : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid ServerId { get; private set; }
    public string? Nickname { get; private set; }
    public DateTime JoinedAt { get; private set; } = DateTime.UtcNow;

    public User User { get; private set; } = null!;
    public Server Server { get; private set; } = null!;
    public ICollection<ServerRole> Roles { get; private set; } = [];

    private ServerMember() { }

    public static ServerMember Create(Guid userId, Guid serverId) => new()
    {
        UserId = userId,
        ServerId = serverId
    };

    public void SetNickname(string? nickname)
    {
        Nickname = nickname?.Trim();
        SetUpdatedAt();
    }
}