using InventoryMaintenance.Application.Abstractions;
using InventoryMaintenance.Infrastructure.Persistence;
using InventoryMaintenance.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryMaintenance.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
            {
                sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
            }));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IEquipmentRepository, EquipmentRepository>();
        services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();
        services.AddScoped<ILookupReadRepository, LookupReadRepository>();
        services.AddScoped<IStaffReadRepository, StaffReadRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddSingleton<IPasswordVerifier, BcryptPasswordVerifier>();

        return services;
    }
}
