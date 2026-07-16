using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socialize.Application.Abstractions;
using Socialize.Application.Common.Dtos;
using Socialize.Application.Common.Pagination;
using Socialize.Application.Posts.Commands.CreatePost;
using Socialize.Application.Posts.Commands.DeletePost;
using Socialize.Application.Posts.Commands.EditPostText;
using Socialize.Application.Posts.Queries.GetFeed;
using Socialize.Application.Posts.Queries.GetPost;
using Socialize.Application.Posts.Queries.GetUserPosts;

namespace Socialize.Api.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class PostsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUser _currentUser;

    public PostsController(IMediator mediator, ICurrentUser currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    public record EditPostRequest(string Content);

    [HttpPost("posts")]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<PostDto>> Create([FromForm] string content, [FromForm] List<IFormFile>? images, CancellationToken cancellationToken)
    {
        var imageUploads = (images ?? new List<IFormFile>())
            .Select(f => new PostImageUpload(f.OpenReadStream(), f.FileName))
            .ToList();

        try
        {
            var result = await _mediator.Send(new CreatePostCommand(_currentUser.UserId, content, imageUploads), cancellationToken);
            return Created(string.Empty, result);
        }
        finally
        {
            foreach (var upload in imageUploads)
            {
                await upload.Content.DisposeAsync();
            }
        }
    }

    [HttpGet("posts/{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PostDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var viewerId = User.Identity?.IsAuthenticated == true ? _currentUser.UserId : (Guid?)null;
        var result = await _mediator.Send(new GetPostQuery(id, viewerId), cancellationToken);
        return Ok(result);
    }

    [HttpPatch("posts/{id:guid}")]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PostDto>> EditText(Guid id, EditPostRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new EditPostTextCommand(id, _currentUser.UserId, request.Content), cancellationToken);
        return Ok(result);
    }

    [HttpDelete("posts/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeletePostCommand(id, _currentUser.UserId), cancellationToken);
        return NoContent();
    }

    [HttpGet("feed")]
    [ProducesResponseType(typeof(CursorPage<PostDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<CursorPage<PostDto>>> GetFeed([FromQuery] string? cursor, [FromQuery] int? limit, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetFeedQuery(_currentUser.UserId, cursor, limit), cancellationToken);
        return Ok(result);
    }

    [HttpGet("users/{id:guid}/posts")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CursorPage<PostDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<CursorPage<PostDto>>> GetUserPosts(Guid id, [FromQuery] string? cursor, [FromQuery] int? limit, CancellationToken cancellationToken)
    {
        var viewerId = User.Identity?.IsAuthenticated == true ? _currentUser.UserId : (Guid?)null;
        var result = await _mediator.Send(new GetUserPostsQuery(id, viewerId, cursor, limit), cancellationToken);
        return Ok(result);
    }
}
