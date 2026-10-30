using System.Text.Json;
using System.Text.Json.Serialization;
using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Utilities;
using LowPressureZone.Data.Extensions;
using LowPressureZone.Identity.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Minerals.StringCases;
using Quartz;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.ConfigureKestrel();
builder.ConfigureWebApi();
builder.AddApiServices();

var app = builder.Build();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.UseResponseCaching();
if (app.Environment.IsDevelopment())
{
    app.MapQuartzDashboard("/quartz-dashboard").AllowAnonymous();
}
app.UseFastEndpoints(config =>
{
    config.Endpoints.RoutePrefix = "api";
    config.Errors.ResponseBuilder = (failures, ctx, statusCode) =>
    {
        return new ValidationProblemDetails(failures
                                            .GroupBy(failure => (failure.PropertyName ?? "none").ToCamelCase())
                                            .ToDictionary(failureGrouping => failureGrouping.Key,
                                                          failureGrouping => failureGrouping
                                                                             .Select(failure => failure
                                                                                         .ErrorMessage)
                                                                             .ToArray()))
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "One or more validation errors occurred.",
            Status = statusCode,
            Instance = ctx.Request.Path,
            Extensions =
            {
                {
                    "traceId", ctx.TraceIdentifier
                }
            }
        };
    };
    config.Endpoints.Configurator = endpoints => { endpoints.Throttle(60, 60); };
    config.Errors.ProducesMetadataType = typeof(ValidationProblemDetails);
});
app.MapOpenApi();
app.MapScalarApiReference();

// Unmatched API routes must not fall through to the client below; they are simply not found.
app.MapFallback("/api/{**path}", () => Results.NotFound()).AllowAnonymous();

// Map any non-valid route to index.html, which serves the website. Any navigation to the site via a route that is
// invalid to the API would fail, but this routes the browser to the client, which may have a router implementation for
// that particular route.
app.MapFallbackToFile("index.html").AllowAnonymous();

if (!AppDomain.CurrentDomain.IsTestRunner())
{
    app.Logger.LogInformation("Migrating databases...");
    await app.MigrateDataContextAsync();
    await app.MigrateIdentityContextAsync();
    app.Logger.LogInformation("Finished migrating databases.");
}
else
{
    app.Logger.LogInformation("Skipping database migrations because the application is running in a test runner.");
}

await app.RunAsync();