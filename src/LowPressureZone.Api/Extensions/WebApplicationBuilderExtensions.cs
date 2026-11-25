using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using FastEndpoints;
using FastEndpoints.OpenApi;
using JasperFx.CodeGeneration;
using JasperFx.CodeGeneration.Model;
using LowPressureZone.Adapter.AzuraCast;
using LowPressureZone.Adapter.AzuraCast.Exceptions;
using LowPressureZone.Adapter.AzuraCast.Extensions;
using LowPressureZone.Api.Auth;
using LowPressureZone.Api.Services.AzuraCast;
using LowPressureZone.Data;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Application;
using LowPressureZone.Identity.Entities;
using LowPressureZone.Infrastructure;
using LowPressureZone.Infrastructure.Configuration;
using LowPressureZone.Infrastructure.Configuration.Streaming;
using LowPressureZone.Infrastructure.Emails;
using LowPressureZone.Infrastructure.Extensions;
using LowPressureZone.Scheduling.Application;
using LowPressureZone.Scheduling.Domain.BroadcastAggregate.Events;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.ErrorHandling;
using Wolverine.Postgresql;
using NightlyTaskService = LowPressureZone.Api.Services.NightlyTasks.NightlyTaskService;

namespace LowPressureZone.Api.Extensions;

public static class WebApplicationBuilderExtensions
{
    private static readonly Action<CookieAuthenticationOptions> ConfigureDevelopmentCookieOptions = options =>
    {
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.HttpOnly = true;
    };

    public static void ConfigureWebApi(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        services.Configure<StreamingConfiguration>(builder.Configuration.GetSection(StreamingConfiguration.Name));
        services.Configure<EmailServiceConfiguration>(builder.Configuration.GetSection(EmailServiceConfiguration.Name));
        services.Configure<UrlConfiguration>(builder.Configuration.GetSection(UrlConfiguration.Name));
        services.Configure<AzuraCastInstallationConfiguration>(builder.Configuration
                                                                      .GetSection(AzuraCastInstallationConfiguration
                                                                                      .Name));
        
        services.Configure<AWSOptions>(options =>
        {
            options.Credentials = new BasicAWSCredentials(builder.Configuration["AwsConfiguration:AccessKey"]!,
                                                          builder.Configuration["AwsConfiguration:SecretKey"]!);
            options.Region = RegionEndpoint.GetBySystemName(builder.Configuration["AwsConfiguration:Region"]!);
        });
        
        services.AddCors(options =>
        {
            options.AddPolicy("Frontend",
                              policyBuilder =>
                              {
                                  var siteUrl = builder.Configuration.GetValue<string>("Url:SiteUrl");
                                  policyBuilder.WithOrigins(siteUrl!)
                                               .AllowAnyHeader()
                                               .WithMethods("GET", "PUT", "POST", "DELETE")
                                               .AllowCredentials();
                              });
        });
    }

    public static void AddApiServices(this WebApplicationBuilder builder)
    {
        var dataConnectionString =
            builder.Configuration.GetConnectionString("Data") ?? throw new InvalidOperationException();
        builder.AddAzuraCast();
        builder.AddDatabases();
        builder.AddQuartz();
        builder.ConfigureAndAddIdentity();
        builder.AddFluentEmail();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddFastEndpoints(options =>
        {
            options.Assemblies = new[]
            {
                Assembly.Load("LowPressureZone.Api"),
                Assembly.Load("LowPressureZone.Scheduling.Application"),
                Assembly.Load("LowPressureZone.Identity.Application")
            };
        });
        builder.Services.AddWolverine(opts =>
        {
            opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Dynamic;
            opts.Discovery.CustomizeHandlerDiscovery(scanner => scanner.Excludes.WithNameSuffix("Handler"));
            opts.Discovery.IncludeAssembly(Assembly.Load("LowPressureZone.Scheduling.Application"));
            opts.Discovery.IncludeAssembly(Assembly.Load("LowPressureZone.Identity.Application"));
            opts.ServiceLocationPolicy = ServiceLocationPolicy.AlwaysAllowed;

            opts.PersistMessagesWithPostgresql(dataConnectionString);
            opts.UseEntityFrameworkCoreTransactions();

            opts.Policies.OnException<IntegrationException>()
                .RetryWithCooldown([.. Enumerable.Repeat(TimeSpan.FromSeconds(10), 25)]).Then
                .SendAdminEmail().Then
                .MoveToErrorQueue();
            opts.Policies.OnException<AzuraCastStateException>()
                .RetryWithCooldown([.. Enumerable.Repeat(TimeSpan.FromSeconds(10), 25)]).Then
                .SendAdminEmail().Then
                .MoveToErrorQueue();

            opts.Policies.ForMessagesOfType<BroadcastArchived>().DelayedFor(TimeSpan.FromMinutes(2));
        });
        builder.Services.AddDefaultAWSOptions((provider) => provider.GetRequiredService<IOptions<AWSOptions>>().Value);
        builder.Services.AddAWSService<IAmazonS3>();
        builder.Services.OpenApiDocument();
        builder.Services.AddHostedService<NightlyTaskService>();
        builder.Services.AddHostedService<BroadcastSyncService>();
        builder.Services.RegisterServicesFromLowPressureZoneApi();
        builder.Services.RegisterServicesFromLowPressureZoneIdentityApplication();
        builder.Services.RegisterServicesFromLowPressureZoneSchedulingApplication();
        builder.Services.RegisterServicesFromLowPressureZoneInfrastructure();
        builder.Services.RegisterServicesFromLowPressureZoneAdapterAzuraCast();
    }

