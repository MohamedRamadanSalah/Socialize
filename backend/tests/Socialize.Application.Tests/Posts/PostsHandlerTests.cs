using FluentAssertions;
using Socialize.Application.Common.Exceptions;
using Socialize.Application.Posts.Commands.CreatePost;
using Socialize.Application.Posts.Commands.DeletePost;
using Socialize.Application.Posts.Commands.EditPostText;
using Socialize.Application.Posts.Queries.GetFeed;
using Socialize.Application.Tests.TestSupport;
using Socialize.Domain.Entities;
using Xunit;

namespace Socialize.Application.Tests.Posts;

public class PostsHandlerTests
{
    private static (Infrastructure.Persistence.AppDbContext Db, User Alice, User Bob, FakeFileStorage Storage) SeedTwoUsers()
    {
        var db = TestDbContextFactory.Create();
        var alice = new User { Id = Guid.NewGuid(), UserName = "alice", Email = "alice@example.com", PasswordHash = "x", DisplayName = "Alice", CreatedAt = DateTimeOffset.UtcNow };
        var bob = new User { Id = Guid.NewGuid(), UserName = "bob", Email = "bob@example.com", PasswordHash = "x", DisplayName = "Bob", CreatedAt = DateTimeOffset.UtcNow };
        db.Users.AddRange(alice, bob);
        db.SaveChanges();
        return (db, alice, bob, new FakeFileStorage());
    }

    [Fact]
    public async Task CreatePost_WithValidImage_Succeeds()
    {
        var (db, alice, _, storage) = SeedTwoUsers();
        var handler = new CreatePostCommandHandler(db, storage);

        var images = new List<PostImageUpload> { new(TestImages.ValidPng(), "photo.png") };
        var result = await handler.Handle(new CreatePostCommand(alice.Id, "Hello world", images), default);

        result.Content.Should().Be("Hello world");
        result.ImageUrls.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreatePost_WithMoreThanFourImages_ThrowsValidation()
    {
        var (db, alice, _, storage) = SeedTwoUsers();
        var handler = new CreatePostCommandHandler(db, storage);

        var images = Enumerable.Range(0, 5).Select(_ => new PostImageUpload(TestImages.ValidPng(), "photo.png")).ToList();
        var act = () => handler.Handle(new CreatePostCommand(alice.Id, "Too many images", images), default);

        await act.Should().ThrowAsync<ValidationAppException>();
    }

    [Fact]
    public async Task CreatePost_WithSpoofedContentType_ThrowsValidation()
    {
        var (db, alice, _, storage) = SeedTwoUsers();
        var handler = new CreatePostCommandHandler(db, storage);

        var images = new List<PostImageUpload> { new(TestImages.Invalid(), "photo.png") };
        var act = () => handler.Handle(new CreatePostCommand(alice.Id, "Bad image", images), default);

        await act.Should().ThrowAsync<ValidationAppException>();
    }

    [Fact]
    public async Task EditPostText_ByOwner_UpdatesContentAndSetsEditedAt()
    {
        var (db, alice, _, storage) = SeedTwoUsers();
        var createHandler = new CreatePostCommandHandler(db, storage);
        var post = await createHandler.Handle(new CreatePostCommand(alice.Id, "Original", new List<PostImageUpload>()), default);

        var editHandler = new EditPostTextCommandHandler(db);
        var edited = await editHandler.Handle(new EditPostTextCommand(post.Id, alice.Id, "Edited text"), default);

        edited.Content.Should().Be("Edited text");
        edited.EditedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task EditPostText_ByNonOwner_ThrowsForbidden()
    {
        var (db, alice, bob, storage) = SeedTwoUsers();
        var createHandler = new CreatePostCommandHandler(db, storage);
        var post = await createHandler.Handle(new CreatePostCommand(alice.Id, "Original", new List<PostImageUpload>()), default);

        var editHandler = new EditPostTextCommandHandler(db);
        var act = () => editHandler.Handle(new EditPostTextCommand(post.Id, bob.Id, "Hacked"), default);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task DeletePost_ByNonOwner_ThrowsForbidden_ByOwner_Succeeds()
    {
        var (db, alice, bob, storage) = SeedTwoUsers();
        var createHandler = new CreatePostCommandHandler(db, storage);
        var post = await createHandler.Handle(new CreatePostCommand(alice.Id, "Original", new List<PostImageUpload>()), default);

        var deleteHandler = new DeletePostCommandHandler(db, storage);
        var act = () => deleteHandler.Handle(new DeletePostCommand(post.Id, bob.Id), default);
        await act.Should().ThrowAsync<ForbiddenException>();

        await deleteHandler.Handle(new DeletePostCommand(post.Id, alice.Id), default);
        db.Posts.Count().Should().Be(0);
    }

    [Fact]
    public async Task GetFeed_OnlyShowsPostsFromFollowedAuthors()
    {
        var (db, alice, bob, storage) = SeedTwoUsers();
        var createHandler = new CreatePostCommandHandler(db, storage);
        await createHandler.Handle(new CreatePostCommand(bob.Id, "Bob's post", new List<PostImageUpload>()), default);

        var feedHandlerBeforeFollow = new GetFeedQueryHandler(db);
        var feedBefore = await feedHandlerBeforeFollow.Handle(new GetFeedQuery(alice.Id, null, null), default);
        feedBefore.Items.Should().BeEmpty();

        db.Follows.Add(new Follow { FollowerId = alice.Id, FolloweeId = bob.Id, CreatedAt = DateTimeOffset.UtcNow });
        db.SaveChanges();

        var feedHandlerAfterFollow = new GetFeedQueryHandler(db);
        var feedAfter = await feedHandlerAfterFollow.Handle(new GetFeedQuery(alice.Id, null, null), default);
        feedAfter.Items.Should().ContainSingle(p => p.Content == "Bob's post");
    }
}
