using MediatR;
using Microsoft.EntityFrameworkCore;
using Socialize.Application.Abstractions;
using Socialize.Application.Common.Dtos;
using Socialize.Application.Common.Exceptions;
using Socialize.Application.Common.Mapping;
using Socialize.Application.Common.Media;
using Socialize.Domain.Entities;

namespace Socialize.Application.Users.Commands.UploadAvatar;

public record UploadAvatarCommand(Guid UserId, Stream Content, string FileName) : IRequest<UserProfileDto>;

public class UploadAvatarCommandHandler : IRequestHandler<UploadAvatarCommand, UserProfileDto>
{
    private readonly IApplicationDbContext _db;
    private readonly IFileStorage _fileStorage;

    public UploadAvatarCommandHandler(IApplicationDbContext db, IFileStorage fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    public async Task<UserProfileDto> Handle(UploadAvatarCommand request, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), request.UserId);

        using var buffer = new MemoryStream();
        await request.Content.CopyToAsync(buffer, cancellationToken);
        buffer.Position = 0;

        var header = new byte[Math.Min(16, buffer.Length)];
        await buffer.ReadAsync(header, 0, header.Length, cancellationToken);
        ImageValidator.ValidateSingle(request.FileName, buffer.Length, header);
        buffer.Position = 0;

        var previousAvatarUrl = user.AvatarUrl;
        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();
        var url = await _fileStorage.SaveAsync(buffer, "avatars", extension, cancellationToken);

        user.AvatarUrl = url;
        await _db.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrEmpty(previousAvatarUrl))
        {
            _fileStorage.Delete(previousAvatarUrl);
        }

        return await ProfileMapper.ToProfileDtoAsync(_db, user, request.UserId, cancellationToken);
    }
}
