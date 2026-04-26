using InventoryMaintenance.Application.Abstractions;
using InventoryMaintenance.Application.Contracts.Maintenance;
using InventoryMaintenance.Domain.Maintenance;
using MediatR;

namespace InventoryMaintenance.Application.Features.Maintenance;

public sealed record UpdateMaintenanceCommand(
    int TenantId,
    int StaffRoleId,
    UpdateMaintenanceRequest Body) : IRequest<UpdateMaintenanceResult>;

public enum UpdateMaintenanceStatus
{
    Ok,
    NotFound,
    Forbidden,
}

public sealed record UpdateMaintenanceResult(
    UpdateMaintenanceStatus Status,
    IReadOnlyList<MaintenanceEventDto>? Events = null);

public sealed class UpdateMaintenanceCommandHandler(IMaintenanceRepository maintenance, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateMaintenanceCommand, UpdateMaintenanceResult>
{
    public async Task<UpdateMaintenanceResult> Handle(UpdateMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var body = request.Body;
        if (body.MarkCompleted && request.StaffRoleId != 2)
            return new UpdateMaintenanceResult(UpdateMaintenanceStatus.Forbidden);

        var record = await maintenance.GetRecordWithEquipmentTrackedAsync(body.Id, request.TenantId, cancellationToken);
        if (record is null)
            return new UpdateMaintenanceResult(UpdateMaintenanceStatus.NotFound);

        if (body.MarkCompleted)
            record.CompletedAt = DateTime.UtcNow;
        else
            record.CompletedAt = null;

        maintenance.AddNote(new MaintenanceNote
        {
            MaintenanceRecordId = record.Id,
            Body = $"{DateTime.UtcNow:O}: " + MaintenanceAccess.Sanitize(body.Description),
            CreatedAt = DateTime.UtcNow,
        });
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var rows = await maintenance.ListRecordsForEquipmentAsync(request.TenantId, record.EquipmentId, cancellationToken);
        var list = rows.Select(MaintenanceEventMapping.Map).ToList();
        return new UpdateMaintenanceResult(UpdateMaintenanceStatus.Ok, list);
    }
}
