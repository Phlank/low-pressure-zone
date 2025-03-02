using FluentEmail.Core.Interfaces;
using FluentEmail.Mailgun;
using LowPressureZone.Api.Endpoints.Audiences;
using LowPressureZone.Api.Endpoints.Performers;
using LowPressureZone.Api.Endpoints.Schedules;
using LowPressureZone.Api.Endpoints.Schedules.Timeslots;
using LowPressureZone.Api.Rules;
using LowPressureZone.Api.Services;
using LowPressureZone.Domain;
using LowPressureZone.Domain.BusinessRules;
using LowPressureZone.Identity;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddDatabases(this WebApplicationBuilder builder)
    {
        var identityConnectionString = builder.Configuration.GetConnectionString("Identity");
        var dataConnectionString = builder.Configuration.GetConnectionString("Data");
        
        builder.Services.AddDbContext<IdentityContext>(options =>
        {
            options.UseNpgsql(identityConnectionString);
        });
        builder.Services.AddDbContext<DataContext>(options =>
        {
            options.UseNpgsql(dataConnectionString);
        });
    }

    public static void AddApiModelMappers(this IServiceCollection services)
    {
        services.AddSingleton<AudienceMapper>();
        services.AddSingleton<ScheduleRequestMapper>();
        services.AddSingleton<ScheduleResponseMapper>();
        services.AddSingleton<PerformerRequestMapper>();
        services.AddSingleton<PerformerResponseMapper>();
        services.AddSingleton<TimeslotRequestMapper>();
        services.AddSingleton<TimeslotResponseMapper>();
    }

    public static void AddDomainRules(this IServiceCollection services)
    {
        services.AddSingleton<AudienceRules>();
        services.AddSingleton<ScheduleRules>();
        services.AddSingleton<PerformerRules>();
        services.AddSingleton<TimeslotRules>();
    }

    public static void ConfigureApiServices(this WebApplicationBuilder builder)
    {
        var emailOptions = builder.Configuration.GetRequiredSection("Email").Get<EmailServiceOptions>()
                           ?? throw new InvalidOperationException("Cannot register email services without configuration"); ;
        builder.Services.AddFluentEmail(emailOptions.FromAddress)
                        .AddMailGunSender(emailOptions.MailgunDomain, emailOptions.MailgunApiKey);
        builder.Services.AddSingleton<EmailService>();

        builder.Services.Configure<UriServiceOptions>(builder.Configuration.GetSection(UriServiceOptions.Name));
        builder.Services.AddSingleton<UriService>();
    }
}
