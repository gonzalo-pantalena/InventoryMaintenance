using InventoryMaintenance.Application.Abstractions;
using InventoryMaintenance.Application.Contracts.Equipment;
using InventoryMaintenance.Domain.Equipment;
using MediatR;

namespace InventoryMaintenance.Application.Features.Equipment;

public sealed record UpsertEquipmentCommand(
    int TenantId,
    int StaffRoleId,
    UpsertEquipmentRequest Body) : IRequest<UpsertEquipmentResult>;

public enum UpsertEquipmentStatus
{
    Ok,
    NotFound,
    Conflict,
    Forbidden,
}

public sealed record UpsertEquipmentResult(UpsertEquipmentStatus Status, EquipmentDto? Data, string? Message = null);

public sealed class UpsertEquipmentCommandHandler(
    IEquipmentRepository equipment,
    IUnitOfWork unitOfWork) : IRequestHandler<UpsertEquipmentCommand, UpsertEquipmentResult>
{
    public async Task<UpsertEquipmentResult> Handle(UpsertEquipmentCommand request, CancellationToken cancellationToken)
    {
        if (request.StaffRoleId < 3)
            return new UpsertEquipmentResult(UpsertEquipmentStatus.Forbidden, null);

        var body = request.Body;
        var duplicate = await equipment.SerialNumberExistsAsync(
            request.TenantId,
            body.SerialNumber.Trim(),
            body.Id,
            cancellationToken);
        if (duplicate)
            return new UpsertEquipmentResult(UpsertEquipmentStatus.Conflict, null, "Serial number already exists for this tenant.");

        EquipmentItem entity;
        if (body.Id == 0)
        {
            entity = new EquipmentItem { TenantId = request.TenantId, PublicId = Guid.NewGuid() };
            equipment.Add(entity);
        }
        else
        {
            var existing = await equipment.GetTrackedByIdAsync(body.Id, request.TenantId, cancellationToken);
            if (existing is null)
                return new UpsertEquipmentResult(UpsertEquipmentStatus.NotFound, null);
            entity = existing;
        }

        entity.DepartmentId = body.DepartmentId;
        entity.EquipmentTypeId = body.EquipmentTypeId;
        entity.EquipmentApplicationId = body.EquipmentApplicationId;
        entity.Name = body.Name.Trim();
        entity.Brand = body.Brand.Trim();
        entity.Model = body.Model.Trim();
        entity.Year = body.Year;
        entity.SerialNumber = body.SerialNumber.Trim();
        entity.EcriCode = body.EcriCode.Trim();
        entity.CommissionedOn = body.CommissionedOn;
        entity.DecommissionedOn = body.DecommissionedOn;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var loaded = await equipment.GetWithNavigationsNoTrackingAsync(entity.Id, cancellationToken);
        var hasOpen = await equipment.HasOpenMaintenanceAsync(loaded.Id, cancellationToken);

        var dto = new EquipmentDto(
            loaded.Id,
            loaded.DepartmentId,
            loaded.Department.Name,
            loaded.EquipmentTypeId,
            loaded.EquipmentType.Name,
            loaded.EquipmentApplicationId,
            loaded.EquipmentApplication.Name,
            loaded.Name,
            loaded.Brand,
            loaded.Model,
            loaded.Year,
            loaded.SerialNumber,
            loaded.EcriCode,
            loaded.CommissionedOn,
            loaded.DecommissionedOn,
            loaded.PublicId,
            hasOpen);

        return new UpsertEquipmentResult(UpsertEquipmentStatus.Ok, dto);
    }
}
