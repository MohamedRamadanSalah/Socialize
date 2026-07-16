namespace Socialize.Application.Abstractions;

public interface ITokenService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);

    /// <summary>Generates a new cryptographically random opaque refresh token (returned to the client).</summary>
    string GenerateRefreshToken();

    /// <summary>Hashes an opaque refresh token for at-rest storage.</summary>
    string HashRefreshToken(string rawToken);
}
