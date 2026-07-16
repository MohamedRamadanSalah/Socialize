using MediatR;
using Microsoft.EntityFrameworkCore;
using Socialize.Application.Abstractions;
using Socialize.Application.Common.Dtos;
using Socialize.Application.Common.Exceptions;
using Socialize.Application.Common.Mapping;
using Socialize.Domain.Entities;

namespace Socialize.Application.Users.Queries.GetProfile;

public record GetProfileByUsernameQuery(string UserName, Guid? ViewerId) : IRequest<UserProfileDto>;

public class GetProfileByUsernameQueryHandler : IRequestHandler<GetProfileByUsernameQuery, UserProfileDto>
{
    private readonly IApplicationDbContext _db;

    public GetProfileByUsernameQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<UserProfileDto> Handle(GetProfileByUsernameQuery request, CancellationToken cancellationToken)
    {
        var key = request.UserName.ToLower();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == key, cancellationToken)
            ?? throw new NotFoundException(nameof(User), request.UserName);

        return await ProfileMapper.ToProfileDtoAsync(_db, user, request.ViewerId, cancellationToken);
    }
}
