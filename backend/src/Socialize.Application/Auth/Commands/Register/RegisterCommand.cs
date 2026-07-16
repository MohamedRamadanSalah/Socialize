using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Socialize.Application.Abstractions;
using Socialize.Application.Common.Dtos;
using Socialize.Application.Common.Exceptions;
using Socialize.Application.Common.Mapping;
using Socialize.Domain.Entities;

namespace Socialize.Application.Auth.Commands.Register;

public record RegisterCommand(string UserName, string Email, string Password, string DisplayName) : IRequest<AuthResponseDto>;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().MinimumLength(3).MaximumLength(30).Matches("^[a-zA-Z0-9_]+$")
            .WithMessage("Username must be 3-30 characters and contain only letters, numbers, and underscores.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(50);
    }
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IApplicationDbContext _db;
    private readonly ITokenService _tokenService;
    private readonly IJwtService _jwtService;

    public RegisterCommandHandler(IApplicationDbContext db, ITokenService tokenService, IJwtService jwtService)
    {
        _db = db;
        _tokenService = tokenService;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var userNameTaken = await _db.Users.AnyAsync(u => u.UserName.ToLower() == request.UserName.ToLower(), cancellationToken);
        if (userNameTaken)
        {
            throw new ConflictException("Username is already taken.");
        }

        var emailTaken = await _db.Users.AnyAsync(u => u.Email.ToLower() == request.Email.ToLower(), cancellationToken);
        if (emailTaken)
        {
            throw new ConflictException("Email is already taken.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.UserName,
            Email = request.Email,
            PasswordHash = _tokenService.HashPassword(request.Password),
            DisplayName = request.DisplayName,
            CreatedAt = DateTimeOffset.UtcNow
        };
        _db.Users.Add(user);

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
