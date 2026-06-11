using DiscordClone.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscordClone.Infrastructure.Persistence.Configurations;

public class ServerMemberConfiguration : IEntityTypeConfiguration<ServerMember>
{
    public void Configure(EntityTypeBuilder<ServerMember> builder)
    {
        builder.HasKey(sm => sm.Id);

        builder.HasIndex(sm => new { sm.UserId, sm.ServerId }).IsUnique();

        builder.Property(sm => sm.Nickname)
            .HasMaxLength(32);

        builder.HasOne(sm => sm.User)
            .WithMany(u => u.ServerMemberships)
            .HasForeignKey(sm => sm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(sm => sm.Roles)
            .WithMany(r => r.Members);
    }
}