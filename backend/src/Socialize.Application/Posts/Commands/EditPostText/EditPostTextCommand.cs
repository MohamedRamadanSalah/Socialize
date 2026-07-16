using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Socialize.Application.Abstractions;
using Socialize.Application.Common.Dtos;
using Socialize.Application.Common.Exceptions;
using Socialize.Application.Common.Mapping;
using Socialize.Domain.Entities;

namespace Socialize.Application.Posts.Commands.EditPostText;

public record EditPostTextCommand(Guid PostId, Guid RequestingUserId, string Content) : IRequest<PostDto>;

public class EditPostTextCommandValidator : AbstractValidator<EditPostTextCommand>
{
    public EditPostTextCommandValidator()
    {
        RuleFor(x => x.Content).NotEmpty().MaximumLength(2000);
    }
}

public class EditPostTextCommandHandler : IRequestHandler<EditPostTextCommand, PostDto>
{
    private readonly IApplicationDbContext _db;

    public EditPostTextCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PostDto> Handle(EditPostTextCommand request, CancellationToken cancellationToken)
    {
        var post = await _db.Posts
            .Include(p => p.Author)
            .Include(p => p.Images)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(p => p.Id == request.PostId, cancellationToken)
            ?? throw new NotFoundException(nameof(Post), request.PostId);

        if (post.AuthorId != request.RequestingUserId)
        {
            throw new ForbiddenException("You can only edit your own posts.");
        }

        post.Content = request.Content;
        post.EditedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);

        return new PostDto(
            post.Id,
            ProfileMapper.ToSummaryDto(post.Author!),
            post.Content,
            post.Images.OrderBy(i => i.Order).Select(i => i.Url).ToList(),
            post.Likes.Count,
            post.Comments.Count,
            post.Likes.Any(l => l.UserId == request.RequestingUserId),
            post.CreatedAt,
            post.EditedAt);
    }
}
