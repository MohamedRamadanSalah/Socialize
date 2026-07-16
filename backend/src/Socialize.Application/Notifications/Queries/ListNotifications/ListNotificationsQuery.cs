using MediatR;
using Microsoft.EntityFrameworkCore;
using Socialize.Application.Abstractions;
using Socialize.Application.Common.Dtos;
using Socialize.Application.Common.Mapping;
using Socialize.Application.Common.Pagination;

namespace Socialize.Application.Notifications.Queries.ListNotifications;

public record ListNotificationsQuery(Guid RecipientId, string? Cursor, int? Limit) : IRequest<CursorPage<NotificationDto>>;

public class ListNotificationsQueryHandler : IRequestHandler<ListNotificationsQuery, CursorPage<NotificationDto>>
{
    private readonly IApplicationDbContext _db;

    public ListNotificationsQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<CursorPage<NotificationDto>> Handle(ListNotificationsQuery request, CancellationToken cancellationToken)
    {
        if (!CursorCodec.TryDecode(request.Cursor, out var seek))
        {
            return new CursorPage<NotificationDto>(Array.Empty<NotificationDto>(), null);
        }

        var limit = PageSize.Clamp(request.Limit);
        var query = _db.Notifications.Where(n => n.RecipientId == request.RecipientId);

        if (seek is not null)
        {
            var (createdAt, id) = seek.Value;
            query = query.Where(n => n.CreatedAt < createdAt || (n.CreatedAt == createdAt && n.Id.CompareTo(id) < 0));
        }

        var page = await query
            .Include(n => n.Actor)
            .OrderByDescending(n => n.CreatedAt).ThenByDescending(n => n.Id)
            .Take(limit + 1)
            .ToListAsync(cancellationToken);

        var hasMore = page.Count > limit;
        var items = page.Take(limit).ToList();
        var nextCursor = hasMore ? CursorCodec.Encode(items[^1].CreatedAt, items[^1].Id) : null;

        var dtos = items
            .Select(n => new NotificationDto(n.Id, n.Type.ToString(), ProfileMapper.ToSummaryDto(n.Actor!), n.EntityId, n.IsRead, n.CreatedAt))
            .ToList();

        return new CursorPage<NotificationDto>(dtos, nextCursor);
    }
}
