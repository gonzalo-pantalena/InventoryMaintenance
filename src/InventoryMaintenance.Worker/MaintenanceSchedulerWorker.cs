using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

namespace InventoryMaintenance.Worker;

public sealed class MaintenanceSchedulerWorker(
    IHttpClientFactory httpClientFactory,
    IOptions<SchedulerOptions> options,
    ILogger<MaintenanceSchedulerWorker> logger) : BackgroundService
{
    private readonly SchedulerOptions _opt = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromHours(Math.Max(1, _opt.IntervalHours)));

        await InvokeJobAsync(stoppingToken);

        while (await timer.WaitForNextTickAsync(stoppingToken))
            await InvokeJobAsync(stoppingToken);
    }

    private async Task InvokeJobAsync(CancellationToken cancellationToken)
    {
        try
        {
            var client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_opt.ApiBaseUrl.TrimEnd('/') + "/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-Api-Key", _opt.ServiceApiKey);

            var url = $"api/internal/reminders/run?days={_opt.HorizonDays}";
            var response = await client.PostAsync(url, content: null, cancellationToken);
            logger.LogInformation("Reminder job finished with status {StatusCode}.", response.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Reminder job failed.");
        }
    }
}
