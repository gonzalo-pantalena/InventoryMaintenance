namespace InventoryMaintenance.Api.Options;

public sealed class IntegrationOptions
{
    public const string SectionName = "Integration";

    public string ServiceApiKey { get; set; } = string.Empty;
}
