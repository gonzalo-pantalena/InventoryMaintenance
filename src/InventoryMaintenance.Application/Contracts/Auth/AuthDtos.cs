namespace InventoryMaintenance.Application.Contracts.Auth;

public sealed record LoginRequest(string UserName, string Password);

public sealed record RefreshRequest(string RefreshToken);

public sealed record LoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset AccessTokenExpiresAtUtc,
    DateTimeOffset RefreshTokenExpiresAtUtc,
    UserSummary User);

public sealed record UserSummary(
    int Id,
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    int TenantId,
    int? DepartmentId,
    int StaffRoleId,
    string StaffRoleName);
