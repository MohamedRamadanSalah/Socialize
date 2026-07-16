using System.Security.Cryptography;
using Socialize.Application.Abstractions;

namespace Socialize.Infrastructure.Auth;

public class TokenService : ITokenService
{
    private const int BCryptWorkFactor = 11;

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
