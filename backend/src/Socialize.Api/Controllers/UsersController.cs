using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socialize.Application.Abstractions;
using Socialize.Application.Common.Dtos;
using Socialize.Application.Common.Pagination;
using Socialize.Application.Users.Commands.Follow;
using Socialize.Application.Users.Commands.Unfollow;
using Socialize.Application.Users.Commands.UpdateProfile;
using Socialize.Application.Users.Commands.UploadAvatar;
using Socialize.Application.Users.Queries.GetFollowers;
using Socialize.Application.Users.Queries.GetFollowing;
using Socialize.Application.Users.Queries.GetProfile;

namespace Socialize.Api.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUser _currentUser;

    public UsersController(IMediator mediator, ICurrentUser currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    public record UpdateProfileRequest(string? DisplayName, string? Bio);

    [HttpGet("users/{username}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserProfileDto>> GetByUsername(string username, CancellationToken cancellationToken)
    {
        var viewerId = User.Identity?.IsAuthenticated == true ? _currentUser.UserId : (Guid?)null;
        var result = await _mediator.Send(new GetProfileByUsernameQuery(username, viewerId), cancellationToken);
        return Ok(result);
    }

    [HttpPut("users/me")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserProfileDto>> UpdateMe(UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateProfileCommand(_currentUser.UserId, request.DisplayName, request.Bio), cancellationToken);
        return Ok(result);
    }

    [HttpPost("users/me/avatar")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserProfileDto>> UploadAvatar(IFormFile file, CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();
        var result = await _mediator.Send(new UploadAvatarCommand(_currentUser.UserId, stream, file.FileName), cancellationToken);
        return Ok(result);
    }

    [HttpPost("users/{id:guid}/follow")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Follow(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new FollowCommand(_currentUser.UserId, id), cancellationToken);
        return NoContent();
    }

    [HttpDelete("users/{id:guid}/follow")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Unfollow(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UnfollowCommand(_currentUser.UserId, id), cancellationToken);
        return NoContent();
    }

    [HttpGet("users/{id:guid}/followers")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CursorPage<UserSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<CursorPage<UserSummaryDto>>> GetFollowers(Guid id, [FromQuery] string? cursor, [FromQuery] int? limit, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetFollowersQuery(id, cursor, limit), cancellationToken);
        return Ok(result);
    }

    [HttpGet("users/{id:guid}/following")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CursorPage<UserSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<CursorPage<UserSummaryDto>>> GetFollowing(Guid id, [FromQuery] string? cursor, [FromQuery] int? limit, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetFollowingQuery(id, cursor, limit), cancellationToken);
        return Ok(result);
    }
}
