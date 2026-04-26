using InventoryMaintenance.Domain.Tenants;

namespace InventoryMaintenance.Domain.Equipment;

public sealed class EquipmentApplication
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public int EquipmentTypeId { get; set; }
    public EquipmentType EquipmentType { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
}
