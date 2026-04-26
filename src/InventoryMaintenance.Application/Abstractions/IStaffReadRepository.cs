using InventoryMaintenance.Domain.Staff;

namespace InventoryMaintenance.Application.Abstractions;

public interface IStaffReadRepository
{
    Task<StaffMember?> GetByUserNameWithRoleAsync(string userName, CancellationToken cancellationToken = default);

    Task<StaffMember?> GetByIdWithRoleAsync(int id, CancellationToken cancellationToken = default);
}
