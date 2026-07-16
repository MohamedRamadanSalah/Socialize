namespace Socialize.Application.Common.Dtos;

public record UserProfileDto(
    Guid Id,
    string UserName,
    string DisplayName,
    string? Bio,
    string? AvatarUrl,
    int FollowerCount,
    int FollowingCount,
    bool IsFollowedByMe,
    DateTimeOffset CreatedAt);
