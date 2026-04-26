using InventoryMaintenance.Domain.Equipment;
using InventoryMaintenance.Domain.Maintenance;

namespace InventoryMaintenance.Application.Abstractions;

public interface IMaintenanceRepository
{
    Task<EquipmentItem?> GetEquipmentForTenantAsync(
        int equipmentId,
        int tenantId,
        CancellationToken cancellationToken = default);

    void AddRecord(MaintenanceRecord record);

    void AddNote(MaintenanceNote note);

    Task<MaintenanceRecord?> GetRecordWithEquipmentTrackedAsync(
        int recordId,
        int tenantId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MaintenanceRecord>> ListRecordsForEquipmentAsync(
        int tenantId,
        int equipmentId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MaintenanceRecord>> ListOpenRecordsWithinHorizonAsync(
        DateTime horizonUtc,
        CancellationToken cancellationToken = default);
}
