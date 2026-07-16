using Microsoft.EntityFrameworkCore;
using Socialize.Application.Abstractions;
using Socialize.Application.Common.Dtos;
using Socialize.Domain.Entities;

namespace Socialize.Application.Common.Mapping;

public static class ProfileMapper
{
    public static async Task<UserProfileDto> ToProfileDtoAsync(IApplicationDbContext db, User user, Guid? viewerId, CancellationToken cancellationToken)
    {
        var followerCount = await db.Follows.CountAsync(f => f.FolloweeId == user.Id, cancellationToken);
        var followingCount = await db.Follows.CountAsync(f => f.FollowerId == user.Id, cancellationToken);

        var isFollowedByMe = viewerId.HasValue && viewerId.Value != user.Id &&
            await db.Follows.AnyAsync(f => f.FollowerId == viewerId.Value && f.FolloweeId == user.Id, cancellationToken);

        return new UserProfileDto(
            user.Id,
            user.UserName,
            user.DisplayName,
            user.Bio,
            user.AvatarUrl,
            followerCount,
            followingCount,
            isFollowedByMe,
            user.CreatedAt);
    }

    public static UserSummaryDto ToSummaryDto(User user) =>
        new(user.Id, user.UserName, user.DisplayName, user.AvatarUrl);
}
