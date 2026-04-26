using System.Security.Claims;
using InventoryMaintenance.Api.Security;
using InventoryMaintenance.Application.Contracts.Equipment;
using InventoryMaintenance.Application.Features.Equipment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMaintenance.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class EquipmentController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<EquipmentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<EquipmentDto>>> List(CancellationToken cancellationToken)
    {
        var (tenantId, staffRoleId, departmentId) = ReadClaims();
        var list = await mediator.Send(new ListEquipmentQuery(tenantId, staffRoleId, departmentId), cancellationToken);
        return Ok(list);
    }

    [HttpPost]
    [ProducesResponseType(typeof(EquipmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<EquipmentDto>> Upsert([FromBody] UpsertEquipmentRequest request, CancellationToken cancellationToken)
    {
        var (tenantId, staffRoleId, _) = ReadClaims();
        var result = await mediator.Send(new UpsertEquipmentCommand(tenantId, staffRoleId, request), cancellationToken);
        return result.Status switch
        {
            UpsertEquipmentStatus.Forbidden => Forbid(),
            UpsertEquipmentStatus.NotFound => NotFound(),
            UpsertEquipmentStatus.Conflict => Conflict(result.Message),
            _ => Ok(result.Data!),
        };
    }

    private (int tenantId, int staffRoleId, int? departmentId) ReadClaims()
    {
        var tenant = int.Parse(User.FindFirstValue(JwtClaimNames.TenantId)!);
        var role = int.Parse(User.FindFirstValue(JwtClaimNames.StaffRoleId)!);
        var deptClaim = User.FindFirst(JwtClaimNames.DepartmentId)?.Value;
        int? dept = string.IsNullOrEmpty(deptClaim) ? null : int.Parse(deptClaim);
        return (tenant, role, dept);
    }
}
