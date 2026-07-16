using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Socialize.Application.Abstractions;
using Socialize.Application.Common.Dtos;
using Socialize.Application.Common.Exceptions;
using Socialize.Application.Common.Mapping;
using Socialize.Domain.Entities;

namespace Socialize.Application.Auth.Commands.Login;

public record LoginCommand(string UserNameOrEmail, string Password) : IRequest<AuthResponseDto>;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.UserNameOrEmail).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IApplicationDbContext _db;
    private readonly ITokenService _tokenService;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(IApplicationDbContext db, ITokenService tokenService, IJwtService jwtService)
    {
        _db = db;
        _tokenService = tokenService;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var key = request.UserNameOrEmail.ToLower();
        var user = await _db.Users.FirstOrDefaultAsync(
            u => u.UserName.ToLower() == key || u.Email.ToLower() == key,
            cancellationToken);

        if (user is null || !_tokenService.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAppException("Invalid username/email or password.");
        }

        var (accessToken, accessExpiresAt) = _jwtService.GenerateAccessToken(user.Id, user.UserName);
        var rawRefreshToken = _tokenService.GenerateRefreshToken();
        _db.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = _tokenService.HashRefreshToken(rawRefreshToken),
            ExpiresAt = DateTimeOffset.UtcNow.Add(_tokenService.RefreshTokenLifetime),
            CreatedAt = DateTimeOffset.UtcNow
        });

        await _db.SaveChangesAsync(cancellationToken);

        var profile = await ProfileMapper.ToProfileDtoAsync(_db, user, user.Id, cancellationToken);
        return new AuthResponseDto(accessToken, rawRefreshToken, accessExpiresAt, profile);
    }
}
