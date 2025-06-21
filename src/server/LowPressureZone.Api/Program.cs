using System.Text.Json;
using FastEndpoints;
using FastEndpoints.Swagger;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Models.Options;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Minerals.StringCases;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AzuraCastOptions>(builder.Configuration.GetSection(AzuraCastOptions.Name));
builder.Services.Configure<IcecastOptions>(builder.Configuration.GetSection(IcecastOptions.Name));
builder.Services.Configure<StreamingOptions>(builder.Configuration.GetSection(StreamingOptions.Name));
builder.Services.Configure<EmailServiceOptions>(builder.Configuration.GetSection(EmailServiceOptions.Name));
builder.Services.Configure<UrlOptions>(builder.Configuration.GetSection(UrlOptions.Name));

builder.AddDatabases();
builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
builder.Services.ConfigureIdentity(builder.Environment);
builder.Services.ConfigureWebApi();
builder.Services.AddApiServices();

var app = builder.Build();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
if (app.Environment.IsDevelopment()) app.UseCors("Development");
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints(config =>
{
    config.Endpoints.RoutePrefix = "api";
    config.Errors.ResponseBuilder = (failures, ctx, statusCode) =>
    {
        return new ValidationProblemDetails(failures.GroupBy(failure => (failure.PropertyName ?? "none").ToCamelCase())
                                                    .ToDictionary(failureGrouping => failureGrouping.Key,
                                                                  failureGrouping => failureGrouping.Select(failure => failure.ErrorMessage).ToArray()))
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
app.Run();
