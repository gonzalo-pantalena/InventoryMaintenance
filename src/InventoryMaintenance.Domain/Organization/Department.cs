using InventoryMaintenance.Domain.Tenants;

namespace InventoryMaintenance.Domain.Organization;

public sealed class Department
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
}
