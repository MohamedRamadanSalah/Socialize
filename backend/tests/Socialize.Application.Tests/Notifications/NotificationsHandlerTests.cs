using FluentAssertions;
using Socialize.Application.Engagement.Commands.AddComment;
using Socialize.Application.Engagement.Commands.LikePost;
using Socialize.Application.Notifications.Commands.MarkAllRead;
using Socialize.Application.Notifications.Commands.MarkRead;
using Socialize.Application.Tests.TestSupport;
using Socialize.Application.Users.Commands.Follow;
using Socialize.Domain.Entities;
using Xunit;

namespace Socialize.Application.Tests.Notifications;

public class NotificationsHandlerTests
{
    private static (Infrastructure.Persistence.AppDbContext Db, User Alice, User Bob, Post AlicesPost, FakeNotificationPublisher Publisher) SeedScenario()
    {
        var db = TestDbContextFactory.Create();
        var alice = new User { Id = Guid.NewGuid(), UserName = "alice", Email = "alice@example.com", PasswordHash = "x", DisplayName = "Alice", CreatedAt = DateTimeOffset.UtcNow };
        var bob = new User { Id = Guid.NewGuid(), UserName = "bob", Email = "bob@example.com", PasswordHash = "x", DisplayName = "Bob", CreatedAt = DateTimeOffset.UtcNow };
        var post = new Post { Id = Guid.NewGuid(), AuthorId = alice.Id, Content = "Alice's post", CreatedAt = DateTimeOffset.UtcNow };
        db.Users.AddRange(alice, bob);
        db.Posts.Add(post);
        db.SaveChanges();
        return (db, alice, bob, post, new FakeNotificationPublisher());
    }

    [Fact]
    public async Task LikingSomeoneElsesPost_CreatesAndPublishesNotification()
    {
        var (db, _, bob, post, publisher) = SeedScenario();
        var handler = new LikePostCommandHandler(db, publisher);

        await handler.Handle(new LikePostCommand(bob.Id, post.Id), default);

        db.Notifications.Count().Should().Be(1);
        publisher.Published.Should().ContainSingle(p => p.RecipientId == post.AuthorId && p.Notification.Type == "Like");
    }

    [Fact]
    public async Task LikingOwnPost_DoesNotCreateNotification()
    {
        var (db, alice, _, post, publisher) = SeedScenario();
        var handler = new LikePostCommandHandler(db, publisher);

        await handler.Handle(new LikePostCommand(alice.Id, post.Id), default);

        db.Notifications.Count().Should().Be(0);
        publisher.Published.Should().BeEmpty();
    }

    [Fact]
    public async Task CommentingOnSomeoneElsesPost_CreatesNotification()
    {
        var (db, _, bob, post, publisher) = SeedScenario();
        var handler = new AddCommentCommandHandler(db, publisher);

        await handler.Handle(new AddCommentCommand(post.Id, bob.Id, "Nice!"), default);

        db.Notifications.Count().Should().Be(1);
        publisher.Published.Should().ContainSingle(p => p.Notification.Type == "Comment");
    }

    [Fact]
    public async Task Following_CreatesNotificationForFollowee()
    {
        var (db, alice, bob, _, publisher) = SeedScenario();
        var handler = new FollowCommandHandler(db, publisher);

        await handler.Handle(new FollowCommand(bob.Id, alice.Id), default);

        db.Notifications.Count().Should().Be(1);
        publisher.Published.Should().ContainSingle(p => p.RecipientId == alice.Id && p.Notification.Type == "Follow");
    }

    [Fact]
    public async Task MarkRead_And_MarkAllRead_UpdateReadState()
    {
        var (db, _, bob, post, publisher) = SeedScenario();
        var likeHandler = new LikePostCommandHandler(db, publisher);
        await likeHandler.Handle(new LikePostCommand(bob.Id, post.Id), default);

        var notification = db.Notifications.Single();
        notification.IsRead.Should().BeFalse();

        var markReadHandler = new MarkNotificationReadCommandHandler(db);
        await markReadHandler.Handle(new MarkNotificationReadCommand(notification.Id, notification.RecipientId), default);
        db.Notifications.Single().IsRead.Should().BeTrue();

        // Reset and verify mark-all-read too.
        notification.IsRead = false;
        db.SaveChanges();
        var markAllHandler = new MarkAllNotificationsReadCommandHandler(db);
        await markAllHandler.Handle(new MarkAllNotificationsReadCommand(notification.RecipientId), default);
        db.Notifications.Single().IsRead.Should().BeTrue();
    }
}
