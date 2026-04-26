using InventoryMaintenance.Application.Abstractions;
using InventoryMaintenance.Application.Contracts.Maintenance;
using InventoryMaintenance.Domain.Maintenance;
using MediatR;

namespace InventoryMaintenance.Application.Features.Maintenance;

public sealed record CreateMaintenanceCommand(
    int TenantId,
    int StaffRoleId,
    int? DepartmentId,
    int RequestedByStaffId,
    CreateMaintenanceRequest Body) : IRequest<IReadOnlyList<MaintenanceEventDto>?>;

public sealed class CreateMaintenanceCommandHandler(IMaintenanceRepository maintenance, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateMaintenanceCommand, IReadOnlyList<MaintenanceEventDto>?>
{
    public async Task<IReadOnlyList<MaintenanceEventDto>?> Handle(CreateMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var body = request.Body;
        var equipment = await maintenance.GetEquipmentForTenantAsync(body.EquipmentId, request.TenantId, cancellationToken);
        if (equipment is null || !MaintenanceAccess.CanAccessEquipment(equipment.DepartmentId, request.StaffRoleId, request.DepartmentId))
            return null;

        var requestedAt = body.ScheduledFor?.UtcDateTime ?? DateTime.UtcNow;
        var record = new MaintenanceRecord
        {
            TenantId = request.TenantId,
            EquipmentId = equipment.Id,
            MaintenanceTypeId = body.MaintenanceTypeId,
            RequestedAt = requestedAt,
            RequestedByStaffId = request.RequestedByStaffId,
        };
        maintenance.AddRecord(record);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var prefix = $"{DateTime.UtcNow:O}: ";
        maintenance.AddNote(new MaintenanceNote
        {
            MaintenanceRecordId = record.Id,
            Body = prefix + MaintenanceAccess.Sanitize(body.Description),
            CreatedAt = DateTime.UtcNow,
        });
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var rows = await maintenance.ListRecordsForEquipmentAsync(request.TenantId, body.EquipmentId, cancellationToken);
        return rows.Select(MaintenanceEventMapping.Map).ToList();
    }
}
