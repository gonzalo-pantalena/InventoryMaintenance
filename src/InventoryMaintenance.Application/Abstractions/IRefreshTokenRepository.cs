using InventoryMaintenance.Domain.Auth;

namespace InventoryMaintenance.Application.Abstractions;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenWithMemberRoleAsync(string token, CancellationToken cancellationToken = default);

    void Add(RefreshToken entity);
}
