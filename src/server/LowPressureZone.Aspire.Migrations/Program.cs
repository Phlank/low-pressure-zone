using LowPressureZone.Aspire.Migrations;
using LowPressureZone.Domain;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder.Services
       .AddOpenTelemetry()
       .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddNpgsqlDbContext<DataContext>("Data");
builder.AddNpgsqlDbContext<IdentityContext>("Identity", configureDbContextOptions: config => config.ConfigureSeeding());

var host = builder.Build();
host.Run();