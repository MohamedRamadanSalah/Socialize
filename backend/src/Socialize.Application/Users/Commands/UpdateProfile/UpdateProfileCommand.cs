using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Socialize.Application.Abstractions;
using Socialize.Application.Common.Dtos;
using Socialize.Application.Common.Exceptions;
using Socialize.Application.Common.Mapping;
using Socialize.Domain.Entities;

namespace Socialize.Application.Users.Commands.UpdateProfile;

public record UpdateProfileCommand(Guid UserId, string? DisplayName, string? Bio) : IRequest<UserProfileDto>;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.DisplayName).MinimumLength(1).MaximumLength(50).When(x => x.DisplayName is not null);
        RuleFor(x => x.Bio).MaximumLength(300).When(x => x.Bio is not null);
    }
}

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, UserProfileDto>
{
    private readonly IApplicationDbContext _db;

    public UpdateProfileCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<UserProfileDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), request.UserId);

        if (request.DisplayName is not null)
        {
            user.DisplayName = request.DisplayName;
        }

        if (request.Bio is not null)
        {
            user.Bio = request.Bio;
        }

        await _db.SaveChangesAsync(cancellationToken);

        return await ProfileMapper.ToProfileDtoAsync(_db, user, request.UserId, cancellationToken);
    }
}
