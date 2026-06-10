using DiscordClone.Domain.Enums;

namespace DiscordClone.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string? AvatarUrl { get; private set; }
    public string? About { get; private set; }
    public UserStatus Status { get; private set; } = UserStatus.Offline;
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiresAt { get; private set; }

    public ICollection<ServerMember> ServerMemberships { get; private set; } = [];
    public ICollection<Message> Messages { get; private set; } = [];
    public ICollection<FriendRequest> SentFriendRequests { get; private set; } = [];
    public ICollection<FriendRequest> ReceivedFriendRequests { get; private set; } = [];

    private User() { }

    public static User Create(string username, string displayName,
        string email, string passwordHash) => new()
        {
            Username = username.ToLower().Trim(),
            DisplayName = displayName.Trim(),
            Email = email.ToLower().Trim(),
            PasswordHash = passwordHash
        };

    public void UpdateProfile(string displayName, string? about)
    {
        DisplayName = displayName.Trim();
        About = about?.Trim();
        SetUpdatedAt();
    }

    public void SetAvatar(string avatarUrl)
    {
        AvatarUrl = avatarUrl;
        SetUpdatedAt();
    }

    public void SetStatus(UserStatus status)
    {
        Status = status;
        SetUpdatedAt();
    }

    public void SetRefreshToken(string token, DateTime expiresAt)
    {
        RefreshToken = token;
        RefreshTokenExpiresAt = expiresAt;
        SetUpdatedAt();
    }

    public void RevokeRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiresAt = null;
        SetUpdatedAt();
    }

    public bool IsRefreshTokenValid(string token) =>
        RefreshToken == token && RefreshTokenExpiresAt > DateTime.UtcNow;
}