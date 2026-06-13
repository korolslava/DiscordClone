using DiscordClone.Domain.Entities;
using DiscordClone.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DiscordClone.Infrastructure.Persistence.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        AppDbContext db, ILogger logger, CancellationToken ct = default)
    {
        if (await db.Users.AnyAsync(ct))
        {
            logger.LogInformation("Database already seeded. Skipping.");
            return;
        }

        logger.LogInformation("Seeding database...");

        var adminHash = BCrypt.Net.BCrypt.HashPassword("Admin123!");
        var userHash = BCrypt.Net.BCrypt.HashPassword("User1234!");

        var admin = User.Create("admin", "Admin", "admin@discordclone.dev", adminHash);
        var alice = User.Create("alice", "Alice", "alice@discordclone.dev", userHash);
        var bob = User.Create("bob", "Bob", "bob@discordclone.dev", userHash);

        db.Users.AddRange(admin, alice, bob);
        await db.SaveChangesAsync(ct);

        var server = Server.Create(
            "DiscordClone HQ", admin.Id, "Official dev server");

        db.Servers.Add(server);
        await db.SaveChangesAsync(ct);

        var general = Channel.Create("general", ChannelType.Text, server.Id, 0, "General chat");
        var announcements = Channel.Create("announcements", ChannelType.Announcement, server.Id, 1, "Official updates");
        var voice = Channel.Create("voice", ChannelType.Voice, server.Id, 2);

        db.Channels.AddRange(general, announcements, voice);

        var everyoneRole = ServerRole.CreateDefault(server.Id);
        var adminRole = ServerRole.Create("Admin", server.Id, 1, "#E74C3C");
        adminRole.GrantPermission(ServerPermission.Administrator);

        db.ServerRoles.AddRange(everyoneRole, adminRole);

        var adminMember = ServerMember.Create(admin.Id, server.Id);
        var aliceMember = ServerMember.Create(alice.Id, server.Id);
        var bobMember = ServerMember.Create(bob.Id, server.Id);

        db.ServerMembers.AddRange(adminMember, aliceMember, bobMember);
        await db.SaveChangesAsync(ct);

        var welcomeMsg = Message.CreateChannelMessage(
            "Welcome to DiscordClone HQ! 🎉",
            admin.Id,
            general.Id);

        db.Messages.Add(welcomeMsg);
        await db.SaveChangesAsync(ct);

        logger.LogInformation(
            "Database seeded: 3 users, 1 server, 3 channels, 1 message.");
    }
}