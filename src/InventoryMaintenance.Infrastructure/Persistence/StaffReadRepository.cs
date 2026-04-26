using InventoryMaintenance.Application.Abstractions;
using InventoryMaintenance.Domain.Staff;
using Microsoft.EntityFrameworkCore;

namespace InventoryMaintenance.Infrastructure.Persistence;

public sealed class StaffReadRepository(AppDbContext db) : IStaffReadRepository
{
    public Task<StaffMember?> GetByUserNameWithRoleAsync(string userName, CancellationToken cancellationToken = default) =>
        db.StaffMembers
            .Include(x => x.StaffRole)
            .FirstOrDefaultAsync(x => x.UserName == userName, cancellationToken);

    public Task<StaffMember?> GetByIdWithRoleAsync(int id, CancellationToken cancellationToken = default) =>
        db.StaffMembers
            .Include(x => x.StaffRole)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
}
