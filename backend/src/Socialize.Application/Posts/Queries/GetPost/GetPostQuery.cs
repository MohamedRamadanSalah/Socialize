using MediatR;
using Microsoft.EntityFrameworkCore;
using Socialize.Application.Abstractions;
using Socialize.Application.Common.Dtos;
using Socialize.Application.Common.Exceptions;
using Socialize.Application.Common.Mapping;
using Socialize.Domain.Entities;

namespace Socialize.Application.Posts.Queries.GetPost;

public record GetPostQuery(Guid PostId, Guid? ViewerId) : IRequest<PostDto>;

public class GetPostQueryHandler : IRequestHandler<GetPostQuery, PostDto>
{
    private readonly IApplicationDbContext _db;

    public GetPostQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PostDto> Handle(GetPostQuery request, CancellationToken cancellationToken)
    {
        var post = await _db.Posts
            .Include(p => p.Author)
            .Include(p => p.Images)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(p => p.Id == request.PostId, cancellationToken)
            ?? throw new NotFoundException(nameof(Post), request.PostId);

        return new PostDto(
            post.Id,
            ProfileMapper.ToSummaryDto(post.Author!),
            post.Content,
            post.Images.OrderBy(i => i.Order).Select(i => i.Url).ToList(),
            post.Likes.Count,
            post.Comments.Count,
            request.ViewerId.HasValue && post.Likes.Any(l => l.UserId == request.ViewerId.Value),
            post.CreatedAt,
            post.EditedAt);
    }
}
