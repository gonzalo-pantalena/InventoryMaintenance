using InventoryMaintenance.Application.Contracts.Maintenance;
using InventoryMaintenance.Domain.Maintenance;

namespace InventoryMaintenance.Application.Features.Maintenance;

internal static class MaintenanceEventMapping
{
    public static MaintenanceEventDto Map(MaintenanceRecord x)
    {
        var name = $"{x.RequestedBy.FirstName} {x.RequestedBy.LastName}".Trim();
        var lines = x.Notes.OrderBy(n => n.CreatedAt).Select(n => n.Body).ToList();
        return new MaintenanceEventDto(
            x.Id,
            x.EquipmentId,
            x.MaintenanceTypeId,
            x.MaintenanceType.Name,
            new DateTimeOffset(DateTime.SpecifyKind(x.RequestedAt, DateTimeKind.Utc)),
            x.CompletedAt is { } c ? new DateTimeOffset(DateTime.SpecifyKind(c, DateTimeKind.Utc)) : null,
            name,
            lines);
    }
}
