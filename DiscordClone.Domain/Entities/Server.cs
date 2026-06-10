using System.Threading.Channels;

namespace DiscordClone.Domain.Entities;

public class Server : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? IconUrl { get; private set; }
    public string InviteCode { get; private set; } = string.Empty;
    public Guid OwnerId { get; private set; }

    public User Owner { get; private set; } = null!;
    public ICollection<ServerMember> Members { get; private set; } = [];
    public ICollection<Channel> Channels { get; private set; } = [];
    public ICollection<ServerRole> Roles { get; private set; } = [];

    private Server() { }

    public static Server Create(string name, Guid ownerId, string? description = null) => new()
    {
        Name = name.Trim(),
        Description = description?.Trim(),
        OwnerId = ownerId,
        InviteCode = GenerateInviteCode()
    };

    public void Update(string name, string? description)
    {
        Name = name.Trim();
        Description = description?.Trim();
        SetUpdatedAt();
    }

    public void SetIcon(string iconUrl)
    {
        IconUrl = iconUrl;
        SetUpdatedAt();
    }

    public void RegenerateInviteCode()
    {
        InviteCode = GenerateInviteCode();
        SetUpdatedAt();
    }

    private static string GenerateInviteCode() =>
        Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("/", "_")
            .Replace("+", "-")[..8];
}