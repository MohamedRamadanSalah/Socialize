namespace Socialize.Application.Common.Dtos;

public record PostDto(
    Guid Id,
    UserSummaryDto Author,
    string Content,
    IReadOnlyList<string> ImageUrls,
    int LikeCount,
    int CommentCount,
    bool LikedByMe,
    DateTimeOffset CreatedAt,
    DateTimeOffset? EditedAt);
