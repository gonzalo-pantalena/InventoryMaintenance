using InventoryMaintenance.Application.Abstractions;
using InventoryMaintenance.Domain.Auth;
using Microsoft.EntityFrameworkCore;

namespace InventoryMaintenance.Infrastructure.Persistence;

public sealed class RefreshTokenRepository(AppDbContext db) : IRefreshTokenRepository
{
    public Task<RefreshToken?> GetByTokenWithMemberRoleAsync(string token, CancellationToken cancellationToken = default) =>
        db.RefreshTokens
            .Include(x => x.StaffMember)
            .ThenInclude(x => x.StaffRole)
            .FirstOrDefaultAsync(x => x.Token == token, cancellationToken);

    public void Add(RefreshToken entity) => db.RefreshTokens.Add(entity);
}
