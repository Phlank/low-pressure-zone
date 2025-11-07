using FastEndpoints;
using FastEndpoints.Swagger;
using FluentEmail.Core.Interfaces;
using FluentEmail.Mailgun;
using LowPressureZone.Adapter.AzuraCast;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Api.Authentication;
using LowPressureZone.Api.Endpoints.Broadcasts;
using LowPressureZone.Api.Endpoints.Communities;
using LowPressureZone.Api.Endpoints.Communities.Relationships;
using LowPressureZone.Api.Endpoints.Performers;
using LowPressureZone.Api.Endpoints.Schedules;
using LowPressureZone.Api.Endpoints.Schedules.Timeslots;
using LowPressureZone.Api.Endpoints.Users.Invites;
using LowPressureZone.Api.Models.Options;
using LowPressureZone.Api.Models.Stream;
using LowPressureZone.Api.Rules;
using LowPressureZone.Api.Services;
using LowPressureZone.Api.Services.Hosted;
using LowPressureZone.Api.Services.Stream;
using LowPressureZone.Domain;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LowPressureZone.Api.Extensions;

public static class ServiceCollectionExtensions
{
    private static readonly Action<CookieAuthenticationOptions> ConfigureDevCookieOptions = options =>
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

    public static void ConfigureIdentity(this IServiceCollection services, IWebHostEnvironment environment)
    {
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
            services.Configure(IdentityConstants.TwoFactorUserIdScheme, ConfigureDevCookieOptions);
            services.Configure(IdentityConstants.TwoFactorRememberMeScheme, ConfigureDevCookieOptions);
            services.Configure(IdentityConstants.ExternalScheme, ConfigureDevCookieOptions);
            services.ConfigureApplicationCookie(ConfigureDevCookieOptions);
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

    public static void ConfigureWebApi(this IServiceCollection services)
    {
        services.AddFastEndpoints();
        services.AddHttpContextAccessor();
        services.SwaggerDocument();
        services.AddCors(options =>
        {
            options.AddPolicy("Development", builder =>
            {
                builder.WithOrigins("http://localhost:4001")
                       .AllowAnyHeader()
                       .WithMethods("GET", "PUT", "POST", "DELETE")
                       .AllowCredentials();
            });
            options.AddPolicy("Production", builder =>
            {
                builder.WithOrigins("https://lowpressurezone.com")
                       .AllowAnyHeader()
                       .WithMethods("GET", "PUT", "POST", "DELETE")
                       .AllowCredentials();
            });
        });
    }

    public static void AddApiServices(this IServiceCollection services)
    {
        AddEndpointServices(services);
        services.AddScoped<ConnectionInformationService>();

        services.AddSingleton<ISender>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<EmailServiceOptions>>();
            return new MailgunSender(options.Value.MailgunDomain, options.Value.MailgunApiKey);
        });
        services.AddSingleton<EmailService>();
        services.AddSingleton<UriService>();
        services.AddHttpClient("Icecast", (serviceProvider, httpClient) =>
        {
            var config = serviceProvider.GetRequiredService<IOptions<StreamingOptions>>();
            var icecastOptions =
                config.Value.Streams.First(stream => stream.Server == StreamServerType.Icecast).Icecast;
            httpClient.BaseAddress = icecastOptions!.IcecastUrl;
            httpClient.Timeout = TimeSpan.FromSeconds(10);
        });
        services.AddHttpClient("AzuraCast", (serviceProvider, httpClient) =>
        {
            var config = serviceProvider.GetRequiredService<IOptions<StreamingOptions>>();
            var azuraCastOptions = config.Value
                                         .Streams
                                         .First(stream => stream.Server == StreamServerType.AzuraCast)
                                         .AzuraCast;
            httpClient.BaseAddress = azuraCastOptions!.ApiUrl;
            httpClient.DefaultRequestHeaders.Add("X-API-Key", azuraCastOptions.ApiKey);
            httpClient.Timeout = TimeSpan.FromSeconds(10);
        });
        services.AddSingleton<AzuraCastClient>();
        services.AddSingleton<AzuraCastStreamStatusService>();
        services.AddSingleton<IcecastStatusService>();
        services.AddSingleton<IStreamStatusService>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<StreamingOptions>>().Value;
            var live = options.Streams.FirstOrDefault(stream => stream.Use == options.Primary);
            if (live is null) throw new InvalidOperationException("No live stream configured");
            return live.Server switch
            {
                StreamServerType.AzuraCast => serviceProvider.GetRequiredService<AzuraCastStreamStatusService>(),
                StreamServerType.Icecast => serviceProvider.GetRequiredService<IcecastStatusService>(),
                _ => throw new InvalidOperationException("Unknown streaming server type")
            };
        });

        services.AddHostedService<BroadcastDeletionService>();
    }

    private static void AddEndpointServices(IServiceCollection services)
    {
        services.AddSingleton<CommunityMapper>();
        services.AddSingleton<CommunityRelationshipMapper>();
        services.AddSingleton<ScheduleMapper>();
        services.AddSingleton<PerformerMapper>();
        services.AddSingleton<TimeslotMapper>();
        services.AddSingleton<InviteMapper>();
        services.AddSingleton<IcecastStatusMapper>();
        services.AddSingleton<BroadcastMapper>();

        services.AddSingleton<CommunityRules>();
        services.AddSingleton<CommunityRelationshipRules>();
        services.AddSingleton<ScheduleRules>();
        services.AddSingleton<PerformerRules>();
        services.AddSingleton<TimeslotRules>();
        services.AddSingleton<BroadcastRules>();
    }
}
