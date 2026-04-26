namespace InventoryMaintenance.Application.Contracts.Equipment;

public sealed record EquipmentDto(
    int Id,
    int DepartmentId,
    string DepartmentName,
    int EquipmentTypeId,
    string EquipmentTypeName,
    int EquipmentApplicationId,
    string EquipmentApplicationName,
    string Name,
    string Brand,
    string Model,
    int Year,
    string SerialNumber,
    string EcriCode,
    DateOnly CommissionedOn,
    DateOnly? DecommissionedOn,
    Guid PublicId,
    bool HasOpenMaintenance);

public sealed record UpsertEquipmentRequest(
    int Id,
    int DepartmentId,
    int EquipmentTypeId,
    int EquipmentApplicationId,
    string Name,
    string Brand,
    string Model,
    int Year,
    string SerialNumber,
    string EcriCode,
    DateOnly CommissionedOn,
    DateOnly? DecommissionedOn);
