using System.Text.Json;
using System.Text.Json.Serialization;
using FastEndpoints;
using FastEndpoints.Swagger;
using FluentEmail.Core.Interfaces;
using FluentEmail.Mailgun;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Adapter.AzuraCast.Configuration;
using LowPressureZone.Api.Authentication;
using LowPressureZone.Api.Endpoints.Broadcasts;
using LowPressureZone.Api.Endpoints.Communities;
using LowPressureZone.Api.Endpoints.Communities.Relationships;
using LowPressureZone.Api.Endpoints.Performers;
using LowPressureZone.Api.Endpoints.Schedules;
using LowPressureZone.Api.Endpoints.Schedules.Timeslots;
using LowPressureZone.Api.Endpoints.Users.Invites;
using LowPressureZone.Api.Models.Configuration;
using LowPressureZone.Api.Models.Configuration.Streaming;
using LowPressureZone.Api.Rules;
using LowPressureZone.Api.Services;
using LowPressureZone.Api.Services.StreamConnectionInfo;
using LowPressureZone.Api.Services.StreamStatus;
using LowPressureZone.Domain;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using FormFileSaver = LowPressureZone.Api.Services.FormFileSaver;
using MediaAnalyzer = LowPressureZone.Api.Services.MediaAnalyzer;

namespace LowPressureZone.Api.Extensions;

public static class WebApplicationBuilderExtensions
{
    private static readonly Action<CookieAuthenticationOptions> ConfigureDevelopmentCookieOptions = options =>
    {
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.HttpOnly = true;
    };

    public static void AddDatabases(this WebApplicationBuilder builder)
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
            if (builder.Environment.IsDevelopment()) options.EnableSensitiveDataLogging();
        });
    }

    public static void ConfigureIdentity(this WebApplicationBuilder builder)
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

    public static void ConfigureWebApi(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        services.Configure<AzuraCastClientConfiguration>(builder.Configuration.GetSection(AzuraCastClientConfiguration.Name));
        services.Configure<IcecastConfiguration>(builder.Configuration.GetSection(IcecastConfiguration.Name));
        services.Configure<StreamingConfiguration>(builder.Configuration.GetSection(StreamingConfiguration.Name));
        services.Configure<EmailServiceConfiguration>(builder.Configuration.GetSection(EmailServiceConfiguration.Name));
        services.Configure<UrlConfiguration>(builder.Configuration.GetSection(UrlConfiguration.Name));
        services.Configure<FileConfiguration>(builder.Configuration.GetSection(FileConfiguration.Name));

        services.AddFastEndpoints();
        services.AddHttpContextAccessor();
        services.SwaggerDocument();
        services.AddCors(options =>
        {
            options.AddPolicy("Frontend", policyBuilder =>
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
        builder.AddEndpointServices();

        builder.Services.AddSingleton<FormFileSaver>();
        builder.Services.AddSingleton<MediaAnalyzer>();
        builder.Services.AddSingleton<TimeslotFileProcessor>();
        builder.Services.AddSingleton<EmailService>();
        builder.Services.AddSingleton<UriService>();
        builder.Services.AddHttpClient<AzuraCastClient>(ConfigureAzuraCastHttpClient);
        builder.Services.AddSingleton<IAzuraCastClient, AzuraCastClient>();
        builder.Services.AddSingleton<IStreamStatusService, AzuraCastStatusService>();
        builder.Services.AddScoped<StreamingInfoService>();
        builder.Services.AddSingleton<ISender, MailgunSender>(serviceProvider => serviceProvider.CreateMailgunSender());
        builder.Services.AddHostedService<BroadcastDeletionService>();
    }

    private static void AddEndpointServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<CommunityMapper>();
        builder.Services.AddSingleton<CommunityRelationshipMapper>();
        builder.Services.AddSingleton<ScheduleMapper>();
        builder.Services.AddSingleton<PerformerMapper>();
        builder.Services.AddSingleton<TimeslotMapper>();
        builder.Services.AddSingleton<InviteMapper>();
        builder.Services.AddSingleton<BroadcastMapper>();

        builder.Services.AddSingleton<CommunityRules>();
        builder.Services.AddSingleton<CommunityRelationshipRules>();
        builder.Services.AddSingleton<ScheduleRules>();
        builder.Services.AddSingleton<PerformerRules>();
        builder.Services.AddSingleton<TimeslotRules>();
        builder.Services.AddSingleton<BroadcastRules>();
    }

    private static MailgunSender CreateMailgunSender(this IServiceProvider services)
    {
        var options = services.GetRequiredService<IOptions<EmailServiceConfiguration>>();
        return new MailgunSender(options.Value.MailgunDomain, options.Value.MailgunApiKey);
    }

    private static readonly Action<IServiceProvider, HttpClient> ConfigureAzuraCastHttpClient =
        (services, client) =>
        {
            var configuration = services.GetRequiredService<IOptions<AzuraCastClientConfiguration>>()
                                        .Value;
            client.BaseAddress = configuration.ApiUrl;
            client.DefaultRequestHeaders.Add("X-API-Key", configuration.ApiKey);
        };

    public static void CreateFileLocations(this WebApplicationBuilder builder)
    {
        const string tempLocationKey = $"{FileConfiguration.Name}:{nameof(FileConfiguration.TemporaryLocation)}";
        var temporaryFilePath = builder.Configuration.GetValue<string>(tempLocationKey);

        if (temporaryFilePath is null)
            throw new InvalidOperationException("Temporary file path is not configured.");

        if (!Directory.Exists(temporaryFilePath))
        {
            Directory.CreateDirectory(temporaryFilePath);
        }
    }
}