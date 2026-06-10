namespace DiscordClone.Domain.Enums;

[Flags]
public enum ServerPermission : long
{
    None = 0,
    ReadMessages = 1 << 0,
    SendMessages = 1 << 1,
    ManageMessages = 1 << 2,
    ManageChannels = 1 << 3,
    ManageRoles = 1 << 4,
    ManageServer = 1 << 5,
    KickMembers = 1 << 6,
    BanMembers = 1 << 7,
    AttachFiles = 1 << 8,
    Administrator = 1 << 9
}