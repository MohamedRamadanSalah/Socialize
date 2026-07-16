using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Socialize.Application.Abstractions;
using Socialize.Application.Common.Dtos;
using Socialize.Application.Common.Exceptions;
using Socialize.Application.Common.Mapping;
using Socialize.Application.Common.Media;
using Socialize.Domain.Entities;

namespace Socialize.Application.Posts.Commands.CreatePost;

public record PostImageUpload(Stream Content, string FileName);

public record CreatePostCommand(Guid AuthorId, string Content, IReadOnlyList<PostImageUpload> Images) : IRequest<PostDto>;

public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
{
    public CreatePostCommandValidator()
    {
        RuleFor(x => x.Content).NotEmpty().MaximumLength(2000);
    }
}

public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, PostDto>
{
    private readonly IApplicationDbContext _db;
    private readonly IFileStorage _fileStorage;

    public CreatePostCommandHandler(IApplicationDbContext db, IFileStorage fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    public async Task<PostDto> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        ImageValidator.ValidateCount(request.Images.Count);

        var author = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.AuthorId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), request.AuthorId);

        var post = new Post
        {
            Id = Guid.NewGuid(),
            AuthorId = request.AuthorId,
            Content = request.Content,
            CreatedAt = DateTimeOffset.UtcNow
        };
        _db.Posts.Add(post);

        var imageUrls = new List<string>();
        for (var i = 0; i < request.Images.Count; i++)
        {
            var image = request.Images[i];

            using var buffer = new MemoryStream();
            await image.Content.CopyToAsync(buffer, cancellationToken);
            buffer.Position = 0;

            var header = new byte[Math.Min(16, buffer.Length)];
            await buffer.ReadAsync(header, 0, header.Length, cancellationToken);
            ImageValidator.ValidateSingle(image.FileName, buffer.Length, header);
            buffer.Position = 0;

            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            var url = await _fileStorage.SaveAsync(buffer, "posts", extension, cancellationToken);
            imageUrls.Add(url);

            _db.PostImages.Add(new PostImage { Id = Guid.NewGuid(), PostId = post.Id, Url = url, Order = i });
        }

        await _db.SaveChangesAsync(cancellationToken);

        return new PostDto(
            post.Id,
            ProfileMapper.ToSummaryDto(author),
            post.Content,
            imageUrls,
            LikeCount: 0,
            CommentCount: 0,
            LikedByMe: false,
            post.CreatedAt,
            EditedAt: null);
    }
}
