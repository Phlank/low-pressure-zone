using LowPressureZone.Aspire.Migrations;
using LowPressureZone.Aspire.ServiceDefaults;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Extensions;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder.Services
       .AddOpenTelemetry()
       .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddNpgsqlDbContext<DataContext>("Data", configureDbContextOptions: config => config.ConfigureDomainSeeding());
builder.AddNpgsqlDbContext<IdentityContext>("Identity",
                                            configureDbContextOptions: config => config.ConfigureIdentitySeeding());

var host = builder.Build();
host.Run();