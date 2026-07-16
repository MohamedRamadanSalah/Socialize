using Socialize.Application.Common.Dtos;
using Socialize.Application.Common.Pagination;

namespace Socialize.Application.Abstractions;

/// <summary>
/// PostgreSQL full-text search is provider-specific; this abstraction keeps that detail out of
/// the Application layer's dependency graph while still allowing handlers to invoke it.
/// </summary>
public interface ISearchService
{
    Task<CursorPage<UserSummaryDto>> SearchUsersAsync(Guid callerId, string query, string? cursor, int limit, CancellationToken cancellationToken = default);
    Task<CursorPage<PostDto>> SearchPostsAsync(Guid callerId, string query, string? cursor, int limit, CancellationToken cancellationToken = default);
}
