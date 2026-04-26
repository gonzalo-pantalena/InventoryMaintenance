using System.Security.Claims;
using InventoryMaintenance.Api.Security;
using InventoryMaintenance.Application.Contracts.Lookups;
using InventoryMaintenance.Application.Features.Lookups;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMaintenance.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class LookupsController(IMediator mediator) : ControllerBase
{
    [HttpGet("bundle")]
    [ProducesResponseType(typeof(LookupBundleDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<LookupBundleDto>> Bundle(CancellationToken cancellationToken)
    {
        var tenantId = int.Parse(User.FindFirstValue(JwtClaimNames.TenantId)!);
        var roleId = int.Parse(User.FindFirstValue(JwtClaimNames.StaffRoleId)!);
        var deptClaim = User.FindFirst(JwtClaimNames.DepartmentId)?.Value;
        int? userDept = string.IsNullOrEmpty(deptClaim) ? null : int.Parse(deptClaim);

        var bundle = await mediator.Send(new GetLookupBundleQuery(tenantId, roleId, userDept), cancellationToken);
        return Ok(bundle);
    }
}
