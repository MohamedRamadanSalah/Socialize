using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socialize.Application.Abstractions;
using Socialize.Application.Common.Dtos;
using Socialize.Application.Common.Pagination;
using Socialize.Application.Search.Queries.SearchPosts;
using Socialize.Application.Search.Queries.SearchUsers;

namespace Socialize.Api.Controllers;

[ApiController]
[Route("api/search")]
[Authorize]
public class SearchController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUser _currentUser;

    public SearchController(IMediator mediator, ICurrentUser currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpGet("users")]
    [ProducesResponseType(typeof(CursorPage<UserSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<CursorPage<UserSummaryDto>>> SearchUsers([FromQuery] string q, [FromQuery] string? cursor, [FromQuery] int? limit, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SearchUsersQuery(_currentUser.UserId, q, cursor, limit), cancellationToken);
        return Ok(result);
    }

    [HttpGet("posts")]
    [ProducesResponseType(typeof(CursorPage<PostDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<CursorPage<PostDto>>> SearchPosts([FromQuery] string q, [FromQuery] string? cursor, [FromQuery] int? limit, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SearchPostsQuery(_currentUser.UserId, q, cursor, limit), cancellationToken);
        return Ok(result);
    }
}
