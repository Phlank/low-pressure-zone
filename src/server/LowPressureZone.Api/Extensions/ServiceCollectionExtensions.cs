using FastEndpoints;
using FastEndpoints.Swagger;
using FluentEmail.Core.Interfaces;
using FluentEmail.Mailgun;
using LowPressureZone.Api.Endpoints.Communities;
using LowPressureZone.Api.Endpoints.Communities.Relationships;
using LowPressureZone.Api.Endpoints.Performers;
using LowPressureZone.Api.Endpoints.Schedules;
using LowPressureZone.Api.Endpoints.Schedules.Timeslots;
using LowPressureZone.Api.Rules;
using LowPressureZone.Api.Services;
using LowPressureZone.Domain;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LowPressureZone.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddDatabases(this WebApplicationBuilder builder)
    {
        var identityConnectionString = builder.Configuration.GetConnectionString("Identity");
        var dataConnectionString = builder.Configuration.GetConnectionString("Data");

        builder.Services.AddDbContext<IdentityContext>(options =>
        {
            options.UseNpgsql(identityConnectionString).EnableSensitiveDataLogging();
        });
        builder.Services.AddDbContext<DataContext>(options =>
        {
            options.UseNpgsql(dataConnectionString).EnableSensitiveDataLogging();
        });
    }

    public static void ConfigureIdentity(this IServiceCollection services)
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
        services.AddAuthentication();
        services.AddAuthorization();
        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.Name = "LowPressureZoneCookie";
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromDays(1);
            options.Cookie.SameSite = SameSiteMode.Lax;
        });
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
        services.AddSingleton<CommunityMapper>();
        services.AddSingleton<CommunityRelationshipMapper>();
        services.AddSingleton<ScheduleMapper>();
        services.AddSingleton<PerformerMapper>();
        services.AddSingleton<TimeslotMapper>();

        services.AddSingleton<CommunityRules>();
        services.AddSingleton<ScheduleRules>();
        services.AddSingleton<PerformerRules>();
        services.AddSingleton<TimeslotRules>();

        services.AddSingleton<ISender>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<EmailServiceOptions>>();
            return new MailgunSender(options.Value.MailgunDomain, options.Value.MailgunApiKey);
        });
        services.AddSingleton<EmailService>();
        services.AddSingleton<UriService>();
    }
}
