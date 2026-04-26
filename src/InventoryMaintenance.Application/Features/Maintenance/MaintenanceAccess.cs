namespace InventoryMaintenance.Application.Features.Maintenance;

internal static class MaintenanceAccess
{
    public static bool CanAccessEquipment(int equipmentDepartmentId, int roleId, int? userDepartmentId) =>
        roleId >= 3 || userDepartmentId is null || equipmentDepartmentId == userDepartmentId;

    public static string Sanitize(string text) =>
        text.ReplaceLineEndings(" ").Trim();
}
