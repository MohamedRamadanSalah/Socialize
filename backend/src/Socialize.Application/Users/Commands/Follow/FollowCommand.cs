using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Socialize.Application.Abstractions;
using Socialize.Application.Common.Dtos;
using Socialize.Application.Common.Exceptions;
using Socialize.Application.Common.Mapping;
using Socialize.Domain.Entities;
using Socialize.Domain.Enums;

namespace Socialize.Application.Users.Commands.Follow;

public record FollowCommand(Guid FollowerId, Guid FolloweeId) : IRequest;

public class FollowCommandHandler : IRequestHandler<FollowCommand>
{
    private readonly IApplicationDbContext _db;
    private readonly INotificationPublisher _notificationPublisher;

    public FollowCommandHandler(IApplicationDbContext db, INotificationPublisher notificationPublisher)
    {
        _db = db;
        _notificationPublisher = notificationPublisher;
    }

    public async Task Handle(FollowCommand request, CancellationToken cancellationToken)
    {
        if (request.FollowerId == request.FolloweeId)
        {
            throw new ValidationAppException(new[] { new ValidationFailure("id", "You cannot follow yourself.") });
        }

        var follower = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.FollowerId, cancellationToken)
            ?? throw new NotFoundException("User", request.FollowerId);
        var followeeExists = await _db.Users.AnyAsync(u => u.Id == request.FolloweeId, cancellationToken);
        if (!followeeExists)
        {
            throw new NotFoundException("User", request.FolloweeId);
        }

        var alreadyFollowing = await _db.Follows.AnyAsync(
            f => f.FollowerId == request.FollowerId && f.FolloweeId == request.FolloweeId,
            cancellationToken);

        if (alreadyFollowing)
        {
            return;
        }

        _db.Follows.Add(new Socialize.Domain.Entities.Follow
        {
            FollowerId = request.FollowerId,
            FolloweeId = request.FolloweeId,
            CreatedAt = DateTimeOffset.UtcNow
        });

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            RecipientId = request.FolloweeId,
            ActorId = request.FollowerId,
            Type = NotificationType.Follow,
            EntityId = null,
            IsRead = false,
            CreatedAt = DateTimeOffset.UtcNow
        };
        _db.Notifications.Add(notification);

        await _db.SaveChangesAsync(cancellationToken);

        var dto = new NotificationDto(notification.Id, notification.Type.ToString(), ProfileMapper.ToSummaryDto(follower), notification.EntityId, notification.IsRead, notification.CreatedAt);
        await _notificationPublisher.PublishAsync(notification.RecipientId, dto, cancellationToken);
    }
}
