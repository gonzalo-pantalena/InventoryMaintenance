using InventoryMaintenance.Api.Options;
using InventoryMaintenance.Application.Abstractions;
using Microsoft.Extensions.Options;

namespace InventoryMaintenance.Api.Services;

internal sealed class JwtAuthenticationSettingsAdapter(IOptions<JwtOptions> options) : IJwtAuthenticationSettings
{
    private readonly JwtOptions _opt = options.Value;

    public int AccessTokenMinutes => _opt.AccessTokenMinutes;

    public int RefreshTokenDays => _opt.RefreshTokenDays;
}
