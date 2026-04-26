using InventoryMaintenance.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<SchedulerOptions>(builder.Configuration.GetSection(SchedulerOptions.SectionName));
builder.Services.AddHttpClient();
builder.Services.AddHostedService<MaintenanceSchedulerWorker>();

var host = builder.Build();
host.Run();
