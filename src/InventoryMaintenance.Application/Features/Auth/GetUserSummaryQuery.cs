using InventoryMaintenance.Application.Abstractions;
using InventoryMaintenance.Application.Contracts.Auth;
using MediatR;

namespace InventoryMaintenance.Application.Features.Auth;

public sealed record GetUserSummaryQuery(int StaffId) : IRequest<UserSummary?>;

public sealed class GetUserSummaryQueryHandler(IStaffReadRepository staff)
    : IRequestHandler<GetUserSummaryQuery, UserSummary?>
{
    public async Task<UserSummary?> Handle(GetUserSummaryQuery request, CancellationToken cancellationToken)
    {
        var user = await staff.GetByIdWithRoleAsync(request.StaffId, cancellationToken);
        if (user is null)
            return null;

        return new UserSummary(
            user.Id,
            user.UserName,
            user.Email,
            user.FirstName,
            user.LastName,
            user.TenantId,
            user.DepartmentId,
            user.StaffRoleId,
            user.StaffRole.Name);
    }
}