    private static void AddDatabases(this WebApplicationBuilder builder)
    {
        var identityConnectionString = builder.Configuration.GetConnectionString("Identity");
        var dataConnectionString = builder.Configuration.GetConnectionString("Data");

        builder.Services.AddDbContext<IdentityContext>(options =>
        {
            options.UseNpgsql(identityConnectionString);
            if (builder.Environment.IsDevelopment()) options.EnableSensitiveDataLogging();
        }).AddDataProtection().PersistKeysToDbContext<IdentityContext>();
        builder.Services.AddDbContext<DataContext>(options =>
                                                   {
                                                       options.UseNpgsql(dataConnectionString);
                                                       if (builder.Environment.IsDevelopment())
                                                           options.EnableSensitiveDataLogging();
                                                   },
                                                   optionsLifetime: ServiceLifetime.Singleton);
    }

    private static void AddQuartz(this WebApplicationBuilder builder)
    {
        builder.Services.AddQuartz(q =>
        {
            if (builder.Environment.IsEnvironment("Testing"))
            {
                q.UseInMemoryStore();
            }
            else
            {
                q.UsePersistentStore(store =>
                {
                    store.UsePostgres(pg => { pg.ConnectionStringName = "Data"; });
                    store.ProvisionSchema();
                });
            }
        });
        builder.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        builder.Services.AddQuartzDashboard();
    }

    private static void ConfigureAndAddIdentity(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var environment = builder.Environment;

        services.AddIdentity<AppUser, AppRole>(options =>
        {
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
        }).AddEntityFrameworkStores<IdentityContext>().AddDefaultTokenProviders();
        if (environment.IsDevelopment())
        {
            // Also configure Auth cookies separately, so SameSite works cross-domain locally in Chromium
            services.Configure(IdentityConstants.TwoFactorUserIdScheme, ConfigureDevelopmentCookieOptions);
            services.Configure(IdentityConstants.TwoFactorRememberMeScheme, ConfigureDevelopmentCookieOptions);
            services.Configure(IdentityConstants.ExternalScheme, ConfigureDevelopmentCookieOptions);
            services.ConfigureApplicationCookie(ConfigureDevelopmentCookieOptions);
        }
        else
        {
            services.ConfigureApplicationCookie(options => { options.Cookie.SameSite = SameSiteMode.Lax; });
        }

        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.Name = "LowPressureZoneCookie";
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromDays(1);
            options.LoginPath = "/api/users/login";
            options.ReturnUrlParameter = "/";

            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            };
        });
        services.AddAuthentication();
        services.AddAuthorization();
        services.AddTransient<IClaimsTransformation, AppUserClaimsTransformation>();
    }

    public static void ConfigureKestrel(this WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel(options => { options.Limits.MaxRequestBodySize = 1024 * 1024 * 1024; });
        builder.Services.Configure<FormOptions>(options => { options.MultipartBodyLengthLimit = 1024 * 1024 * 1024; });
    }

    private static void AddFluentEmail(this WebApplicationBuilder builder)
    {
        var mailpitConnectionString = builder.Configuration.GetConnectionString("mailpit");
        if (!string.IsNullOrEmpty(mailpitConnectionString))
        {
            // Format is Endpoint={SmtpUrl}
            var uriPart = mailpitConnectionString.Split('=')[1];
            Uri url = new(uriPart);
            builder.Services
                   .AddFluentEmail("noreply@lowpressurezone.com", "Low Pressure Zone")
                   .AddSmtpSender(url.Host, url.Port);
        }
        else
        {
            var mailgunApiConfigKey =
                $"{EmailServiceConfiguration.Name}:{nameof(EmailServiceConfiguration.MailgunApiKey)}";
            var mailgunDomainConfigKey =
                $"{EmailServiceConfiguration.Name}:{nameof(EmailServiceConfiguration.MailgunDomain)}";
            var mailgunApiKey = builder.Configuration.GetValue<string>(mailgunApiConfigKey);
            var mailgunDomain = builder.Configuration.GetValue<string>(mailgunDomainConfigKey);
            builder.Services
                   .AddFluentEmail("noreply@lowpressurezone.com", "Low Pressure Zone")
                   .AddMailGunSender(mailgunDomain, mailgunApiKey);
        }
    }

    private static void AddAws(this WebApplicationBuilder builder)
    {
        var awsConfig = builder.Configuration.GetSection("AwsConfiguration").Get<AwsConfiguration>() ??
                        throw new InvalidOperationException();
        var credentials = new BasicAWSCredentials(awsConfig.AccessKey, awsConfig.SecretKey);
        AmazonS3Config config = string.IsNullOrEmpty(awsConfig.Endpoint)
                                    ? new() { RegionEndpoint = RegionEndpoint.GetBySystemName(awsConfig.Region) }
                                    : new() { ServiceURL = awsConfig.Endpoint, ForcePathStyle = true };
        builder.Services.AddSingleton<IAmazonS3>(_ => new AmazonS3Client(credentials, config));
    }
}