using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socialize.Application.Auth.Commands.Login;
using Socialize.Application.Auth.Commands.Logout;
using Socialize.Application.Auth.Commands.Refresh;
using Socialize.Application.Auth.Commands.Register;
using Socialize.Application.Auth.Queries.GetMe;
using Socialize.Application.Common.Dtos;

namespace Socialize.Api.Controllers;

[ApiController]
[Route("api/auth")]
[Authorize]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public record RegisterRequest(string UserName, string Email, string Password, string DisplayName);
    public record LoginRequest(string UserNameOrEmail, string Password);
    public record RefreshRequest(string RefreshToken);

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RegisterCommand(request.UserName, request.Email, request.Password, request.DisplayName), cancellationToken);
        return Created(string.Empty, result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new LoginCommand(request.UserNameOrEmail, request.Password), cancellationToken);
        return Ok(result);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResponseDto>> Refresh(RefreshRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RefreshCommand(request.RefreshToken), cancellationToken);
        return Ok(result);
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(RefreshRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new LogoutCommand(request.RefreshToken), cancellationToken);
        return NoContent();
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserProfileDto>> Me(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMeQuery(CurrentUserId), cancellationToken);
        return Ok(result);
    }
}
