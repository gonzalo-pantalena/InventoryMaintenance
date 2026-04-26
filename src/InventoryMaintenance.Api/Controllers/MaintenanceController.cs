using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using InventoryMaintenance.Api.Security;
using InventoryMaintenance.Application.Contracts.Maintenance;
using InventoryMaintenance.Application.Features.Maintenance;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMaintenance.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class MaintenanceController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<MaintenanceEventDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<MaintenanceEventDto>>> List(
        [FromQuery] int equipmentId,
        CancellationToken cancellationToken)
    {
        var (tenantId, roleId, deptId) = ReadClaims();
        var result = await mediator.Send(
            new ListMaintenanceEventsQuery(tenantId, roleId, deptId, equipmentId),
            cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(IReadOnlyList<MaintenanceEventDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<MaintenanceEventDto>>> Create(
        [FromBody] CreateMaintenanceRequest request,
        CancellationToken cancellationToken)
    {
        var (tenantId, roleId, deptId) = ReadClaims();
        var sub = int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
        var result = await mediator.Send(
            new CreateMaintenanceCommand(tenantId, roleId, deptId, sub, request),
            cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPatch]
    [ProducesResponseType(typeof(IReadOnlyList<MaintenanceEventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<MaintenanceEventDto>>> Update(
        [FromBody] UpdateMaintenanceRequest request,
        CancellationToken cancellationToken)
    {
        var (tenantId, roleId, _) = ReadClaims();
        var result = await mediator.Send(
            new UpdateMaintenanceCommand(tenantId, roleId, request),
            cancellationToken);
        return result.Status switch
        {
            UpdateMaintenanceStatus.Forbidden => Forbid(),
            UpdateMaintenanceStatus.NotFound => NotFound(),
            _ => Ok(result.Events!),
        };
    }

    private (int tenantId, int roleId, int? departmentId) ReadClaims()
    {
        var tenant = int.Parse(User.FindFirstValue(JwtClaimNames.TenantId)!);
        var role = int.Parse(User.FindFirstValue(JwtClaimNames.StaffRoleId)!);
        var deptClaim = User.FindFirst(JwtClaimNames.DepartmentId)?.Value;
        int? dept = string.IsNullOrEmpty(deptClaim) ? null : int.Parse(deptClaim);
        return (tenant, role, dept);
    }
}
