using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socialize.Application.Abstractions;
using Socialize.Application.Common.Dtos;
using Socialize.Application.Common.Pagination;
using Socialize.Application.Engagement.Commands.AddComment;
using Socialize.Application.Engagement.Commands.DeleteComment;
using Socialize.Application.Engagement.Commands.LikePost;
using Socialize.Application.Engagement.Commands.UnlikePost;
using Socialize.Application.Engagement.Queries.GetComments;

namespace Socialize.Api.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class EngagementController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUser _currentUser;

    public EngagementController(IMediator mediator, ICurrentUser currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    public record AddCommentRequest(string Content);

    [HttpPost("posts/{id:guid}/like")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Like(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new LikePostCommand(_currentUser.UserId, id), cancellationToken);
        return NoContent();
    }

    [HttpDelete("posts/{id:guid}/like")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Unlike(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UnlikePostCommand(_currentUser.UserId, id), cancellationToken);
        return NoContent();
    }

    [HttpPost("posts/{id:guid}/comments")]
    [ProducesResponseType(typeof(CommentDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<CommentDto>> AddComment(Guid id, AddCommentRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new AddCommentCommand(id, _currentUser.UserId, request.Content), cancellationToken);
        return Created(string.Empty, result);
    }

    [HttpGet("posts/{id:guid}/comments")]
    [ProducesResponseType(typeof(CursorPage<CommentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<CursorPage<CommentDto>>> GetComments(Guid id, [FromQuery] string? cursor, [FromQuery] int? limit, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCommentsQuery(id, cursor, limit), cancellationToken);
        return Ok(result);
    }

    [HttpDelete("comments/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteComment(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteCommentCommand(id, _currentUser.UserId), cancellationToken);
        return NoContent();
    }
}
