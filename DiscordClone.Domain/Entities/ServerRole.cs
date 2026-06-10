using DiscordClone.Domain.Enums;

namespace DiscordClone.Domain.Entities;

public class ServerRole : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Color { get; private set; } = "#99AAB5";
    public int Position { get; private set; }
    public bool IsDefault { get; private set; }
    public long Permissions { get; private set; }
    public Guid ServerId { get; private set; }

    public Server Server { get; private set; } = null!;
    public ICollection<ServerMember> Members { get; private set; } = [];

    private ServerRole() { }

    public static ServerRole CreateDefault(Guid serverId) => new()
    {
        Name = "@everyone",
        ServerId = serverId,
        IsDefault = true,
        Position = 0,
        Permissions = (long)(ServerPermission.ReadMessages | ServerPermission.SendMessages)
    };

    public static ServerRole Create(string name, Guid serverId,
        int position, string color = "#99AAB5") => new()
        {
            Name = name.Trim(),
            ServerId = serverId,
            Position = position,
            Color = color
        };

    public void GrantPermission(ServerPermission permission)
    {
        Permissions |= (long)permission;
        SetUpdatedAt();
    }

    public void RevokePermission(ServerPermission permission)
    {
        Permissions &= ~(long)permission;
        SetUpdatedAt();
    }

    public bool HasPermission(ServerPermission permission) =>
        (Permissions & (long)permission) == (long)permission;
}