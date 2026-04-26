using InventoryMaintenance.Domain.Equipment;
using InventoryMaintenance.Domain.Staff;
using InventoryMaintenance.Domain.Tenants;

namespace InventoryMaintenance.Domain.Maintenance;

public sealed class MaintenanceRecord
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public int EquipmentId { get; set; }
    public EquipmentItem Equipment { get; set; } = null!;
    public int MaintenanceTypeId { get; set; }
    public MaintenanceType MaintenanceType { get; set; } = null!;
    public DateTime RequestedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int RequestedByStaffId { get; set; }
    public StaffMember RequestedBy { get; set; } = null!;
    public ICollection<MaintenanceNote> Notes { get; set; } = new List<MaintenanceNote>();
}
