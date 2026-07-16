using Socialize.Application.Abstractions;

namespace Socialize.Application.Tests.TestSupport;

public class FakeJwtService : IJwtService
{
    public (string Token, DateTimeOffset ExpiresAt) GenerateAccessToken(Guid userId, string userName) =>
        ($"access-token-for-{userId}", DateTimeOffset.UtcNow.AddMinutes(15));
}
