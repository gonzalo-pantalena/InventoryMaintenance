namespace InventoryMaintenance.Domain.Maintenance;

public sealed class MaintenanceNote
{
    public int Id { get; set; }
    public int MaintenanceRecordId { get; set; }
    public MaintenanceRecord MaintenanceRecord { get; set; } = null!;
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
