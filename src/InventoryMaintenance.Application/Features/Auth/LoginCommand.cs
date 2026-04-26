using System.Security.Cryptography;
using InventoryMaintenance.Application.Abstractions;
using InventoryMaintenance.Application.Contracts.Auth;
using InventoryMaintenance.Domain.Auth;
using InventoryMaintenance.Domain.Staff;
using MediatR;

namespace InventoryMaintenance.Application.Features.Auth;

public sealed record LoginCommand(LoginRequest Request) : IRequest<LoginResponse?>;

public sealed class LoginCommandHandler(
    IStaffReadRepository staff,
    IRefreshTokenRepository refreshTokens,
    IJwtTokenIssuer tokenIssuer,
    IJwtAuthenticationSettings jwtSettings,
    IPasswordVerifier passwordVerifier,
    IUnitOfWork unitOfWork) : IRequestHandler<LoginCommand, LoginResponse?>
{
    public async Task<LoginResponse?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await staff.GetByUserNameWithRoleAsync(request.Request.UserName, cancellationToken);
        if (user is null || !passwordVerifier.Verify(request.Request.Password, user.PasswordHash))
            return null;

        return await IssueTokensAsync(user, cancellationToken);
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
