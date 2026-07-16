using Microsoft.EntityFrameworkCore;
using Socialize.Application.Abstractions;
using Socialize.Domain.Entities;

namespace Socialize.Infrastructure.Persistence;

/// <summary>
/// Idempotent demo-data seeder: only runs when the Users table is empty (FR-027, SC-007).
/// Demo password for every seeded user is "P@ssw0rd123" (documented in API.md).
/// </summary>
public static class DataSeeder
{
    private const string DemoPassword = "P@ssw0rd123";

    public static async Task SeedAsync(AppDbContext db, ITokenService tokenService)
    {
        if (await db.Users.AnyAsync())
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var passwordHash = tokenService.HashPassword(DemoPassword);

        var demoUsers = new[]
        {
            new User { Id = Guid.NewGuid(), UserName = "alice", Email = "alice@example.com", PasswordHash = passwordHash, DisplayName = "Alice Anderson", Bio = "Flutter trainee exploring the Socialize API.", CreatedAt = now.AddDays(-30) },
            new User { Id = Guid.NewGuid(), UserName = "bob", Email = "bob@example.com", PasswordHash = passwordHash, DisplayName = "Bob Baker", Bio = "Coffee, code, and cursor pagination.", CreatedAt = now.AddDays(-25) },
            new User { Id = Guid.NewGuid(), UserName = "carol", Email = "carol@example.com", PasswordHash = passwordHash, DisplayName = "Carol Chen", Bio = "Backend training demo user.", CreatedAt = now.AddDays(-20) },
            new User { Id = Guid.NewGuid(), UserName = "dave", Email = "dave@example.com", PasswordHash = passwordHash, DisplayName = "Dave Diaz", Bio = "Building things with SignalR.", CreatedAt = now.AddDays(-15) },
            new User { Id = Guid.NewGuid(), UserName = "erin", Email = "erin@example.com", PasswordHash = passwordHash, DisplayName = "Erin Evans", Bio = "Full-text search enthusiast.", CreatedAt = now.AddDays(-10) }
        };

        db.Users.AddRange(demoUsers);

        var alice = demoUsers[0];
        var bob = demoUsers[1];
        var carol = demoUsers[2];
        var dave = demoUsers[3];
        var erin = demoUsers[4];

        var follows = new[]
        {
            new Follow { FollowerId = alice.Id, FolloweeId = bob.Id, CreatedAt = now.AddDays(-9) },
            new Follow { FollowerId = alice.Id, FolloweeId = carol.Id, CreatedAt = now.AddDays(-8) },
            new Follow { FollowerId = bob.Id, FolloweeId = alice.Id, CreatedAt = now.AddDays(-7) },
            new Follow { FollowerId = bob.Id, FolloweeId = dave.Id, CreatedAt = now.AddDays(-6) },
            new Follow { FollowerId = carol.Id, FolloweeId = alice.Id, CreatedAt = now.AddDays(-5) },
            new Follow { FollowerId = dave.Id, FolloweeId = erin.Id, CreatedAt = now.AddDays(-4) },
            new Follow { FollowerId = erin.Id, FolloweeId = alice.Id, CreatedAt = now.AddDays(-3) }
        };

        db.Follows.AddRange(follows);

        await db.SaveChangesAsync();
    }
}
