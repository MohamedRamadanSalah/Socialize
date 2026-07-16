using FluentAssertions;
using Socialize.Application.Common.Exceptions;
using Socialize.Application.Tests.TestSupport;
using Socialize.Application.Users.Commands.Follow;
using Socialize.Application.Users.Commands.Unfollow;
using Socialize.Application.Users.Commands.UpdateProfile;
using Socialize.Application.Users.Queries.GetFollowers;
using Socialize.Application.Users.Queries.GetFollowing;
using Socialize.Domain.Entities;
using Xunit;

namespace Socialize.Application.Tests.Users;

public class UsersHandlerTests
{
    private static (Infrastructure.Persistence.AppDbContext Db, User Alice, User Bob) SeedTwoUsers()
    {
        var db = TestDbContextFactory.Create();
        var alice = new User { Id = Guid.NewGuid(), UserName = "alice", Email = "alice@example.com", PasswordHash = "x", DisplayName = "Alice", CreatedAt = DateTimeOffset.UtcNow };
        var bob = new User { Id = Guid.NewGuid(), UserName = "bob", Email = "bob@example.com", PasswordHash = "x", DisplayName = "Bob", CreatedAt = DateTimeOffset.UtcNow };
        db.Users.AddRange(alice, bob);
        db.SaveChanges();
        return (db, alice, bob);
    }

    [Fact]
    public async Task UpdateProfile_UpdatesDisplayNameAndBio()
    {
        var (db, alice, _) = SeedTwoUsers();
        var handler = new UpdateProfileCommandHandler(db);

        var result = await handler.Handle(new UpdateProfileCommand(alice.Id, "Alice Updated", "New bio"), default);

        result.DisplayName.Should().Be("Alice Updated");
        result.Bio.Should().Be("New bio");
    }

    [Fact]
    public async Task Follow_SelfFollow_ThrowsValidation()
    {
        var (db, alice, _) = SeedTwoUsers();
        var handler = new FollowCommandHandler(db, new FakeNotificationPublisher());

        var act = () => handler.Handle(new FollowCommand(alice.Id, alice.Id), default);

        await act.Should().ThrowAsync<ValidationAppException>();
    }

    [Fact]
    public async Task Follow_IsIdempotent()
    {
        var (db, alice, bob) = SeedTwoUsers();
        var handler = new FollowCommandHandler(db, new FakeNotificationPublisher());

        await handler.Handle(new FollowCommand(alice.Id, bob.Id), default);
        await handler.Handle(new FollowCommand(alice.Id, bob.Id), default);

        db.Follows.Count().Should().Be(1);
    }

    [Fact]
    public async Task Follow_ThenAppearsInFollowersAndFollowing_ThenUnfollow_Removes()
    {
        var (db, alice, bob) = SeedTwoUsers();
        var followHandler = new FollowCommandHandler(db, new FakeNotificationPublisher());
        await followHandler.Handle(new FollowCommand(alice.Id, bob.Id), default);

        var followersHandler = new GetFollowersQueryHandler(db);
        var followers = await followersHandler.Handle(new GetFollowersQuery(bob.Id, null, null), default);
        followers.Items.Should().ContainSingle(u => u.Id == alice.Id);

        var followingHandler = new GetFollowingQueryHandler(db);
        var following = await followingHandler.Handle(new GetFollowingQuery(alice.Id, null, null), default);
        following.Items.Should().ContainSingle(u => u.Id == bob.Id);

        var unfollowHandler = new UnfollowCommandHandler(db);
        await unfollowHandler.Handle(new UnfollowCommand(alice.Id, bob.Id), default);
        db.Follows.Count().Should().Be(0);
    }
}
