namespace Socialize.Application.Common.Dtos;

public record CommentDto(Guid Id, Guid PostId, UserSummaryDto Author, string Content, DateTimeOffset CreatedAt);
