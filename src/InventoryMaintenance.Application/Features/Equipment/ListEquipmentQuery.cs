using InventoryMaintenance.Application.Abstractions;
using InventoryMaintenance.Application.Contracts.Equipment;
using MediatR;

namespace InventoryMaintenance.Application.Features.Equipment;

public sealed record ListEquipmentQuery(
    int TenantId,
    int StaffRoleId,
    int? DepartmentId) : IRequest<IReadOnlyList<EquipmentDto>>;

public sealed class ListEquipmentQueryHandler(IEquipmentRepository equipment)
    : IRequestHandler<ListEquipmentQuery, IReadOnlyList<EquipmentDto>>
{
    public async Task<IReadOnlyList<EquipmentDto>> Handle(ListEquipmentQuery request, CancellationToken cancellationToken)
    {
        int? deptFilter = request.StaffRoleId < 3 ? request.DepartmentId : null;
        var list = await equipment.ListWithNavigationsAsync(request.TenantId, deptFilter, cancellationToken);
        var openSet = await equipment.GetEquipmentIdsWithOpenMaintenanceAsync(request.TenantId, cancellationToken);

        return list.Select(e => new EquipmentDto(
            e.Id,
            e.DepartmentId,
            e.Department.Name,
            e.EquipmentTypeId,
            e.EquipmentType.Name,
            e.EquipmentApplicationId,
            e.EquipmentApplication.Name,
            e.Name,
            e.Brand,
            e.Model,
            e.Year,
            e.SerialNumber,
            e.EcriCode,
            e.CommissionedOn,
            e.DecommissionedOn,
            e.PublicId,
            openSet.Contains(e.Id))).ToList();
    }
}
