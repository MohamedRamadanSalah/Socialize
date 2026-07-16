using MediatR;
using Microsoft.EntityFrameworkCore;
using Socialize.Application.Abstractions;
using Socialize.Application.Common.Dtos;
using Socialize.Application.Common.Mapping;
using Socialize.Application.Common.Pagination;

namespace Socialize.Application.Engagement.Queries.GetComments;

public record GetCommentsQuery(Guid PostId, string? Cursor, int? Limit) : IRequest<CursorPage<CommentDto>>;

public class GetCommentsQueryHandler : IRequestHandler<GetCommentsQuery, CursorPage<CommentDto>>
{
    private readonly IApplicationDbContext _db;

    public GetCommentsQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<CursorPage<CommentDto>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
    {
        if (!CursorCodec.TryDecode(request.Cursor, out var seek))
        {
            return new CursorPage<CommentDto>(Array.Empty<CommentDto>(), null);
        }

        var limit = PageSize.Clamp(request.Limit);
        var query = _db.Comments.Where(c => c.PostId == request.PostId);

        if (seek is not null)
        {
            var (createdAt, id) = seek.Value;
            query = query.Where(c => c.CreatedAt < createdAt || (c.CreatedAt == createdAt && c.Id.CompareTo(id) < 0));
        }

        var page = await query
            .Include(c => c.Author)
            .OrderByDescending(c => c.CreatedAt).ThenByDescending(c => c.Id)
            .Take(limit + 1)
            .ToListAsync(cancellationToken);

        var hasMore = page.Count > limit;
        var items = page.Take(limit).ToList();
        var nextCursor = hasMore ? CursorCodec.Encode(items[^1].CreatedAt, items[^1].Id) : null;

        var dtos = items
            .Select(c => new CommentDto(c.Id, c.PostId, ProfileMapper.ToSummaryDto(c.Author!), c.Content, c.CreatedAt))
            .ToList();

        return new CursorPage<CommentDto>(dtos, nextCursor);
    }
}
