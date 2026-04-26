namespace InventoryMaintenance.Application.Abstractions;

public interface IJwtTokenIssuer
{
    string CreateAccessToken(int staffId, string userName, int tenantId, int staffRoleId, int? departmentId);
}
