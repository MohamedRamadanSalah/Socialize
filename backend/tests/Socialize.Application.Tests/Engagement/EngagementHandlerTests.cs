using FluentAssertions;
using Socialize.Application.Common.Exceptions;
using Socialize.Application.Engagement.Commands.AddComment;
using Socialize.Application.Engagement.Commands.DeleteComment;
using Socialize.Application.Engagement.Commands.LikePost;
using Socialize.Application.Engagement.Commands.UnlikePost;
using Socialize.Application.Tests.TestSupport;
using Socialize.Domain.Entities;
using Xunit;

namespace Socialize.Application.Tests.Engagement;

public class EngagementHandlerTests
{
    private static (Infrastructure.Persistence.AppDbContext Db, User Alice, User Bob, Post Post) SeedPost()
    {
        var db = TestDbContextFactory.Create();
        var alice = new User { Id = Guid.NewGuid(), UserName = "alice", Email = "alice@example.com", PasswordHash = "x", DisplayName = "Alice", CreatedAt = DateTimeOffset.UtcNow };
        var bob = new User { Id = Guid.NewGuid(), UserName = "bob", Email = "bob@example.com", PasswordHash = "x", DisplayName = "Bob", CreatedAt = DateTimeOffset.UtcNow };
        var post = new Post { Id = Guid.NewGuid(), AuthorId = alice.Id, Content = "Alice's post", CreatedAt = DateTimeOffset.UtcNow };
        db.Users.AddRange(alice, bob);
        db.Posts.Add(post);
        db.SaveChanges();
        return (db, alice, bob, post);
    }

    [Fact]
    public async Task LikePost_IsIdempotent_AndUnlikeRemoves()
    {
        var (db, _, bob, post) = SeedPost();
        var likeHandler = new LikePostCommandHandler(db, new FakeNotificationPublisher());

        await likeHandler.Handle(new LikePostCommand(bob.Id, post.Id), default);
        await likeHandler.Handle(new LikePostCommand(bob.Id, post.Id), default);
        db.Likes.Count().Should().Be(1);

        var unlikeHandler = new UnlikePostCommandHandler(db);
        await unlikeHandler.Handle(new UnlikePostCommand(bob.Id, post.Id), default);
        db.Likes.Count().Should().Be(0);
    }

    [Fact]
    public async Task AddComment_PersistsComment()
    {
        var (db, _, bob, post) = SeedPost();
        var handler = new AddCommentCommandHandler(db, new FakeNotificationPublisher());

        var result = await handler.Handle(new AddCommentCommand(post.Id, bob.Id, "Nice post!"), default);

        result.Content.Should().Be("Nice post!");
        db.Comments.Count().Should().Be(1);
    }

    [Fact]
    public async Task DeleteComment_ByNonAuthor_ThrowsForbidden_ByAuthor_Succeeds()
    {
        var (db, _, bob, post) = SeedPost();
        var addHandler = new AddCommentCommandHandler(db, new FakeNotificationPublisher());
        var comment = await addHandler.Handle(new AddCommentCommand(post.Id, bob.Id, "Comment"), default);

        var deleteHandler = new DeleteCommentCommandHandler(db);
        var otherUserId = Guid.NewGuid();
        var act = () => deleteHandler.Handle(new DeleteCommentCommand(comment.Id, otherUserId), default);
        await act.Should().ThrowAsync<ForbiddenException>();

        await deleteHandler.Handle(new DeleteCommentCommand(comment.Id, bob.Id), default);
        db.Comments.Count().Should().Be(0);
    }
}
