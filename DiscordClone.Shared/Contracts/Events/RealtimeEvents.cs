namespace DiscordClone.Shared.Contracts.Events;

public static class RealtimeEvents
{
    // Messages
    public const string MessageReceived = "MessageReceived";
    public const string MessageEdited = "MessageEdited";
    public const string MessageDeleted = "MessageDeleted";
    public const string ReactionAdded = "ReactionAdded";
    public const string ReactionRemoved = "ReactionRemoved";

    // Presence
    public const string UserOnline = "UserOnline";
    public const string UserOffline = "UserOffline";
    public const string UserStatusChanged = "UserStatusChanged";
    public const string UserTyping = "UserTyping";

    // Server
    public const string MemberJoined = "MemberJoined";
    public const string MemberLeft = "MemberLeft";
    public const string MemberKicked = "MemberKicked";

    // Friends
    public const string FriendRequestReceived = "FriendRequestReceived";
    public const string FriendRequestAccepted = "FriendRequestAccepted";
}