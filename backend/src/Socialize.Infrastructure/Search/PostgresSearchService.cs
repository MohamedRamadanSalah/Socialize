using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;
using Socialize.Application.Abstractions;
using Socialize.Application.Common.Dtos;
using Socialize.Application.Common.Pagination;
using Socialize.Domain.Entities;

namespace Socialize.Infrastructure.Search;

/// <summary>
/// PostgreSQL full-text search (research R10). Kept out of the Application layer because it uses
/// provider-specific tsvector/tsquery/ts_rank constructs. Pagination is keyset on (rank, CreatedAt, Id)
/// since ranked results need the rank value as part of the seek predicate, not just CreatedAt/Id.
/// EF.Functions.WebSearchToTsQuery must appear inline inside each LINQ expression tree (not hoisted
/// to a local variable) — calling it outside a translated query throws at runtime. Post search is a
/// two-step fetch: rank+page a lightweight {Id, CreatedAt, Rank} projection first, then load the full
/// Post entities (with Include) for just that page's ids — Include cannot follow a Select into an
/// anonymous type, so the two cannot be combined in one query.
/// </summary>
public class PostgresSearchService : ISearchService
{
    private readonly IApplicationDbContext _db;

    public PostgresSearchService(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<CursorPage<UserSummaryDto>> SearchUsersAsync(Guid callerId, string query, string? cursor, int limit, CancellationToken cancellationToken = default)
    {
        var candidates = _db.Users
            .Where(u => EF.Property<NpgsqlTsVector>(u, "SearchVector").Matches(EF.Functions.WebSearchToTsQuery("english", query)))
            .Select(u => new
            {
                u.Id,
                u.UserName,
                u.DisplayName,
                u.AvatarUrl,
                u.CreatedAt,
                Rank = EF.Property<NpgsqlTsVector>(u, "SearchVector").Rank(EF.Functions.WebSearchToTsQuery("english", query))
            });

        if (!RankCursor.TryDecode(cursor, out var seek))
        {
            return new CursorPage<UserSummaryDto>(Array.Empty<UserSummaryDto>(), null);
        }

        if (seek is not null)
        {
            var (rank, createdAt, id) = seek.Value;
            candidates = candidates.Where(c =>
                c.Rank < rank ||
                (c.Rank == rank && c.CreatedAt < createdAt) ||
                (c.Rank == rank && c.CreatedAt == createdAt && c.Id.CompareTo(id) < 0));
        }

        var page = await candidates
            .OrderByDescending(c => c.Rank).ThenByDescending(c => c.CreatedAt).ThenByDescending(c => c.Id)
            .Take(limit + 1)
            .ToListAsync(cancellationToken);

        var hasMore = page.Count > limit;
        var items = page.Take(limit).ToList();
        var nextCursor = hasMore && items.Count > 0
            ? RankCursor.Encode(items[^1].Rank, items[^1].CreatedAt, items[^1].Id)
            : null;

        return new CursorPage<UserSummaryDto>(
            items.Select(i => new UserSummaryDto(i.Id, i.UserName, i.DisplayName, i.AvatarUrl)).ToList(),
            nextCursor);
    }

    public async Task<CursorPage<PostDto>> SearchPostsAsync(Guid callerId, string query, string? cursor, int limit, CancellationToken cancellationToken = default)
    {
        // Step 1: rank + page over a lightweight projection (no Include here).
        var candidates = _db.Posts
            .Where(p => EF.Property<NpgsqlTsVector>(p, "SearchVector").Matches(EF.Functions.WebSearchToTsQuery("english", query)))
            .Select(p => new
            {
                p.Id,
                p.CreatedAt,
                Rank = EF.Property<NpgsqlTsVector>(p, "SearchVector").Rank(EF.Functions.WebSearchToTsQuery("english", query))
            });

        if (!RankCursor.TryDecode(cursor, out var seek))
        {
            return new CursorPage<PostDto>(Array.Empty<PostDto>(), null);
        }

        if (seek is not null)
        {
            var (rank, createdAt, id) = seek.Value;
            candidates = candidates.Where(c =>
                c.Rank < rank ||
                (c.Rank == rank && c.CreatedAt < createdAt) ||
                (c.Rank == rank && c.CreatedAt == createdAt && c.Id.CompareTo(id) < 0));
        }

        var rankedPage = await candidates
            .OrderByDescending(c => c.Rank).ThenByDescending(c => c.CreatedAt).ThenByDescending(c => c.Id)
            .Take(limit + 1)
            .ToListAsync(cancellationToken);

        var hasMore = rankedPage.Count > limit;
        var rankedItems = rankedPage.Take(limit).ToList();
        var nextCursor = hasMore && rankedItems.Count > 0
            ? RankCursor.Encode(rankedItems[^1].Rank, rankedItems[^1].CreatedAt, rankedItems[^1].Id)
            : null;

        // Step 2: load full entities (with Include) for just this page's ids, then re-order to
        // match the rank order computed in step 1 (a plain IN-list query has no ORDER BY rank).
        var ids = rankedItems.Select(r => r.Id).ToList();
        var posts = await _db.Posts
            .Where(p => ids.Contains(p.Id))
            .Include(p => p.Author)
            .Include(p => p.Images)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .ToListAsync(cancellationToken);

        var postsById = posts.ToDictionary(p => p.Id);
        var dtos = rankedItems.Select(r =>
        {
            var p = postsById[r.Id];
            return new PostDto(
                p.Id,
                new UserSummaryDto(p.Author!.Id, p.Author.UserName, p.Author.DisplayName, p.Author.AvatarUrl),
                p.Content,
                p.Images.OrderBy(img => img.Order).Select(img => img.Url).ToList(),
                p.Likes.Count,
                p.Comments.Count,
                p.Likes.Any(l => l.UserId == callerId),
                p.CreatedAt,
                p.EditedAt);
        }).ToList();

        return new CursorPage<PostDto>(dtos, nextCursor);
    }
}

public static class RankCursor
{
    public static string Encode(float rank, DateTimeOffset createdAt, Guid id) =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes($"{rank:R}|{createdAt:O}|{id}"));

    public static bool TryDecode(string? cursor, out (float Rank, DateTimeOffset CreatedAt, Guid Id)? value)
    {
        if (string.IsNullOrWhiteSpace(cursor))
        {
            value = null;
            return true;
        }

        try
        {
            var raw = Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
            var parts = raw.Split('|');
            if (parts.Length != 3)
            {
                value = null;
                return false;
            }

            var rank = float.Parse(parts[0], CultureInfo.InvariantCulture);
            var createdAt = DateTimeOffset.Parse(parts[1], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            var id = Guid.Parse(parts[2]);
            value = (rank, createdAt, id);
            return true;
        }
        catch
        {
            value = null;
            return false;
        }
    }
}
