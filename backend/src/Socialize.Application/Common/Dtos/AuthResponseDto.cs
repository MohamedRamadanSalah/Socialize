namespace Socialize.Application.Common.Dtos;

public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset AccessTokenExpiresAt,
    UserProfileDto User);
