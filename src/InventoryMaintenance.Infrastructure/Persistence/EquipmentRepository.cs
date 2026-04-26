using InventoryMaintenance.Application.Abstractions;
using InventoryMaintenance.Domain.Equipment;
using Microsoft.EntityFrameworkCore;

namespace InventoryMaintenance.Infrastructure.Persistence;

public sealed class EquipmentRepository(AppDbContext db) : IEquipmentRepository
{
    public async Task<IReadOnlyList<EquipmentItem>> ListWithNavigationsAsync(
        int tenantId,
        int? filterByDepartmentId,
        CancellationToken cancellationToken = default)
    {
        var query = db.EquipmentItems
            .AsNoTracking()
            .Include(x => x.Department)
            .Include(x => x.EquipmentType)
            .Include(x => x.EquipmentApplication)
            .Where(x => x.TenantId == tenantId);

        if (filterByDepartmentId is { } deptId)
            query = query.Where(x => x.DepartmentId == deptId);

        return await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);
    }

    public async Task<HashSet<int>> GetEquipmentIdsWithOpenMaintenanceAsync(
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        var open = await db.MaintenanceRecords
            .AsNoTracking()
            .Where(m => m.TenantId == tenantId && m.CompletedAt == null)
            .Select(m => m.EquipmentId)
            .Distinct()
            .ToListAsync(cancellationToken);
        return open.ToHashSet();
    }

    public Task<bool> SerialNumberExistsAsync(
        int tenantId,
        string serialNumber,
        int excludeEquipmentId,
        CancellationToken cancellationToken = default) =>
        db.EquipmentItems.AnyAsync(
            x => x.TenantId == tenantId && x.SerialNumber == serialNumber && x.Id != excludeEquipmentId,
            cancellationToken);

    public Task<EquipmentItem?> GetTrackedByIdAsync(int id, int tenantId, CancellationToken cancellationToken = default) =>
        db.EquipmentItems.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId, cancellationToken);

    public void Add(EquipmentItem entity) => db.EquipmentItems.Add(entity);

    public Task<EquipmentItem> GetWithNavigationsNoTrackingAsync(int id, CancellationToken cancellationToken = default) =>
        db.EquipmentItems
            .AsNoTracking()
            .Include(x => x.Department)
            .Include(x => x.EquipmentType)
            .Include(x => x.EquipmentApplication)
            .FirstAsync(x => x.Id == id, cancellationToken);

    public Task<bool> HasOpenMaintenanceAsync(int equipmentId, CancellationToken cancellationToken = default) =>
        db.MaintenanceRecords.AnyAsync(m => m.EquipmentId == equipmentId && m.CompletedAt == null, cancellationToken);
}
