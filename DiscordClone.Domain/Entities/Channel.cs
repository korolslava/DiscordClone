using DiscordClone.Domain.Enums;

namespace DiscordClone.Domain.Entities;

public class Channel : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Topic { get; private set; }
    public ChannelType Type { get; private set; }
    public int Position { get; private set; }
    public bool IsPrivate { get; private set; }
    public Guid ServerId { get; private set; }

    public Server Server { get; private set; } = null!;
    public ICollection<Message> Messages { get; private set; } = [];

    private Channel() { }

    public static Channel Create(string name, ChannelType type, Guid serverId,
        int position = 0, string? topic = null, bool isPrivate = false) => new()
        {
            Name = name.ToLower().Trim().Replace(" ", "-"),
            Type = type,
            ServerId = serverId,
            Position = position,
            Topic = topic?.Trim(),
            IsPrivate = isPrivate
        };

    public void Update(string name, string? topic)
    {
        Name = name.ToLower().Trim().Replace(" ", "-");
        Topic = topic?.Trim();
        SetUpdatedAt();
    }
}