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
        var tsQuery = EF.Functions.WebSearchToTsQuery("english", query);

        var candidates = _db.Users
            .Where(u => EF.Property<NpgsqlTsVector>(u, "SearchVector").Matches(tsQuery))
            .Select(u => new
            {
                u.Id,
                u.UserName,
                u.DisplayName,
                u.AvatarUrl,
                u.CreatedAt,
                Rank = EF.Property<NpgsqlTsVector>(u, "SearchVector").Rank(tsQuery)
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
        var tsQuery = EF.Functions.WebSearchToTsQuery("english", query);

        var candidates = _db.Posts
            .Where(p => EF.Property<NpgsqlTsVector>(p, "SearchVector").Matches(tsQuery))
            .Select(p => new
            {
                Post = p,
                Rank = EF.Property<NpgsqlTsVector>(p, "SearchVector").Rank(tsQuery)
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
                (c.Rank == rank && c.Post.CreatedAt < createdAt) ||
                (c.Rank == rank && c.Post.CreatedAt == createdAt && c.Post.Id.CompareTo(id) < 0));
        }

        var page = await candidates
            .OrderByDescending(c => c.Rank).ThenByDescending(c => c.Post.CreatedAt).ThenByDescending(c => c.Post.Id)
            .Take(limit + 1)
            .Include(c => c.Post.Author)
            .Include(c => c.Post.Images)
            .Include(c => c.Post.Likes)
            .Include(c => c.Post.Comments)
            .ToListAsync(cancellationToken);

        var hasMore = page.Count > limit;
        var items = page.Take(limit).ToList();
        var nextCursor = hasMore && items.Count > 0
            ? RankCursor.Encode(items[^1].Rank, items[^1].Post.CreatedAt, items[^1].Post.Id)
            : null;

        var dtos = items.Select(i =>
        {
            var p = i.Post;
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
