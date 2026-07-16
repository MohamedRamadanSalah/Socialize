using MediatR;
using Microsoft.EntityFrameworkCore;
using Socialize.Application.Abstractions;

namespace Socialize.Application.Notifications.Commands.MarkAllRead;

public record MarkAllNotificationsReadCommand(Guid RecipientId) : IRequest;

public class MarkAllNotificationsReadCommandHandler : IRequestHandler<MarkAllNotificationsReadCommand>
{
    private readonly IApplicationDbContext _db;

    public MarkAllNotificationsReadCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Handle(MarkAllNotificationsReadCommand request, CancellationToken cancellationToken)
    {
        var unread = await _db.Notifications
            .Where(n => n.RecipientId == request.RecipientId && !n.IsRead)
            .ToListAsync(cancellationToken);

        if (unread.Count == 0)
        {
            return;
        }

        foreach (var notification in unread)
        {
            notification.IsRead = true;
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
