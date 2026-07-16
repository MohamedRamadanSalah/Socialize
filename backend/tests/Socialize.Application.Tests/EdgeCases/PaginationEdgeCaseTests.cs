using FluentAssertions;
using Socialize.Application.Common.Pagination;
using Socialize.Application.Posts.Queries.GetFeed;
using Socialize.Application.Tests.TestSupport;
using Socialize.Domain.Entities;
using Xunit;

namespace Socialize.Application.Tests.EdgeCases;

/// <summary>
/// Spec Edge Cases: "A pagination cursor that is invalid or points past the end returns an
/// empty page rather than an error." Covers both the pure codec and one representative handler.
/// </summary>
public class PaginationEdgeCaseTests
{
    [Fact]
    public void CursorCodec_MalformedCursor_ReturnsFalse_NotThrows()
    {
        var ok = CursorCodec.TryDecode("%%%not-base64%%%", out var value);

        ok.Should().BeFalse();
        value.Should().BeNull();
    }

    [Fact]
    public void CursorCodec_NoCursor_ReturnsTrueWithNullValue_MeaningFirstPage()
    {
        var ok = CursorCodec.TryDecode(null, out var value);

        ok.Should().BeTrue();
        value.Should().BeNull();
    }

    [Fact]
    public async Task GetFeed_WithMalformedCursor_ReturnsEmptyPage_NotError()
    {
        var db = TestDbContextFactory.Create();
        var alice = new User { Id = Guid.NewGuid(), UserName = "alice", Email = "alice@example.com", PasswordHash = "x", DisplayName = "Alice", CreatedAt = DateTimeOffset.UtcNow };
        db.Users.Add(alice);
        db.SaveChanges();

        var handler = new GetFeedQueryHandler(db);
        var result = await handler.Handle(new GetFeedQuery(alice.Id, "this-is-not-a-valid-cursor", null), default);

        result.Items.Should().BeEmpty();
        result.NextCursor.Should().BeNull();
    }
}
