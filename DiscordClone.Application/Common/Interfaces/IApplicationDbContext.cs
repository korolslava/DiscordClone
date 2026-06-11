using DiscordClone.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Server> Servers { get; }
    DbSet<Channel> Channels { get; }
    DbSet<Message> Messages { get; }
    DbSet<ServerMember> ServerMembers { get; }
    DbSet<ServerRole> ServerRoles { get; }
    DbSet<FriendRequest> FriendRequests { get; }
    DbSet<DirectMessage> DirectMessages { get; }
    DbSet<DirectMessageParticipant> DirectMessageParticipants { get; }
    DbSet<MessageAttachment> MessageAttachments { get; }
    DbSet<MessageReaction> MessageReactions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}