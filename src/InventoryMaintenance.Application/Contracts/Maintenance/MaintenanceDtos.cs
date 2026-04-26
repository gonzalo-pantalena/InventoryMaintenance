namespace InventoryMaintenance.Application.Contracts.Maintenance;

public sealed record MaintenanceEventDto(
    int Id,
    int EquipmentId,
    int MaintenanceTypeId,
    string MaintenanceTypeName,
    DateTimeOffset RequestedAt,
    DateTimeOffset? CompletedAt,
    string RequestedByName,
    IReadOnlyList<string> NoteLines);

public sealed record CreateMaintenanceRequest(
    int EquipmentId,
    int MaintenanceTypeId,
    string Description,
    DateTimeOffset? ScheduledFor);

public sealed record UpdateMaintenanceRequest(
    int Id,
    string Description,
    bool MarkCompleted);
