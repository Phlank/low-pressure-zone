﻿using LowPressureZone.Api.Endpoints.Audiences;
using LowPressureZone.Api.Endpoints.Performers;
using LowPressureZone.Api.Endpoints.Schedules;
using LowPressureZone.Api.Endpoints.Schedules.Timeslots;
using LowPressureZone.Domain;
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

    public static void AddMappers(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<AudienceRequestMapper>();
        builder.Services.AddSingleton<AudienceResponseMapper>();
        builder.Services.AddSingleton<ScheduleRequestMapper>();
        builder.Services.AddSingleton<ScheduleResponseMapper>();
        builder.Services.AddSingleton<PerformerRequestMapper>();
        builder.Services.AddSingleton<PerformerResponseMapper>();
        builder.Services.AddSingleton<TimeslotRequestMapper>();
        builder.Services.AddSingleton<TimeslotResponseMapper>();
    }
}
