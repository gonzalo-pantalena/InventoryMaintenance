using InventoryMaintenance.Application.Abstractions;
using InventoryMaintenance.Application.Contracts.Lookups;
using MediatR;

namespace InventoryMaintenance.Application.Features.Lookups;

public sealed record GetLookupBundleQuery(
    int TenantId,
    int StaffRoleId,
    int? UserDepartmentId) : IRequest<LookupBundleDto>;

public sealed class GetLookupBundleQueryHandler(ILookupReadRepository lookups)
    : IRequestHandler<GetLookupBundleQuery, LookupBundleDto>
{
    public Task<LookupBundleDto> Handle(GetLookupBundleQuery request, CancellationToken cancellationToken) =>
        lookups.GetBundleAsync(request.TenantId, request.StaffRoleId, request.UserDepartmentId, cancellationToken);
}
