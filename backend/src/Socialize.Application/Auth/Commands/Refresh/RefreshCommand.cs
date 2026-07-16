using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Socialize.Application.Abstractions;
using Socialize.Application.Common.Dtos;
using Socialize.Application.Common.Exceptions;
using Socialize.Application.Common.Mapping;
using Socialize.Domain.Entities;

namespace Socialize.Application.Auth.Commands.Refresh;

public record RefreshCommand(string RefreshToken) : IRequest<AuthResponseDto>;

public class RefreshCommandValidator : AbstractValidator<RefreshCommand>
{
    public RefreshCommandValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}

public class RefreshCommandHandler : IRequestHandler<RefreshCommand, AuthResponseDto>
{
    private readonly IApplicationDbContext _db;
    private readonly ITokenService _tokenService;
    private readonly IJwtService _jwtService;

    public RefreshCommandHandler(IApplicationDbContext db, ITokenService tokenService, IJwtService jwtService)
    {
        _db = db;
        _tokenService = tokenService;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var presentedHash = _tokenService.HashRefreshToken(request.RefreshToken);
        var existing = await _db.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == presentedHash, cancellationToken);

        if (existing is null || !existing.IsActive || existing.User is null)
        {
            throw new UnauthorizedAppException("Invalid, expired, or revoked refresh token.");
        }

        var now = DateTimeOffset.UtcNow;
        var rawNewRefreshToken = _tokenService.GenerateRefreshToken();
        var newHash = _tokenService.HashRefreshToken(rawNewRefreshToken);

        existing.RevokedAt = now;
        existing.ReplacedByTokenHash = newHash;

        _db.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = existing.UserId,
            TokenHash = newHash,
            ExpiresAt = now.Add(_tokenService.RefreshTokenLifetime),
            CreatedAt = now
        });

        var (accessToken, accessExpiresAt) = _jwtService.GenerateAccessToken(existing.UserId, existing.User.UserName);

        await _db.SaveChangesAsync(cancellationToken);

        var profile = await ProfileMapper.ToProfileDtoAsync(_db, existing.User, existing.UserId, cancellationToken);
        return new AuthResponseDto(accessToken, rawNewRefreshToken, accessExpiresAt, profile);
    }
}
