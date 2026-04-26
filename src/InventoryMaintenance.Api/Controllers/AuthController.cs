using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using InventoryMaintenance.Application.Contracts.Auth;
using InventoryMaintenance.Application.Features.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMaintenance.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new LoginCommand(request), cancellationToken);
        return result is null ? Unauthorized() : Ok(result);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Refresh([FromBody] RefreshRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RefreshCommand(request), cancellationToken);
        return result is null ? Unauthorized() : Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserSummary), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserSummary>> Me(CancellationToken cancellationToken)
    {
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(sub) || !int.TryParse(sub, out var id))
            return Unauthorized();

        var user = await mediator.Send(new GetUserSummaryQuery(id), cancellationToken);
        return user is null ? Unauthorized() : Ok(user);
    }
}
