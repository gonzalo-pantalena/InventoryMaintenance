namespace InventoryMaintenance.Worker;

public sealed class SchedulerOptions
{
    public const string SectionName = "Scheduler";

    public string ApiBaseUrl { get; set; } = "https://localhost:7223";
    public string ServiceApiKey { get; set; } = string.Empty;
    public int IntervalHours { get; set; } = 24;
    public int HorizonDays { get; set; } = 7;
}
