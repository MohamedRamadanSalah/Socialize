using MediatR;
using Microsoft.EntityFrameworkCore;
using Socialize.Application.Abstractions;
using Socialize.Application.Common.Dtos;
using Socialize.Application.Common.Pagination;

namespace Socialize.Application.Users.Queries.GetFollowing;

public record GetFollowingQuery(Guid UserId, string? Cursor, int? Limit) : IRequest<CursorPage<UserSummaryDto>>;

public class GetFollowingQueryHandler : IRequestHandler<GetFollowingQuery, CursorPage<UserSummaryDto>>
{
    private readonly IApplicationDbContext _db;

    public GetFollowingQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<CursorPage<UserSummaryDto>> Handle(GetFollowingQuery request, CancellationToken cancellationToken)
    {
        if (!CursorCodec.TryDecode(request.Cursor, out var seek))
        {
            return new CursorPage<UserSummaryDto>(Array.Empty<UserSummaryDto>(), null);
        }

        var limit = PageSize.Clamp(request.Limit);
        var query = _db.Follows.Where(f => f.FollowerId == request.UserId);

        if (seek is not null)
        {
            var (createdAt, id) = seek.Value;
            query = query.Where(f => f.CreatedAt < createdAt || (f.CreatedAt == createdAt && f.FolloweeId.CompareTo(id) < 0));
        }

        var page = await query
            .OrderByDescending(f => f.CreatedAt).ThenByDescending(f => f.FolloweeId)
            .Take(limit + 1)
            .Select(f => new { f.CreatedAt, f.FolloweeId, Followee = f.FolloweeUser })
            .ToListAsync(cancellationToken);

        var hasMore = page.Count > limit;
        var items = page.Take(limit).ToList();
        var nextCursor = hasMore ? CursorCodec.Encode(items[^1].CreatedAt, items[^1].FolloweeId) : null;

        var dtos = items
            .Select(i => new UserSummaryDto(i.Followee!.Id, i.Followee.UserName, i.Followee.DisplayName, i.Followee.AvatarUrl))
            .ToList();

        return new CursorPage<UserSummaryDto>(dtos, nextCursor);
    }
}
