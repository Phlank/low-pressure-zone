using System.Text.Json;
using FastEndpoints;
using FastEndpoints.Swagger;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Minerals.StringCases;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddJsonFile("appsettings.json");
if (builder.Environment.IsDevelopment()) builder.Configuration.AddJsonFile("appsettings.Development.json", true);
if (builder.Environment.IsProduction()) builder.Configuration.AddJsonFile("appsettings.Production.json", true);
builder.Services.Configure<EmailServiceOptions>(builder.Configuration.GetSection(EmailServiceOptions.Name));
builder.Services.Configure<UriServiceOptions>(builder.Configuration.GetSection(UriServiceOptions.Name));

builder.AddDatabases();
builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
builder.Services.ConfigureIdentity();
builder.Services.ConfigureWebApi();
builder.Services.AddApiServices();

var app = builder.Build();
app.UseCors(app.Environment.IsDevelopment() ? "Development" : "Production");
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints(config =>
{
    config.Endpoints.RoutePrefix = "api";
    config.Errors.ResponseBuilder = (failures, ctx, statusCode) =>
    {
        return new ValidationProblemDetails(
                                            failures.GroupBy(f => (f.PropertyName ?? "none").ToCamelCase())
                                                    .ToDictionary(
                                                                  e => e.Key,
                                                                  e => e.Select(m => m.ErrorMessage).ToArray()))
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
    config.Errors.ProducesMetadataType = typeof(ValidationProblemDetails);
}).UseSwaggerGen();
app.UseHsts();
app.Run();
