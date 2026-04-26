using System.Security.Cryptography;
using InventoryMaintenance.Application.Abstractions;
using InventoryMaintenance.Application.Contracts.Auth;
using InventoryMaintenance.Domain.Auth;
using InventoryMaintenance.Domain.Staff;
using MediatR;

namespace InventoryMaintenance.Application.Features.Auth;

public sealed record RefreshCommand(RefreshRequest Request) : IRequest<LoginResponse?>;

public sealed class RefreshCommandHandler(
    IRefreshTokenRepository refreshTokens,
    IJwtTokenIssuer tokenIssuer,
    IJwtAuthenticationSettings jwtSettings,
    IUnitOfWork unitOfWork) : IRequestHandler<RefreshCommand, LoginResponse?>
{
    public async Task<LoginResponse?> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var token = await refreshTokens.GetByTokenWithMemberRoleAsync(request.Request.RefreshToken, cancellationToken);
        if (token is null || token.RevokedAt is not null || token.ExpiresAt < DateTime.UtcNow)
            return null;

        token.RevokedAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await IssueTokensAsync(token.StaffMember, cancellationToken);
    }

    private async Task<LoginResponse> IssueTokensAsync(StaffMember user, CancellationToken cancellationToken)
    {
        var access = tokenIssuer.CreateAccessToken(user.Id, user.UserName, user.TenantId, user.StaffRoleId, user.DepartmentId);
        var refreshPlain = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
        var refreshExpires = DateTime.UtcNow.AddDays(jwtSettings.RefreshTokenDays);

        refreshTokens.Add(new RefreshToken
        {
            StaffMemberId = user.Id,
            Token = refreshPlain,
            ExpiresAt = refreshExpires,
        });

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var accessExpires = DateTimeOffset.UtcNow.AddMinutes(jwtSettings.AccessTokenMinutes);

        var summary = new UserSummary(
            user.Id,
            user.UserName,
            user.Email,
            user.FirstName,
            user.LastName,
            user.TenantId,
            user.DepartmentId,
            user.StaffRoleId,
            user.StaffRole.Name);

        return new LoginResponse(
            access,
            refreshPlain,
            accessExpires,
            new DateTimeOffset(refreshExpires, TimeSpan.Zero),
            summary);
    }
}
