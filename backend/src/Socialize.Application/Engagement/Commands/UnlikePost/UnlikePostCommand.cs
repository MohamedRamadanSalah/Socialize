using MediatR;
using Microsoft.EntityFrameworkCore;
using Socialize.Application.Abstractions;

namespace Socialize.Application.Engagement.Commands.UnlikePost;

public record UnlikePostCommand(Guid UserId, Guid PostId) : IRequest;

public class UnlikePostCommandHandler : IRequestHandler<UnlikePostCommand>
{
    private readonly IApplicationDbContext _db;

    public UnlikePostCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Handle(UnlikePostCommand request, CancellationToken cancellationToken)
    {
        var existing = await _db.Likes.FirstOrDefaultAsync(
            l => l.UserId == request.UserId && l.PostId == request.PostId,
            cancellationToken);

        if (existing is not null)
        {
            _db.Likes.Remove(existing);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
