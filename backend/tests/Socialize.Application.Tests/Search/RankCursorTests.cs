using FluentAssertions;
using Socialize.Application.Search.Queries.SearchPosts;
using Socialize.Application.Search.Queries.SearchUsers;
using Socialize.Application.Tests.TestSupport;
using Socialize.Infrastructure.Search;
using Xunit;

namespace Socialize.Application.Tests.Search;

/// <summary>
/// PostgresSearchService itself uses Npgsql-specific tsvector/ts_rank LINQ translation, which the
/// EF InMemory provider used elsewhere in this test project cannot execute — that behavior is
/// validated against a real Postgres via quickstart.md Scenario 6 instead. What IS pure C# and
/// unit-testable is the rank-aware cursor codec and the empty-query short-circuit, covered here.
/// </summary>
public class RankCursorTests
{
    [Fact]
    public void Encode_Then_Decode_RoundTrips()
    {
        var rank = 0.607927f;
        var createdAt = DateTimeOffset.UtcNow;
        var id = Guid.NewGuid();

        var cursor = RankCursor.Encode(rank, createdAt, id);
        var ok = RankCursor.TryDecode(cursor, out var decoded);

        ok.Should().BeTrue();
        decoded!.Value.Rank.Should().Be(rank);
        decoded.Value.CreatedAt.Should().Be(createdAt);
        decoded.Value.Id.Should().Be(id);
    }

    [Fact]
    public void TryDecode_NullOrEmptyCursor_ReturnsTrueWithNullValue()
    {
        RankCursor.TryDecode(null, out var decoded).Should().BeTrue();
        decoded.Should().BeNull();
    }

    [Fact]
    public void TryDecode_MalformedCursor_ReturnsFalse()
    {
        RankCursor.TryDecode("not-a-valid-cursor!!", out var decoded).Should().BeFalse();
        decoded.Should().BeNull();
    }

    [Fact]
    public async Task SearchUsersQuery_EmptyQuery_ReturnsEmptyPageWithoutCallingSearchService()
    {
        var fakeSearch = new FakeSearchService();
        var handler = new SearchUsersQueryHandler(fakeSearch);

        var result = await handler.Handle(new SearchUsersQuery(Guid.NewGuid(), "   ", null, null), default);

        result.Items.Should().BeEmpty();
        fakeSearch.SearchUsersCalled.Should().BeFalse();
    }

    [Fact]
    public async Task SearchPostsQuery_EmptyQuery_ReturnsEmptyPageWithoutCallingSearchService()
    {
        var fakeSearch = new FakeSearchService();
        var handler = new SearchPostsQueryHandler(fakeSearch);

        var result = await handler.Handle(new SearchPostsQuery(Guid.NewGuid(), "", null, null), default);

        result.Items.Should().BeEmpty();
        fakeSearch.SearchPostsCalled.Should().BeFalse();
    }
}
