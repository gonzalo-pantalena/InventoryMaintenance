using InventoryMaintenance.Application.Abstractions;
using InventoryMaintenance.Application.Contracts.Maintenance;
using MediatR;

namespace InventoryMaintenance.Application.Features.Maintenance;

public sealed record ListMaintenanceEventsQuery(
    int TenantId,
    int StaffRoleId,
    int? DepartmentId,
    int EquipmentId) : IRequest<IReadOnlyList<MaintenanceEventDto>?>;

public sealed class ListMaintenanceEventsQueryHandler(IMaintenanceRepository maintenance)
    : IRequestHandler<ListMaintenanceEventsQuery, IReadOnlyList<MaintenanceEventDto>?>
{
    public async Task<IReadOnlyList<MaintenanceEventDto>?> Handle(
        ListMaintenanceEventsQuery request,
        CancellationToken cancellationToken)
    {
        var equipment = await maintenance.GetEquipmentForTenantAsync(request.EquipmentId, request.TenantId, cancellationToken);
        if (equipment is null || !MaintenanceAccess.CanAccessEquipment(equipment.DepartmentId, request.StaffRoleId, request.DepartmentId))
            return null;

        var rows = await maintenance.ListRecordsForEquipmentAsync(request.TenantId, request.EquipmentId, cancellationToken);
        return rows.Select(MaintenanceEventMapping.Map).ToList();
    }
}
