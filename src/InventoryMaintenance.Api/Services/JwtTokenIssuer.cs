using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InventoryMaintenance.Api.Options;
using InventoryMaintenance.Api.Security;
using InventoryMaintenance.Application.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace InventoryMaintenance.Api.Services;

public sealed class JwtTokenIssuer(IOptions<JwtOptions> options) : IJwtTokenIssuer
{
    private readonly JwtOptions _opt = options.Value;

    public string CreateAccessToken(int staffId, string userName, int tenantId, int staffRoleId, int? departmentId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.SigningKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_opt.AccessTokenMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, staffId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, userName),
            new(JwtClaimNames.TenantId, tenantId.ToString()),
            new(JwtClaimNames.StaffRoleId, staffRoleId.ToString()),
        };
        if (departmentId is { } d)
            claims.Add(new Claim(JwtClaimNames.DepartmentId, d.ToString()));

        var token = new JwtSecurityToken(
            issuer: _opt.Issuer,
            audience: _opt.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
