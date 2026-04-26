using InventoryMaintenance.Domain.Auth;
using InventoryMaintenance.Domain.Equipment;
using InventoryMaintenance.Domain.Maintenance;
using InventoryMaintenance.Domain.Organization;
using InventoryMaintenance.Domain.Staff;
using InventoryMaintenance.Domain.Tenants;
using Microsoft.EntityFrameworkCore;

namespace InventoryMaintenance.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<StaffRole> StaffRoles => Set<StaffRole>();
    public DbSet<StaffMember> StaffMembers => Set<StaffMember>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<EquipmentType> EquipmentTypes => Set<EquipmentType>();
    public DbSet<EquipmentApplication> EquipmentApplications => Set<EquipmentApplication>();
    public DbSet<EquipmentItem> EquipmentItems => Set<EquipmentItem>();
    public DbSet<MaintenanceType> MaintenanceTypes => Set<MaintenanceType>();
    public DbSet<MaintenanceRecord> MaintenanceRecords => Set<MaintenanceRecord>();
    public DbSet<MaintenanceNote> MaintenanceNotes => Set<MaintenanceNote>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>(e =>
        {
            e.ToTable("Tenants");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<StaffRole>(e =>
        {
            e.ToTable("StaffRoles");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Department>(e =>
        {
            e.ToTable("Departments");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.HasOne(x => x.Tenant).WithMany(x => x.Departments).HasForeignKey(x => x.TenantId);
        });

        modelBuilder.Entity<StaffMember>(e =>
        {
            e.ToTable("StaffMembers");
            e.HasKey(x => x.Id);
            e.Property(x => x.UserName).HasMaxLength(100).IsRequired();
            e.Property(x => x.Email).HasMaxLength(256).IsRequired();
            e.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            e.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            e.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
            e.HasIndex(x => new { x.TenantId, x.UserName }).IsUnique();
            e.HasOne(x => x.Tenant).WithMany(x => x.Staff).HasForeignKey(x => x.TenantId);
            e.HasOne(x => x.Department).WithMany().HasForeignKey(x => x.DepartmentId);
            e.HasOne(x => x.StaffRole).WithMany().HasForeignKey(x => x.StaffRoleId);
        });

        modelBuilder.Entity<EquipmentType>(e =>
        {
            e.ToTable("EquipmentTypes");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.HasIndex(x => new { x.TenantId, x.Name }).IsUnique();
            e.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId);
        });

        modelBuilder.Entity<EquipmentApplication>(e =>
        {
            e.ToTable("EquipmentApplications");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.HasIndex(x => new { x.TenantId, x.EquipmentTypeId, x.Name }).IsUnique();
            e.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.EquipmentType).WithMany().HasForeignKey(x => x.EquipmentTypeId);
        });

        modelBuilder.Entity<EquipmentItem>(e =>
        {
            e.ToTable("EquipmentItems");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Brand).HasMaxLength(100).IsRequired();
            e.Property(x => x.Model).HasMaxLength(100).IsRequired();
            e.Property(x => x.SerialNumber).HasMaxLength(100).IsRequired();
            e.Property(x => x.EcriCode).HasMaxLength(100).IsRequired();
            e.HasIndex(x => new { x.TenantId, x.SerialNumber }).IsUnique();
            e.HasIndex(x => x.PublicId).IsUnique();
            e.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Department).WithMany().HasForeignKey(x => x.DepartmentId);
            e.HasOne(x => x.EquipmentType).WithMany().HasForeignKey(x => x.EquipmentTypeId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.EquipmentApplication).WithMany().HasForeignKey(x => x.EquipmentApplicationId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<MaintenanceType>(e =>
        {
            e.ToTable("MaintenanceTypes");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<MaintenanceRecord>(e =>
        {
            e.ToTable("MaintenanceRecords");
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Equipment).WithMany(x => x.MaintenanceRecords).HasForeignKey(x => x.EquipmentId);
            e.HasOne(x => x.MaintenanceType).WithMany().HasForeignKey(x => x.MaintenanceTypeId);
            e.HasOne(x => x.RequestedBy).WithMany().HasForeignKey(x => x.RequestedByStaffId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<MaintenanceNote>(e =>
        {
            e.ToTable("MaintenanceNotes");
            e.HasKey(x => x.Id);
            e.Property(x => x.Body).HasMaxLength(4000).IsRequired();
            e.HasOne(x => x.MaintenanceRecord).WithMany(x => x.Notes).HasForeignKey(x => x.MaintenanceRecordId);
        });

        modelBuilder.Entity<RefreshToken>(e =>
        {
            e.ToTable("RefreshTokens");
            e.HasKey(x => x.Id);
            e.Property(x => x.Token).HasMaxLength(200).IsRequired();
            e.HasIndex(x => x.Token).IsUnique();
            e.HasOne(x => x.StaffMember).WithMany(x => x.RefreshTokens).HasForeignKey(x => x.StaffMemberId);
        });
    }
}
