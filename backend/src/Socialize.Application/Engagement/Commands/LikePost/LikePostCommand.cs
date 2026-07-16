using MediatR;
using Microsoft.EntityFrameworkCore;
using Socialize.Application.Abstractions;
using Socialize.Application.Common.Dtos;
using Socialize.Application.Common.Exceptions;
using Socialize.Application.Common.Mapping;
using Socialize.Domain.Entities;
using Socialize.Domain.Enums;

namespace Socialize.Application.Engagement.Commands.LikePost;

public record LikePostCommand(Guid UserId, Guid PostId) : IRequest;

public class LikePostCommandHandler : IRequestHandler<LikePostCommand>
{
    private readonly IApplicationDbContext _db;
    private readonly INotificationPublisher _notificationPublisher;

    public LikePostCommandHandler(IApplicationDbContext db, INotificationPublisher notificationPublisher)
    {
        _db = db;
        _notificationPublisher = notificationPublisher;
    }

    public async Task Handle(LikePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _db.Posts.FirstOrDefaultAsync(p => p.Id == request.PostId, cancellationToken)
            ?? throw new NotFoundException(nameof(Post), request.PostId);

        var alreadyLiked = await _db.Likes.AnyAsync(
            l => l.UserId == request.UserId && l.PostId == request.PostId,
            cancellationToken);

        if (alreadyLiked)
        {
            return;
        }

        _db.Likes.Add(new Like { UserId = request.UserId, PostId = request.PostId, CreatedAt = DateTimeOffset.UtcNow });

        Notification? notification = null;
        if (post.AuthorId != request.UserId)
        {
            notification = new Notification
            {
                Id = Guid.NewGuid(),
                RecipientId = post.AuthorId,
                ActorId = request.UserId,
                Type = NotificationType.Like,
                EntityId = post.Id,
                IsRead = false,
                CreatedAt = DateTimeOffset.UtcNow
            };
            _db.Notifications.Add(notification);
        }

        await _db.SaveChangesAsync(cancellationToken);

        if (notification is not null)
        {
            var actor = await _db.Users.FirstAsync(u => u.Id == request.UserId, cancellationToken);
            var dto = new NotificationDto(notification.Id, notification.Type.ToString(), ProfileMapper.ToSummaryDto(actor), notification.EntityId, notification.IsRead, notification.CreatedAt);
            await _notificationPublisher.PublishAsync(notification.RecipientId, dto, cancellationToken);
        }
    }
}
