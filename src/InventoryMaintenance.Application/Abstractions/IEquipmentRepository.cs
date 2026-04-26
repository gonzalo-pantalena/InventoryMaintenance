using InventoryMaintenance.Domain.Equipment;

namespace InventoryMaintenance.Application.Abstractions;

public interface IEquipmentRepository
{
    Task<IReadOnlyList<EquipmentItem>> ListWithNavigationsAsync(
        int tenantId,
        int? filterByDepartmentId,
        CancellationToken cancellationToken = default);

    Task<HashSet<int>> GetEquipmentIdsWithOpenMaintenanceAsync(
        int tenantId,
        CancellationToken cancellationToken = default);

    Task<bool> SerialNumberExistsAsync(
        int tenantId,
        string serialNumber,
        int excludeEquipmentId,
        CancellationToken cancellationToken = default);

    Task<EquipmentItem?> GetTrackedByIdAsync(
        int id,
        int tenantId,
        CancellationToken cancellationToken = default);

    void Add(EquipmentItem entity);

    Task<EquipmentItem> GetWithNavigationsNoTrackingAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<bool> HasOpenMaintenanceAsync(
        int equipmentId,
        CancellationToken cancellationToken = default);
}
