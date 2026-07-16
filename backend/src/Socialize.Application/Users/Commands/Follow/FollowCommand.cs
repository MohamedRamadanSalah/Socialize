using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Socialize.Application.Abstractions;
using Socialize.Application.Common.Exceptions;

namespace Socialize.Application.Users.Commands.Follow;

public record FollowCommand(Guid FollowerId, Guid FolloweeId) : IRequest;

public class FollowCommandHandler : IRequestHandler<FollowCommand>
{
    private readonly IApplicationDbContext _db;

    public FollowCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Handle(FollowCommand request, CancellationToken cancellationToken)
    {
        if (request.FollowerId == request.FolloweeId)
        {
            throw new ValidationAppException(new[] { new ValidationFailure("id", "You cannot follow yourself.") });
        }

        var followeeExists = await _db.Users.AnyAsync(u => u.Id == request.FolloweeId, cancellationToken);
        if (!followeeExists)
        {
            throw new NotFoundException("User", request.FolloweeId);
        }

        var alreadyFollowing = await _db.Follows.AnyAsync(
            f => f.FollowerId == request.FollowerId && f.FolloweeId == request.FolloweeId,
            cancellationToken);

        if (!alreadyFollowing)
        {
            _db.Follows.Add(new Socialize.Domain.Entities.Follow
            {
                FollowerId = request.FollowerId,
                FolloweeId = request.FolloweeId,
                CreatedAt = DateTimeOffset.UtcNow
            });
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
