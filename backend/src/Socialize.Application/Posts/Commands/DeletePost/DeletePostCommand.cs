using MediatR;
using Microsoft.EntityFrameworkCore;
using Socialize.Application.Abstractions;
using Socialize.Application.Common.Exceptions;
using Socialize.Domain.Entities;

namespace Socialize.Application.Posts.Commands.DeletePost;

public record DeletePostCommand(Guid PostId, Guid RequestingUserId) : IRequest;

public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand>
{
    private readonly IApplicationDbContext _db;
    private readonly IFileStorage _fileStorage;

    public DeletePostCommandHandler(IApplicationDbContext db, IFileStorage fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    public async Task Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _db.Posts
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == request.PostId, cancellationToken)
            ?? throw new NotFoundException(nameof(Post), request.PostId);

        if (post.AuthorId != request.RequestingUserId)
        {
            throw new ForbiddenException("You can only delete your own posts.");
        }

        foreach (var image in post.Images)
        {
            _fileStorage.Delete(image.Url);
        }

        _db.Posts.Remove(post); // cascades to PostImages, Comments, Likes rows (research/data-model)
        await _db.SaveChangesAsync(cancellationToken);
    }
}
