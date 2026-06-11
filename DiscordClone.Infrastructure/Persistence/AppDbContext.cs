using DiscordClone.Application.Common.Interfaces;
using DiscordClone.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiscordClone.Infrastructure.Persistence;

public class AppDbContext : DbContext, IApplicationDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Server> Servers => Set<Server>();
    public DbSet<Channel> Channels => Set<Channel>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<ServerMember> ServerMembers => Set<ServerMember>();
    public DbSet<ServerRole> ServerRoles => Set<ServerRole>();
    public DbSet<FriendRequest> FriendRequests => Set<FriendRequest>();
    public DbSet<DirectMessage> DirectMessages => Set<DirectMessage>();
    public DbSet<DirectMessageParticipant> DirectMessageParticipants => Set<DirectMessageParticipant>();
    public DbSet<MessageAttachment> MessageAttachments => Set<MessageAttachment>();
    public DbSet<MessageReaction> MessageReactions => Set<MessageReaction>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(builder);
    }
}