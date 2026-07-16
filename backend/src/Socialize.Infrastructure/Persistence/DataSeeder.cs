using Microsoft.EntityFrameworkCore;
using Socialize.Application.Abstractions;
using Socialize.Domain.Entities;
using Socialize.Domain.Enums;

namespace Socialize.Infrastructure.Persistence;

/// <summary>
/// Idempotent demo-data seeder: only runs when the Users table is empty (FR-027, SC-007).
/// Demo password for every seeded user is "P@ssw0rd123" (documented in API.md). Posts are seeded
/// as text-only (no images) since there are no real image files to attach at seed time; the
/// image-upload path itself is exercised live via the API rather than by the seed.
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

        var postContents = new (User Author, string Content, int DaysAgo)[]
        {
            (alice, "Kicking off my Flutter training project against this backend today!", 9),
            (alice, "Cursor pagination finally clicked for me. Feels great once it works.", 6),
            (bob, "Just wired up the token-refresh interceptor. JWT rotation is a fun lesson.", 8),
            (bob, "SignalR notifications arrived in under a second. Loving this training backend.", 4),
            (carol, "Full-text search over Postgres tsvector columns is surprisingly approachable.", 7),
            (dave, "Multipart image upload validation caught my oversized test file immediately.", 5),
            (erin, "Studying the CQRS handler structure in Socialize.Application today.", 3),
            (erin, "Edited this post to fix a typo — the editedAt flag makes that visible.", 1)
        };

        var posts = new List<Post>();
        foreach (var (author, content, daysAgo) in postContents)
        {
            posts.Add(new Post
            {
                Id = Guid.NewGuid(),
                AuthorId = author.Id,
                Content = content,
                CreatedAt = now.AddDays(-daysAgo)
            });
        }
        db.Posts.AddRange(posts);

        var likes = new List<Like>
        {
            new() { UserId = bob.Id, PostId = posts[0].Id, CreatedAt = now.AddDays(-8) },
            new() { UserId = carol.Id, PostId = posts[0].Id, CreatedAt = now.AddDays(-8) },
            new() { UserId = alice.Id, PostId = posts[2].Id, CreatedAt = now.AddDays(-7) },
            new() { UserId = dave.Id, PostId = posts[3].Id, CreatedAt = now.AddDays(-3) },
            new() { UserId = alice.Id, PostId = posts[4].Id, CreatedAt = now.AddDays(-6) },
            new() { UserId = erin.Id, PostId = posts[6].Id, CreatedAt = now.AddDays(-2) }
        };
        db.Likes.AddRange(likes);

        var comments = new List<Comment>
        {
            new() { Id = Guid.NewGuid(), PostId = posts[0].Id, AuthorId = bob.Id, Content = "Welcome aboard!", CreatedAt = now.AddDays(-8) },
            new() { Id = Guid.NewGuid(), PostId = posts[2].Id, AuthorId = alice.Id, Content = "Rotation on refresh is the best part.", CreatedAt = now.AddDays(-7) },
            new() { Id = Guid.NewGuid(), PostId = posts[4].Id, AuthorId = dave.Id, Content = "GIN indexes make it fast too.", CreatedAt = now.AddDays(-6) }
        };
        db.Comments.AddRange(comments);

        var notifications = new List<Notification>
        {
            new() { Id = Guid.NewGuid(), RecipientId = alice.Id, ActorId = bob.Id, Type = NotificationType.Like, EntityId = posts[0].Id, IsRead = false, CreatedAt = now.AddDays(-8) },
            new() { Id = Guid.NewGuid(), RecipientId = alice.Id, ActorId = carol.Id, Type = NotificationType.Like, EntityId = posts[0].Id, IsRead = false, CreatedAt = now.AddDays(-8) },
            new() { Id = Guid.NewGuid(), RecipientId = bob.Id, ActorId = alice.Id, Type = NotificationType.Comment, EntityId = posts[2].Id, IsRead = true, CreatedAt = now.AddDays(-7) },
            new() { Id = Guid.NewGuid(), RecipientId = alice.Id, ActorId = erin.Id, Type = NotificationType.Follow, EntityId = null, IsRead = false, CreatedAt = now.AddDays(-3) }
        };
        db.Notifications.AddRange(notifications);

        await db.SaveChangesAsync();
    }
}
