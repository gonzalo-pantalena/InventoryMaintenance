using InventoryMaintenance.Domain.Auth;
using InventoryMaintenance.Domain.Organization;
using InventoryMaintenance.Domain.Tenants;

namespace InventoryMaintenance.Domain.Staff;

public sealed class StaffMember
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }
    public int StaffRoleId { get; set; }
    public StaffRole StaffRole { get; set; } = null!;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
