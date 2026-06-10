using DiscordClone.Domain.Enums;

namespace DiscordClone.Domain.Entities;

public class FriendRequest : BaseEntity
{
    public Guid SenderId { get; private set; }
    public Guid ReceiverId { get; private set; }
    public FriendRequestStatus Status { get; private set; } = FriendRequestStatus.Pending;

    public User Sender { get; private set; } = null!;
    public User Receiver { get; private set; } = null!;

    private FriendRequest() { }

    public static FriendRequest Create(Guid senderId, Guid receiverId) => new()
    {
        SenderId = senderId,
        ReceiverId = receiverId
    };

    public void Accept()
    {
        Status = FriendRequestStatus.Accepted;
        SetUpdatedAt();
    }

    public void Decline()
    {
        Status = FriendRequestStatus.Declined;
        SetUpdatedAt();
    }
}