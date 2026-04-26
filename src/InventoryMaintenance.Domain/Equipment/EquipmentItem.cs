using InventoryMaintenance.Domain.Organization;
using InventoryMaintenance.Domain.Tenants;

namespace InventoryMaintenance.Domain.Equipment;

public sealed class EquipmentItem
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public int DepartmentId { get; set; }
    public Department Department { get; set; } = null!;
    public int EquipmentTypeId { get; set; }
    public EquipmentType EquipmentType { get; set; } = null!;
    public int EquipmentApplicationId { get; set; }
    public EquipmentApplication EquipmentApplication { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public string EcriCode { get; set; } = string.Empty;
    public DateOnly CommissionedOn { get; set; }
    public DateOnly? DecommissionedOn { get; set; }
    public Guid PublicId { get; set; } = Guid.NewGuid();
    public ICollection<Maintenance.MaintenanceRecord> MaintenanceRecords { get; set; } =
        new List<Maintenance.MaintenanceRecord>();
}
