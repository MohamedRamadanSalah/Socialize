using MediatR;
using Microsoft.EntityFrameworkCore;
using Socialize.Application.Abstractions;

namespace Socialize.Application.Users.Commands.Unfollow;

public record UnfollowCommand(Guid FollowerId, Guid FolloweeId) : IRequest;

public class UnfollowCommandHandler : IRequestHandler<UnfollowCommand>
{
    private readonly IApplicationDbContext _db;

    public UnfollowCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Handle(UnfollowCommand request, CancellationToken cancellationToken)
    {
        var existing = await _db.Follows.FirstOrDefaultAsync(
            f => f.FollowerId == request.FollowerId && f.FolloweeId == request.FolloweeId,
            cancellationToken);

        if (existing is not null)
        {
            _db.Follows.Remove(existing);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
