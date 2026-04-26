using InventoryMaintenance.Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryMaintenance.Application.Features.Internal;

public sealed record RunMaintenanceRemindersCommand(DateTime HorizonUtc) : IRequest<RunMaintenanceRemindersResult>;

public sealed record RunMaintenanceRemindersResult(int RecordCount);

public sealed class RunMaintenanceRemindersCommandHandler(
    IMaintenanceRepository maintenance,
    ILogger<RunMaintenanceRemindersCommandHandler> logger)
    : IRequestHandler<RunMaintenanceRemindersCommand, RunMaintenanceRemindersResult>
{
    public async Task<RunMaintenanceRemindersResult> Handle(
        RunMaintenanceRemindersCommand request,
        CancellationToken cancellationToken)
    {
        var pending = await maintenance.ListOpenRecordsWithinHorizonAsync(request.HorizonUtc, cancellationToken);

        logger.LogInformation("Maintenance reminder job found {Count} open records within horizon.", pending.Count);
        foreach (var group in pending.GroupBy(x => x.TenantId))
            logger.LogInformation(
                "Tenant {TenantId}: {Count} records (email integration not configured).",
                group.Key,
                group.Count());

        return new RunMaintenanceRemindersResult(pending.Count);
    }
}
