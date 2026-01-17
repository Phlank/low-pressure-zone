using System.Text.Json;
using System.Text.Json.Serialization;
using FastEndpoints;
using FastEndpoints.Swagger;
using LowPressureZone.Api.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Minerals.StringCases;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.ConfigureKestrel();
builder.ConfigureWebApi();
builder.AddApiServices();
builder.CreateFileLocations();

var app = builder.Build();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
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
}).UseSwaggerGen(uiConfig: uiSettings => { uiSettings.CustomStylesheetPath = "/swagger-ui/swagger-dark.css"; });
app.Run();