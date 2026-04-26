using InventoryMaintenance.Application.Abstractions;
using InventoryMaintenance.Application.Contracts.Lookups;
using Microsoft.EntityFrameworkCore;

namespace InventoryMaintenance.Infrastructure.Persistence;

public sealed class LookupReadRepository(AppDbContext db) : ILookupReadRepository
{
    public async Task<LookupBundleDto> GetBundleAsync(
        int tenantId,
        int staffRoleId,
        int? userDepartmentId,
        CancellationToken cancellationToken = default)
    {
        var deptQuery = db.Departments.AsNoTracking().Where(x => x.TenantId == tenantId);
        if (staffRoleId < 3 && userDepartmentId is { } ud)
            deptQuery = deptQuery.Where(x => x.Id == ud);

        var departments = await deptQuery
            .OrderBy(x => x.Name)
            .Select(x => new IdNameDto(x.Id, x.Name))
            .ToListAsync(cancellationToken);

        var types = await db.EquipmentTypes.AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.Name)
            .Select(x => new IdNameDto(x.Id, x.Name))
            .ToListAsync(cancellationToken);

        var apps = await db.EquipmentApplications.AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.Name)
            .Select(x => new EquipmentApplicationLookupDto(x.Id, x.Name, x.EquipmentTypeId))
            .ToListAsync(cancellationToken);

        var maintTypes = await db.MaintenanceTypes.AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new IdNameDto(x.Id, x.Name))
            .ToListAsync(cancellationToken);

        return new LookupBundleDto(departments, types, apps, maintTypes);
    }
}
