using InventoryMaintenance.Application.Contracts.Lookups;

namespace InventoryMaintenance.Application.Abstractions;

public interface ILookupReadRepository
{
    Task<LookupBundleDto> GetBundleAsync(
        int tenantId,
        int staffRoleId,
        int? userDepartmentId,
        CancellationToken cancellationToken = default);
}
