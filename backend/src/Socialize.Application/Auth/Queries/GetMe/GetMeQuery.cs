using MediatR;
using Microsoft.EntityFrameworkCore;
using Socialize.Application.Abstractions;
using Socialize.Application.Common.Dtos;
using Socialize.Application.Common.Exceptions;
using Socialize.Application.Common.Mapping;

namespace Socialize.Application.Auth.Queries.GetMe;

public record GetMeQuery(Guid UserId) : IRequest<UserProfileDto>;

public class GetMeQueryHandler : IRequestHandler<GetMeQuery, UserProfileDto>
{
    private readonly IApplicationDbContext _db;

    public GetMeQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<UserProfileDto> Handle(GetMeQuery request, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.User), request.UserId);

        return await ProfileMapper.ToProfileDtoAsync(_db, user, request.UserId, cancellationToken);
    }
}
