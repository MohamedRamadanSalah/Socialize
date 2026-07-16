using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Socialize.Application.Abstractions;
using Socialize.Application.Common.Dtos;
using Socialize.Application.Common.Exceptions;
using Socialize.Application.Common.Mapping;
using Socialize.Domain.Entities;
using Socialize.Domain.Enums;

namespace Socialize.Application.Engagement.Commands.AddComment;

public record AddCommentCommand(Guid PostId, Guid AuthorId, string Content) : IRequest<CommentDto>;

public class AddCommentCommandValidator : AbstractValidator<AddCommentCommand>
{
    public AddCommentCommandValidator()
    {
        RuleFor(x => x.Content).NotEmpty().MaximumLength(1000);
    }
}

public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, CommentDto>
{
    private readonly IApplicationDbContext _db;
    private readonly INotificationPublisher _notificationPublisher;

    public AddCommentCommandHandler(IApplicationDbContext db, INotificationPublisher notificationPublisher)
    {
        _db = db;
        _notificationPublisher = notificationPublisher;
    }

    public async Task<CommentDto> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
        var post = await _db.Posts.FirstOrDefaultAsync(p => p.Id == request.PostId, cancellationToken)
            ?? throw new NotFoundException(nameof(Post), request.PostId);

        var author = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.AuthorId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), request.AuthorId);

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            PostId = request.PostId,
            AuthorId = request.AuthorId,
            Content = request.Content,
            CreatedAt = DateTimeOffset.UtcNow
        };
        _db.Comments.Add(comment);

        Notification? notification = null;
        if (post.AuthorId != request.AuthorId)
        {
            notification = new Notification
            {
                Id = Guid.NewGuid(),
                RecipientId = post.AuthorId,
                ActorId = request.AuthorId,
                Type = NotificationType.Comment,
                EntityId = post.Id,
                IsRead = false,
                CreatedAt = DateTimeOffset.UtcNow
            };
            _db.Notifications.Add(notification);
        }

        await _db.SaveChangesAsync(cancellationToken);

        if (notification is not null)
        {
            var dto = new NotificationDto(notification.Id, notification.Type.ToString(), ProfileMapper.ToSummaryDto(author), notification.EntityId, notification.IsRead, notification.CreatedAt);
            await _notificationPublisher.PublishAsync(notification.RecipientId, dto, cancellationToken);
        }

        return new CommentDto(comment.Id, comment.PostId, ProfileMapper.ToSummaryDto(author), comment.Content, comment.CreatedAt);
    }
}
