using System.Text.Json;
using System.Text.Json.Serialization;
using FastEndpoints;
using FastEndpoints.Swagger;
using FluentEmail.Core.Interfaces;
using FluentEmail.Mailgun;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Adapter.AzuraCast.Options;
using LowPressureZone.Api.Authentication;
using LowPressureZone.Api.Endpoints.Broadcasts;
using LowPressureZone.Api.Endpoints.Communities;
using LowPressureZone.Api.Endpoints.Communities.Relationships;
using LowPressureZone.Api.Endpoints.Performers;
using LowPressureZone.Api.Endpoints.Schedules;
using LowPressureZone.Api.Endpoints.Schedules.Timeslots;
using LowPressureZone.Api.Endpoints.Users.Invites;
using LowPressureZone.Api.Models.Options;
using LowPressureZone.Api.Rules;
using LowPressureZone.Api.Services;
using LowPressureZone.Api.Services.Hosted;
using LowPressureZone.Api.Services.Stream;
using LowPressureZone.Api.Services.StreamingInfo;
using LowPressureZone.Domain;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

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

    public static void ConfigureWebApi(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        services.Configure<AzuraCastOptions>(builder.Configuration.GetSection(AzuraCastOptions.Name));
        services.Configure<IcecastOptions>(builder.Configuration.GetSection(IcecastOptions.Name));
        services.Configure<StreamingOptions>(builder.Configuration.GetSection(StreamingOptions.Name));
        services.Configure<EmailServiceOptions>(builder.Configuration.GetSection(EmailServiceOptions.Name));
        services.Configure<UrlOptions>(builder.Configuration.GetSection(UrlOptions.Name));

        services.AddFastEndpoints();
        services.AddHttpContextAccessor();
        services.SwaggerDocument();
        services.AddCors(options =>
        {
            options.AddPolicy("Frontend",
                              policyBuilder =>
                              {
                                  policyBuilder
                                      .WithOrigins(configuration.GetRequiredSection(UrlOptions.Name)["SiteUrl"]!)
                                      .AllowAnyHeader()
                                      .WithMethods("GET", "PUT", "POST", "DELETE")
                                      .AllowCredentials();
                              });
        });
    }

    public static void AddApiServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        AddEndpointServices(services);
        services.AddScoped<StreamingInfoService>();

        services.AddSingleton<ISender>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<EmailServiceOptions>>();
            return new MailgunSender(options.Value.MailgunDomain, options.Value.MailgunApiKey);
        });
        services.AddSingleton<EmailService>();
        services.AddSingleton<UriService>();
        services.AddSingleton<IAzuraCastClient, AzuraCastClient>();
        services.AddSingleton<AzuraCastStatusService>();
        services.AddSingleton<IcecastStatusService>();
        services.AddSingleton<IStreamStatusService, AzuraCastStatusService>();

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