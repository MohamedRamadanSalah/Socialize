using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Socialize.Application.Abstractions;

namespace Socialize.Infrastructure.Auth;

public class TokenService : ITokenService
{
    private const int BCryptWorkFactor = 11;
    private readonly JwtOptions _options;

    public TokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public TimeSpan RefreshTokenLifetime => TimeSpan.FromDays(_options.RefreshTokenDays);

    public string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password, BCryptWorkFactor);

    public bool VerifyPassword(string password, string passwordHash) => BCrypt.Net.BCrypt.Verify(password, passwordHash);

    public string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
    }

    public string HashRefreshToken(string rawToken)
    {
        var bytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexString(bytes);
    }
}
