using MediatR;
using Microsoft.EntityFrameworkCore;
using Socialize.Application.Abstractions;
using Socialize.Application.Common.Exceptions;
using Socialize.Domain.Entities;

namespace Socialize.Application.Notifications.Commands.MarkRead;

public record MarkNotificationReadCommand(Guid NotificationId, Guid RecipientId) : IRequest;

public class MarkNotificationReadCommandHandler : IRequestHandler<MarkNotificationReadCommand>
{
    private readonly IApplicationDbContext _db;

    public MarkNotificationReadCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await _db.Notifications.FirstOrDefaultAsync(
            n => n.Id == request.NotificationId && n.RecipientId == request.RecipientId,
            cancellationToken)
            ?? throw new NotFoundException(nameof(Notification), request.NotificationId);

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
