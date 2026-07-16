using Socialize.Application.Abstractions;
using Socialize.Application.Common.Dtos;
using Socialize.Application.Common.Pagination;

namespace Socialize.Application.Tests.TestSupport;

public class FakeSearchService : ISearchService
{
    public bool SearchUsersCalled { get; private set; }
    public bool SearchPostsCalled { get; private set; }

    public Task<CursorPage<UserSummaryDto>> SearchUsersAsync(Guid callerId, string query, string? cursor, int limit, CancellationToken cancellationToken = default)
    {
        SearchUsersCalled = true;
        return Task.FromResult(new CursorPage<UserSummaryDto>(Array.Empty<UserSummaryDto>(), null));
    }

    public Task<CursorPage<PostDto>> SearchPostsAsync(Guid callerId, string query, string? cursor, int limit, CancellationToken cancellationToken = default)
    {
        SearchPostsCalled = true;
        return Task.FromResult(new CursorPage<PostDto>(Array.Empty<PostDto>(), null));
    }
}
