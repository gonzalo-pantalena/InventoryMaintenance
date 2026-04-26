using InventoryMaintenance.Domain.Equipment;
using InventoryMaintenance.Domain.Maintenance;
using InventoryMaintenance.Domain.Organization;
using InventoryMaintenance.Domain.Staff;
using InventoryMaintenance.Domain.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InventoryMaintenance.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task EnsureSeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("SeedData");
        //await context.Database.MigrateAsync(cancellationToken);

            var standard = new StaffRole { Name = "Standard" };
            var technician = new StaffRole { Name = "Technician" };
            var administrator = new StaffRole { Name = "Administrator" };
            context.StaffRoles.AddRange(standard, technician, administrator);
            await context.SaveChangesAsync(cancellationToken);

            context.MaintenanceTypes.AddRange(
                new MaintenanceType { Name = "Corrective" },
                new MaintenanceType { Name = "Preventive" },
                new MaintenanceType { Name = "Predictive" });
            await context.SaveChangesAsync(cancellationToken);

            var tenant = new Tenant { Name = "Demo Institution" };
            context.Tenants.Add(tenant);
            await context.SaveChangesAsync(cancellationToken);

            var dept = new Department { TenantId = tenant.Id, Name = "General Services" };
            context.Departments.Add(dept);
            await context.SaveChangesAsync(cancellationToken);

            var admin = new StaffMember
            {
                TenantId = tenant.Id,
                DepartmentId = dept.Id,
                StaffRoleId = administrator.Id,
                UserName = "admin",
                Email = "admin@example.local",
                FirstName = "System",
                LastName = "Administrator",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            };
            context.StaffMembers.Add(admin);
            await context.SaveChangesAsync(cancellationToken);
        

        await EnsureEquipmentCatalogAsync(context, logger, cancellationToken);
    }

    private static async Task EnsureEquipmentCatalogAsync(
        AppDbContext context,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var tenantId = await context.Tenants.AsNoTracking().OrderBy(t => t.Id).Select(t => t.Id).FirstOrDefaultAsync(cancellationToken);
        if (tenantId == 0)
            return;

        if (await context.EquipmentTypes.AnyAsync(x => x.TenantId == tenantId, cancellationToken))
            return;

        var type = new EquipmentType { TenantId = tenantId, Name = "General" };
        context.EquipmentTypes.Add(type);
        await context.SaveChangesAsync(cancellationToken);

        context.EquipmentApplications.Add(new EquipmentApplication
        {
            TenantId = tenantId,
            EquipmentTypeId = type.Id,
            Name = "General use",
        });
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Seeded default equipment catalog for tenant {TenantId}.", tenantId);
    }
}
