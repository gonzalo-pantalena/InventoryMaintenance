namespace InventoryMaintenance.Application.Abstractions;

public interface IJwtAuthenticationSettings
{
    int AccessTokenMinutes { get; }

    int RefreshTokenDays { get; }
}
