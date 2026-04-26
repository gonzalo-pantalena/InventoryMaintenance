using InventoryMaintenance.Application.Abstractions;

namespace InventoryMaintenance.Infrastructure.Security;

public sealed class BcryptPasswordVerifier : IPasswordVerifier
{
    public bool Verify(string password, string passwordHash) =>
        BCrypt.Net.BCrypt.Verify(password, passwordHash);
}
