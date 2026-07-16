namespace Socialize.Application.Abstractions;

public interface IJwtService
{
    (string Token, DateTimeOffset ExpiresAt) GenerateAccessToken(Guid userId, string userName);
}
