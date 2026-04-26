using InventoryMaintenance.Application.Abstractions;
using InventoryMaintenance.Domain.Equipment;
using InventoryMaintenance.Domain.Maintenance;
using Microsoft.EntityFrameworkCore;

namespace InventoryMaintenance.Infrastructure.Persistence;

public sealed class MaintenanceRepository(AppDbContext db) : IMaintenanceRepository
{
    public Task<EquipmentItem?> GetEquipmentForTenantAsync(
        int equipmentId,
        int tenantId,
        CancellationToken cancellationToken = default) =>
        db.EquipmentItems.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == equipmentId && x.TenantId == tenantId, cancellationToken);

    public void AddRecord(MaintenanceRecord record) => db.MaintenanceRecords.Add(record);

    public void AddNote(MaintenanceNote note) => db.MaintenanceNotes.Add(note);

    public Task<MaintenanceRecord?> GetRecordWithEquipmentTrackedAsync(
        int recordId,
        int tenantId,
        CancellationToken cancellationToken = default) =>
        db.MaintenanceRecords
            .Include(x => x.Equipment)
            .FirstOrDefaultAsync(x => x.Id == recordId && x.TenantId == tenantId, cancellationToken);

    public async Task<IReadOnlyList<MaintenanceRecord>> ListRecordsForEquipmentAsync(
        int tenantId,
        int equipmentId,
        CancellationToken cancellationToken = default) =>
        await db.MaintenanceRecords
            .AsNoTracking()
            .Include(x => x.MaintenanceType)
            .Include(x => x.RequestedBy)
            .Include(x => x.Notes)
            .Where(x => x.TenantId == tenantId && x.EquipmentId == equipmentId)
            .OrderBy(x => x.RequestedAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<MaintenanceRecord>> ListOpenRecordsWithinHorizonAsync(
        DateTime horizonUtc,
        CancellationToken cancellationToken = default) =>
        await db.MaintenanceRecords
            .AsNoTracking()
            .Include(x => x.Equipment)
            .Where(x => x.CompletedAt == null
                        && x.RequestedAt < horizonUtc
                        && x.Equipment.DecommissionedOn == null)
            .ToListAsync(cancellationToken);
}
