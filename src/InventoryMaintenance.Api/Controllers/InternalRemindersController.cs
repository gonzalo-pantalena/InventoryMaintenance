using InventoryMaintenance.Api.Options;
using InventoryMaintenance.Application.Features.Internal;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace InventoryMaintenance.Api.Controllers;

[ApiController]
[Route("api/internal")]
public sealed class InternalRemindersController(
    IMediator mediator,
    IOptions<IntegrationOptions> integration) : ControllerBase
{
    [HttpPost("reminders/run")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Run(
        [FromHeader(Name = "X-Api-Key")] string? apiKey,
        [FromQuery] int days = 7,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(apiKey) || apiKey != integration.Value.ServiceApiKey)
            return Unauthorized();

        var horizon = DateTime.UtcNow.AddDays(days);
        await mediator.Send(new RunMaintenanceRemindersCommand(horizon), cancellationToken);

        return Accepted();
    }
}
