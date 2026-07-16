using MediatR;
using Microsoft.EntityFrameworkCore;
using Socialize.Application.Abstractions;
using Socialize.Application.Common.Exceptions;
using Socialize.Domain.Entities;

namespace Socialize.Application.Engagement.Commands.DeleteComment;

public record DeleteCommentCommand(Guid CommentId, Guid RequestingUserId) : IRequest;

public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand>
{
    private readonly IApplicationDbContext _db;

    public DeleteCommentCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _db.Comments.FirstOrDefaultAsync(c => c.Id == request.CommentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Comment), request.CommentId);

        if (comment.AuthorId != request.RequestingUserId)
        {
            throw new ForbiddenException("You can only delete your own comments.");
        }

        _db.Comments.Remove(comment);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
