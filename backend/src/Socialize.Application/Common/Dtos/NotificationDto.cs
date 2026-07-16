namespace Socialize.Application.Common.Dtos;

public record NotificationDto(
    Guid Id,
    string Type,
    UserSummaryDto Actor,
    Guid? EntityId,
    bool IsRead,
    DateTimeOffset CreatedAt);
