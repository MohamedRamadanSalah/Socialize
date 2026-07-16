using MediatR;
using Microsoft.EntityFrameworkCore;
using Socialize.Application.Abstractions;
using Socialize.Application.Common.Dtos;
using Socialize.Application.Common.Mapping;
using Socialize.Application.Common.Pagination;
using Socialize.Domain.Entities;

namespace Socialize.Application.Posts.Queries.GetUserPosts;

public record GetUserPostsQuery(Guid AuthorId, Guid? ViewerId, string? Cursor, int? Limit) : IRequest<CursorPage<PostDto>>;

public class GetUserPostsQueryHandler : IRequestHandler<GetUserPostsQuery, CursorPage<PostDto>>
{
    private readonly IApplicationDbContext _db;

    public GetUserPostsQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<CursorPage<PostDto>> Handle(GetUserPostsQuery request, CancellationToken cancellationToken)
    {
        if (!CursorCodec.TryDecode(request.Cursor, out var seek))
        {
            return new CursorPage<PostDto>(Array.Empty<PostDto>(), null);
        }

        var limit = PageSize.Clamp(request.Limit);
        var query = _db.Posts.Where(p => p.AuthorId == request.AuthorId);

        if (seek is not null)
        {
            var (createdAt, id) = seek.Value;
            query = query.Where(p => p.CreatedAt < createdAt || (p.CreatedAt == createdAt && p.Id.CompareTo(id) < 0));
        }

        var page = await query
            .Include(p => p.Author)
            .Include(p => p.Images)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .OrderByDescending(p => p.CreatedAt).ThenByDescending(p => p.Id)
            .Take(limit + 1)
            .ToListAsync(cancellationToken);

        var hasMore = page.Count > limit;
        var items = page.Take(limit).ToList();
        var nextCursor = hasMore ? CursorCodec.Encode(items[^1].CreatedAt, items[^1].Id) : null;

        var viewerId = request.ViewerId ?? Guid.Empty;
        var dtos = items.Select(p => new PostDto(
            p.Id,
            ProfileMapper.ToSummaryDto(p.Author!),
            p.Content,
            p.Images.OrderBy(i => i.Order).Select(i => i.Url).ToList(),
            p.Likes.Count,
            p.Comments.Count,
            p.Likes.Any(l => l.UserId == viewerId),
            p.CreatedAt,
            p.EditedAt)).ToList();

        return new CursorPage<PostDto>(dtos, nextCursor);
    }
}
