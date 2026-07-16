using System.Security.Cryptography;
using Socialize.Application.Abstractions;

namespace Socialize.Application.Tests.TestSupport;

/// <summary>Deterministic, fast stand-in for BCrypt/SHA in unit tests (no real cryptography needed here).</summary>
public class FakeTokenService : ITokenService
{
    public TimeSpan RefreshTokenLifetime => TimeSpan.FromDays(7);

    public string HashPassword(string password) => $"hashed:{password}";

    public bool VerifyPassword(string password, string passwordHash) => passwordHash == $"hashed:{password}";

    public string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));

    public string HashRefreshToken(string rawToken) => $"tokenhash:{rawToken}";
}
