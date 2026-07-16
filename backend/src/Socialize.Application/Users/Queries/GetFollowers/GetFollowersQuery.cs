using MediatR;
using Microsoft.EntityFrameworkCore;
using Socialize.Application.Abstractions;
using Socialize.Application.Common.Dtos;
using Socialize.Application.Common.Pagination;

namespace Socialize.Application.Users.Queries.GetFollowers;

public record GetFollowersQuery(Guid UserId, string? Cursor, int? Limit) : IRequest<CursorPage<UserSummaryDto>>;

public class GetFollowersQueryHandler : IRequestHandler<GetFollowersQuery, CursorPage<UserSummaryDto>>
{
    private readonly IApplicationDbContext _db;

    public GetFollowersQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<CursorPage<UserSummaryDto>> Handle(GetFollowersQuery request, CancellationToken cancellationToken)
    {
        if (!CursorCodec.TryDecode(request.Cursor, out var seek))
        {
            return new CursorPage<UserSummaryDto>(Array.Empty<UserSummaryDto>(), null);
        }

        var limit = PageSize.Clamp(request.Limit);
        var query = _db.Follows.Where(f => f.FolloweeId == request.UserId);

        if (seek is not null)
        {
            var (createdAt, id) = seek.Value;
            query = query.Where(f => f.CreatedAt < createdAt || (f.CreatedAt == createdAt && f.FollowerId.CompareTo(id) < 0));
        }

        var page = await query
            .OrderByDescending(f => f.CreatedAt).ThenByDescending(f => f.FollowerId)
            .Take(limit + 1)
            .Select(f => new { f.CreatedAt, f.FollowerId, Follower = f.FollowerUser })
            .ToListAsync(cancellationToken);

        var hasMore = page.Count > limit;
        var items = page.Take(limit).ToList();
        var nextCursor = hasMore ? CursorCodec.Encode(items[^1].CreatedAt, items[^1].FollowerId) : null;

        var dtos = items
            .Select(i => new UserSummaryDto(i.Follower!.Id, i.Follower.UserName, i.Follower.DisplayName, i.Follower.AvatarUrl))
            .ToList();

        return new CursorPage<UserSummaryDto>(dtos, nextCursor);
    }
}
