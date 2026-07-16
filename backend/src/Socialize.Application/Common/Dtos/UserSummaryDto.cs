namespace Socialize.Application.Common.Dtos;

public record UserSummaryDto(Guid Id, string UserName, string DisplayName, string? AvatarUrl);
