namespace InventoryMaintenance.Application.Contracts.Lookups;

public sealed record IdNameDto(int Id, string Name);

public sealed record LookupBundleDto(
    IReadOnlyList<IdNameDto> Departments,
    IReadOnlyList<IdNameDto> EquipmentTypes,
    IReadOnlyList<EquipmentApplicationLookupDto> EquipmentApplications,
    IReadOnlyList<IdNameDto> MaintenanceTypes);

public sealed record EquipmentApplicationLookupDto(int Id, string Name, int EquipmentTypeId);
