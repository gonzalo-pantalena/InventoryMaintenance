namespace InventoryMaintenance.Application.Abstractions;

public interface IPasswordVerifier
{
    bool Verify(string password, string passwordHash);
}
