using InventoryMaintenance.Domain.Organization;
using InventoryMaintenance.Domain.Staff;

namespace InventoryMaintenance.Domain.Tenants;

public sealed class Tenant
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<StaffMember> Staff { get; set; } = new List<StaffMember>();
    public ICollection<Department> Departments { get; set; } = new List<Department>();
}
